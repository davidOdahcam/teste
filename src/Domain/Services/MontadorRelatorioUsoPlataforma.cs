using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.CrossCutting.Enums;
using Autoglass.PlataformaHUB.Domain.Configuracoes;
using Autoglass.PlataformaHUB.Domain.Entities;
using Autoglass.PlataformaHUB.Domain.Interfaces.Repositories;
using Autoglass.PlataformaHUB.Domain.Interfaces.Services;
using Autoglass.PlataformaHUB.Domain.Relatorios;

namespace Autoglass.PlataformaHUB.Domain.Services
{
    /// <summary>
    /// Monta o relatório de uso da plataforma. O método raiz busca os dados de apuração e
    /// delega cada visão do relatório a um método privado, onde fica a regra de negócio.
    /// A estrutura é sempre montada a partir do <see cref="CatalogoServicos"/>, garantindo
    /// o relatório completo mesmo sem métricas.
    /// </summary>
    public class MontadorRelatorioUsoPlataforma
        : IMontadorRelatorio<RelatorioUsoPlataformaParametros, RelatorioUsoPlataforma>
    {
        private readonly IMetricaRepository _metricaRepository;

        public MontadorRelatorioUsoPlataforma(IMetricaRepository metricaRepository)
        {
            _metricaRepository = metricaRepository;
        }

        public async Task<RelatorioUsoPlataforma> MontarAsync(
            RelatorioUsoPlataformaParametros parametros,
            CancellationToken cancellationToken = default)
        {
            var (inicio, fim) = parametros;

            IReadOnlyList<Metrica> metricas = await _metricaRepository
                .ObterPorPeriodoAsync(inicio, fim, cancellationToken);

            IReadOnlyDictionary<ContextoMetricaEnum, DateTime> ultimoAcesso = await _metricaRepository
                .ObterDataUltimoAcessoPorContextoAsync(cancellationToken);

            IReadOnlyDictionary<ContextoMetricaEnum, long> aplicacoes = await _metricaRepository
                .ObterValorMaisRecentePorContextoAsync(CatalogoServicos.ChaveAplicacoes, cancellationToken);

            var totaisPorContextoChave = metricas
                .GroupBy(m => (m.Contexto, m.Chave))
                .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));

