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
    /// Responsável por transformar as métricas brutas no relatório de uso da plataforma.
    /// A estrutura é sempre montada a partir do <see cref="CatalogoServicos"/>, de modo que
    /// todos os serviços e seções existam mesmo quando não houver métricas no período.
    /// </summary>
    public class RelatorioService : IRelatorioService
    {
        private readonly IMetricaRepository _metricaRepository;

        public RelatorioService(IMetricaRepository metricaRepository)
        {
            _metricaRepository = metricaRepository;
        }

        public async Task<RelatorioUsoPlataforma> GerarRelatorioUsoPlataformaAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Metrica> metricas = await _metricaRepository
                .ObterPorPeriodoAsync(inicio, fim, cancellationToken);

            IReadOnlyDictionary<ContextoMetricaEnum, DateTime> ultimoAcessoPorContexto = await _metricaRepository
                .ObterDataUltimoAcessoPorContextoAsync(cancellationToken);

            IReadOnlyDictionary<ContextoMetricaEnum, long> aplicacoesPorContexto = await _metricaRepository
                .ObterValorMaisRecentePorContextoAsync(CatalogoServicos.ChaveAplicacoes, cancellationToken);

            var contexto = new ContextoApuracao(metricas, ultimoAcessoPorContexto, aplicacoesPorContexto);

            IReadOnlyList<ServicoDetalhado> servicos = MontarServicos(contexto);

            VisaoGeral visaoGeral = MontarVisaoGeral(contexto, servicos);
            VisaoEmpresas empresas = MontarVisaoEmpresas(metricas);

            return new RelatorioUsoPlataforma(inicio, fim, visaoGeral, servicos, empresas);
        }

        private static IReadOnlyList<ServicoDetalhado> MontarServicos(ContextoApuracao apuracao)
        {
            return CatalogoServicos.Todos
                .Select(definicao => MontarServico(definicao, apuracao))
                .ToList();
        }

        private static ServicoDetalhado MontarServico(DefinicaoServico definicao, ContextoApuracao apuracao)
        {
            long acessos = apuracao.ObterAcessos(definicao.Contexto);
            DateTime? ultimoUso = apuracao.ObterUltimoAcesso(definicao.Contexto);

            DetalheServico? detalhe = definicao.PossuiDetalhes
                ? MontarDetalhe(definicao, apuracao)
                : null;

            return new ServicoDetalhado(
                definicao.Contexto,
                definicao.Nome,
                definicao.Categoria,
                acessos,
                ultimoUso,
                detalhe);
        }

        private static DetalheServico MontarDetalhe(DefinicaoServico definicao, ContextoApuracao apuracao)
        {
            long totalPrincipal = apuracao.SomarChaves(definicao.Contexto, definicao.ChavesTotalPrincipal);

            long? aplicacoes = definicao.IncluiAplicacoes
                ? apuracao.ObterAplicacoes(definicao.Contexto)
                : null;

            decimal? horasEconomizadas = definicao.GeraEconomiaHoras
                ? totalPrincipal * definicao.HorasPorUnidade!.Value
                : null;

            IReadOnlyList<PontoTemporal> atividade = apuracao
                .ApurarAtividade(definicao.Contexto, definicao.ChavesTotalPrincipal);

            IReadOnlyList<ValorPorRotulo> especificacoes = definicao.Especificacoes
                .Select(e => new ValorPorRotulo(e.Rotulo, apuracao.SomarChave(definicao.Contexto, e.Chave)))
                .ToList();

            return new DetalheServico(
                aplicacoes,
                definicao.RotuloTotalPrincipal,
                totalPrincipal,
                horasEconomizadas,
                atividade,
                especificacoes);
        }

        private static VisaoGeral MontarVisaoGeral(
            ContextoApuracao apuracao,
            IReadOnlyList<ServicoDetalhado> servicos)
        {
            long acessosPlataforma = apuracao.TotalAcessos;

            int totalServicos = CatalogoServicos.Todos.Count;
            int servicosAcessados = servicos.Count(s => s.Acessos > 0);
            decimal percentualAcessados = totalServicos == 0
                ? 0m
                : Math.Round((decimal)servicosAcessados / totalServicos * 100m, 2);

            decimal horasEconomizadas = servicos
                .Where(s => s.Detalhe?.HorasEconomizadas is not null)
                .Sum(s => s.Detalhe!.HorasEconomizadas!.Value);

            long provisionamentosInfra = CatalogoServicos.ServicosInfraestrutura
                .Sum(d => apuracao.SomarChaves(d.Contexto, d.ChavesTotalPrincipal));

            var indicadores = new IndicadoresGerais(
                acessosPlataforma,
                percentualAcessados,
                Math.Round(horasEconomizadas, 2),
                provisionamentosInfra);

            IReadOnlyList<PontoTemporal> acessosPorPeriodo = apuracao.AcessosPorPeriodo();

            IReadOnlyList<ValorPorServico> maisAcessados = servicos
                .OrderByDescending(s => s.Acessos)
                .ThenBy(s => s.Nome)
                .Select(s => new ValorPorServico(s.Nome, s.Acessos))
                .ToList();

            IReadOnlyList<EconomiaPorServico> maiorEconomia = servicos
                .Where(s => s.Detalhe?.HorasEconomizadas is not null)
                .OrderByDescending(s => s.Detalhe!.HorasEconomizadas!.Value)
                .ThenBy(s => s.Nome)
                .Select(s => new EconomiaPorServico(s.Nome, s.Detalhe!.HorasEconomizadas!.Value))
                .ToList();

            return new VisaoGeral(indicadores, acessosPorPeriodo, maisAcessados, maiorEconomia);
        }

        private static VisaoEmpresas MontarVisaoEmpresas(IReadOnlyList<Metrica> metricas)
        {
            HashSet<(ContextoMetricaEnum, ChaveMetricaEnum)> paresProvisionamento = CatalogoServicos.ServicosProvisionamento
                .SelectMany(d => d.ChavesTotalPrincipal.Select(ch => (d.Contexto, ch)))
                .ToHashSet();

            HashSet<(ContextoMetricaEnum, ChaveMetricaEnum)> paresInfraestrutura = CatalogoServicos.ServicosInfraestrutura
                .SelectMany(d => d.ChavesTotalPrincipal.Select(ch => (d.Contexto, ch)))
                .ToHashSet();

            List<Metrica> metricasProvisionamento = metricas
                .Where(m => paresProvisionamento.Contains((m.Contexto, m.Chave)))
                .ToList();

            IReadOnlyList<ValorPorEmpresa> consumoInfra = metricas
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

            IReadOnlyList<ProvisionamentoEmpresaServico> provisionamentosPorEmpresa =
                MontarTabelaProvisionamentos(metricasProvisionamento);

            return new VisaoEmpresas(consumoInfra, provisionamentosPorPeriodo, provisionamentosPorEmpresa);
        }

        private static IReadOnlyList<ProvisionamentoEmpresaServico> MontarTabelaProvisionamentos(
            IReadOnlyList<Metrica> metricasProvisionamento)
        {
            var empresas = metricasProvisionamento
                .Select(m => new { m.EmpresaId, m.EmpresaNome })
                .Distinct()
                .OrderBy(e => e.EmpresaNome)
                .ToList();

            var servicos = CatalogoServicos.ServicosProvisionamento
                .OrderBy(d => d.Nome)
                .ToList();

            var totaisPorEmpresaContexto = metricasProvisionamento
                .GroupBy(m => (m.EmpresaId, m.Contexto))
                .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));

            var linhas = new List<ProvisionamentoEmpresaServico>();

            foreach (var empresa in empresas)
            {
                foreach (var servico in servicos)
                {
                    totaisPorEmpresaContexto.TryGetValue((empresa.EmpresaId, servico.Contexto), out long quantidade);
                    linhas.Add(new ProvisionamentoEmpresaServico(
                        empresa.EmpresaId,
                        empresa.EmpresaNome,
                        servico.Nome,
                        quantidade));
                }
            }

            return linhas;
        }

        /// <summary>
        /// Agrupa em memória as métricas do período para apurações repetidas de forma eficiente.
        /// </summary>
        private sealed class ContextoApuracao
        {
            private readonly IReadOnlyList<Metrica> _metricas;
            private readonly IReadOnlyDictionary<ContextoMetricaEnum, DateTime> _ultimoAcessoPorContexto;
            private readonly IReadOnlyDictionary<ContextoMetricaEnum, long> _aplicacoesPorContexto;
            private readonly Dictionary<(ContextoMetricaEnum, ChaveMetricaEnum), long> _totaisPorContextoChave;
            private readonly Dictionary<ContextoMetricaEnum, long> _acessosPorContexto;

            public ContextoApuracao(
                IReadOnlyList<Metrica> metricas,
                IReadOnlyDictionary<ContextoMetricaEnum, DateTime> ultimoAcessoPorContexto,
                IReadOnlyDictionary<ContextoMetricaEnum, long> aplicacoesPorContexto)
            {
                _metricas = metricas;
                _ultimoAcessoPorContexto = ultimoAcessoPorContexto;
                _aplicacoesPorContexto = aplicacoesPorContexto;

                _totaisPorContextoChave = metricas
                    .GroupBy(m => (m.Contexto, m.Chave))
                    .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));

                _acessosPorContexto = metricas
                    .Where(m => m.Chave == CatalogoServicos.ChaveAcessos)
                    .GroupBy(m => m.Contexto)
                    .ToDictionary(g => g.Key, g => g.Sum(m => m.Valor));
            }

            public long TotalAcessos => _acessosPorContexto.Values.Sum();

            public long ObterAcessos(ContextoMetricaEnum contexto) =>
                _acessosPorContexto.TryGetValue(contexto, out long valor) ? valor : 0;

            public DateTime? ObterUltimoAcesso(ContextoMetricaEnum contexto) =>
                _ultimoAcessoPorContexto.TryGetValue(contexto, out DateTime data) ? data : null;

            public long ObterAplicacoes(ContextoMetricaEnum contexto) =>
                _aplicacoesPorContexto.TryGetValue(contexto, out long valor) ? valor : 0;

            public long SomarChave(ContextoMetricaEnum contexto, ChaveMetricaEnum chave) =>
                _totaisPorContextoChave.TryGetValue((contexto, chave), out long valor) ? valor : 0;

            public long SomarChaves(ContextoMetricaEnum contexto, IReadOnlyList<ChaveMetricaEnum> chaves) =>
                chaves.Sum(chave => SomarChave(contexto, chave));

            public IReadOnlyList<PontoTemporal> AcessosPorPeriodo() =>
                _metricas
                    .Where(m => m.Chave == CatalogoServicos.ChaveAcessos)
                    .GroupBy(m => m.Data.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new PontoTemporal(g.Key, g.Sum(m => m.Valor)))
                    .ToList();

            public IReadOnlyList<PontoTemporal> ApurarAtividade(
                ContextoMetricaEnum contexto,
                IReadOnlyList<ChaveMetricaEnum> chaves)
            {
                var chavesSet = chaves.ToHashSet();

                return _metricas
                    .Where(m => m.Contexto == contexto && chavesSet.Contains(m.Chave))
                    .GroupBy(m => m.Data.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new PontoTemporal(g.Key, g.Sum(m => m.Valor)))
                    .ToList();
            }
        }
    }
}