            var acessosPorContexto = metricas
                .Where(m => m.Chave == CatalogoServicos.ChaveAcessos)
                .GroupBy(m => m.Contexto)
                .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));

            var apuracao = new ContextoApuracao(
                metricas, ultimoAcesso, aplicacoes, totaisPorContextoChave, acessosPorContexto);

            IReadOnlyList<ServicoDetalhado> servicos = MontarVisaoServicos(apuracao);
            VisaoGeral visaoGeral = MontarVisaoGeral(apuracao, servicos);
            VisaoEmpresas empresas = MontarVisaoEmpresas(apuracao);

            return new RelatorioUsoPlataforma(inicio, fim, visaoGeral, servicos, empresas);
        }

        // ---------- Visão Serviços ----------
        private static IReadOnlyList<ServicoDetalhado> MontarVisaoServicos(ContextoApuracao apuracao)
        {
            long Somar(ContextoMetricaEnum contexto, IReadOnlyList<ChaveMetricaEnum> chaves) =>
                chaves.Sum(chave => apuracao.TotaisPorContextoChave.GetValueOrDefault((contexto, chave)));

            DateTime? UltimoUso(ContextoMetricaEnum contexto) =>
                apuracao.UltimoAcessoPorContexto.TryGetValue(contexto, out DateTime data) ? data : null;

            IReadOnlyList<PontoTemporal> Atividade(ContextoMetricaEnum contexto, IReadOnlyList<ChaveMetricaEnum> chaves) =>
                apuracao.Metricas
                    .Where(m => m.Contexto == contexto && chaves.Contains(m.Chave))
                    .GroupBy(m => m.Data.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new PontoTemporal(g.Key, g.Sum(m => m.Valor)))
                    .ToList();

            DetalheServico? MontarDetalhe(DefinicaoServico definicao)
            {
                if (!definicao.PossuiDetalhes)
                    return null;

                long total = Somar(definicao.Contexto, definicao.ChavesTotalPrincipal);

                return new DetalheServico(
                    definicao.IncluiAplicacoes ? apuracao.AplicacoesPorContexto.GetValueOrDefault(definicao.Contexto) : null,
                    total,
                    definicao.GeraEconomiaHoras ? total * definicao.HorasPorUnidade!.Value : null,
                    Atividade(definicao.Contexto, definicao.ChavesTotalPrincipal),
                    definicao.Especificacoes
                        .Select(e => new ValorPorRotulo(
                            e.Rotulo, apuracao.TotaisPorContextoChave.GetValueOrDefault((definicao.Contexto, e.Chave))))
                        .ToList());
            }

            return CatalogoServicos.Todos
                .Select(definicao => new ServicoDetalhado(
                    definicao.Contexto,
                    definicao.Nome,
                    definicao.Categoria,
                    apuracao.AcessosPorContexto.GetValueOrDefault(definicao.Contexto),
                    UltimoUso(definicao.Contexto),
                    MontarDetalhe(definicao)))
                .ToList();
        }

        // ---------- Visão Geral ----------
        private static VisaoGeral MontarVisaoGeral(ContextoApuracao apuracao, IReadOnlyList<ServicoDetalhado> servicos)
        {
            long Somar(ContextoMetricaEnum contexto, IReadOnlyList<ChaveMetricaEnum> chaves) =>
                chaves.Sum(chave => apuracao.TotaisPorContextoChave.GetValueOrDefault((contexto, chave)));

            decimal EconomiaServico(DefinicaoServico definicao) =>
                definicao.GeraEconomiaHoras
                    ? Somar(definicao.Contexto, definicao.ChavesTotalPrincipal) * definicao.HorasPorUnidade!.Value
                    : 0m;

            long acessosPlataforma = servicos.Sum(s => s.Acessos);
            int servicosAcessados = servicos.Count(s => s.Acessos > 0);
            decimal percentualAcessados = servicos.Count == 0
                ? 0m
                : Math.Round((decimal)servicosAcessados / servicos.Count * 100m, 2);

            decimal horasEconomizadas = CatalogoServicos.Todos.Sum(EconomiaServico);

            long provisionamentosInfra = CatalogoServicos.ServicosInfraestrutura
                .Sum(d => Somar(d.Contexto, d.ChavesTotalPrincipal));

            var indicadores = new IndicadoresGerais(
                acessosPlataforma,
                percentualAcessados,
                Math.Round(horasEconomizadas, 2),
                provisionamentosInfra);

            IReadOnlyList<PontoTemporal> acessosPorPeriodo = apuracao.Metricas
                .Where(m => m.Chave == CatalogoServicos.ChaveAcessos)
                .GroupBy(m => m.Data.Date)
                .OrderBy(g => g.Key)
                .Select(g => new PontoTemporal(g.Key, g.Sum(m => m.Valor)))
                .ToList();

            IReadOnlyList<ValorPorServico> maisAcessados = servicos
                .OrderByDescending(s => s.Acessos)
                .ThenBy(s => s.Nome)
                .Select(s => new ValorPorServico(s.Nome, s.Acessos))
                .ToList();

            IReadOnlyList<EconomiaPorServico> maiorEconomia = CatalogoServicos.Todos
                .Where(d => d.GeraEconomiaHoras)
                .Select(d => new EconomiaPorServico(d.Nome, Math.Round(EconomiaServico(d), 2)))
                .OrderByDescending(e => e.Horas)
                .ThenBy(e => e.Servico)
                .ToList();

            return new VisaoGeral(indicadores, acessosPorPeriodo, maisAcessados, maiorEconomia);
        }

        // ---------- Visão Empresas ----------
        private static VisaoEmpresas MontarVisaoEmpresas(ContextoApuracao apuracao)
        {
            HashSet<(ContextoMetricaEnum, ChaveMetricaEnum)> paresProvisionamento = CatalogoServicos.ServicosProvisionamento
                .SelectMany(d => d.ChavesTotalPrincipal.Select(ch => (d.Contexto, ch)))
                .ToHashSet();

            HashSet<(ContextoMetricaEnum, ChaveMetricaEnum)> paresInfraestrutura = CatalogoServicos.ServicosInfraestrutura
                .SelectMany(d => d.ChavesTotalPrincipal.Select(ch => (d.Contexto, ch)))
                .ToHashSet();

            List<Metrica> metricasProvisionamento = apuracao.Metricas
                .Where(m => paresProvisionamento.Contains((m.Contexto, m.Chave)))
                .ToList();

            IReadOnlyList<ValorPorEmpresa> consumoInfra = apuracao.Metricas
                .Where(m => paresInfraestrutura.Contains((m.Contexto, m.Chave)))
                .GroupBy(m => new { m.EmpresaId, m.EmpresaNome })
                .Select(g => new ValorPorEmpresa(g.Key.EmpresaId, g.Key.EmpresaNome, g.Sum(m => m.Valor)))
                .OrderByDescending(e => e.Quantidade)
                .ThenBy(e => e.EmpresaNome)
                .ToList();

            IReadOnlyList<SeriePorEmpresa> provisionamentosPorPeriodo = metricasProvisionamento
                .GroupBy(m => new { m.EmpresaId, m.EmpresaNome })
                .Select(g => new SeriePorEmpresa(
                    g.Key.EmpresaId,
                    g.Key.EmpresaNome,
                    g.GroupBy(m => m.Data.Date)
                        .OrderBy(d => d.Key)
                        .Select(d => new PontoTemporal(d.Key, d.Sum(m => m.Valor)))
                        .ToList()))
                .OrderBy(s => s.EmpresaNome)
                .ToList();

            var totaisPorEmpresaContexto = metricasProvisionamento
                .GroupBy(m => (m.EmpresaId, m.EmpresaNome, m.Contexto))
                .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));

            var empresas = metricasProvisionamento
                .Select(m => (m.EmpresaId, m.EmpresaNome))
                .Distinct()
                .OrderBy(e => e.EmpresaNome);

            var provisionamentosPorEmpresa = new List<ProvisionamentoEmpresaServico>();
            foreach (var empresa in empresas)
            {
                foreach (var servico in CatalogoServicos.ServicosProvisionamento.OrderBy(d => d.Nome))
                {
                    totaisPorEmpresaContexto.TryGetValue(
                        (empresa.EmpresaId, empresa.EmpresaNome, servico.Contexto), out long quantidade);

                    provisionamentosPorEmpresa.Add(new ProvisionamentoEmpresaServico(
                        empresa.EmpresaId, empresa.EmpresaNome, servico.Nome, quantidade));
                }
            }

            return new VisaoEmpresas(consumoInfra, provisionamentosPorPeriodo, provisionamentosPorEmpresa);
        }

        /// <summary>
        /// Contêiner com os dados de apuração (brutos e pré-agregados) necessários para montar as visões.
        /// </summary>
        private sealed record ContextoApuracao(
            IReadOnlyList<Metrica> Metricas,
            IReadOnlyDictionary<ContextoMetricaEnum, DateTime> UltimoAcessoPorContexto,
            IReadOnlyDictionary<ContextoMetricaEnum, long> AplicacoesPorContexto,
            IReadOnlyDictionary<(ContextoMetricaEnum, ChaveMetricaEnum), long> TotaisPorContextoChave,
            IReadOnlyDictionary<ContextoMetricaEnum, long> AcessosPorContexto);
    }
}
