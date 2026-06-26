using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Application.Interfaces;
using Autoglass.PlataformaHUB.Application.ViewModels.Responses;
using Autoglass.PlataformaHUB.CrossCutting.Extensions;
using Autoglass.PlataformaHUB.Domain.Interfaces.Services;
using Autoglass.PlataformaHUB.Domain.Relatorios;

namespace Autoglass.PlataformaHUB.Application.Services
{
    public class RelatorioAppService : IRelatorioAppService
    {
        private readonly IRelatorioService _relatorioService;

        public RelatorioAppService(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        public async Task<RelatorioUsoPlataformaResponse> ObterUsoPlataformaAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default)
        {
            RelatorioUsoPlataforma relatorio = await _relatorioService
                .GerarRelatorioUsoPlataformaAsync(inicio, fim, cancellationToken);

            return MapearRelatorio(relatorio);
        }

        private static RelatorioUsoPlataformaResponse MapearRelatorio(RelatorioUsoPlataforma relatorio)
        {
            return new RelatorioUsoPlataformaResponse(
                relatorio.PeriodoInicio,
                relatorio.PeriodoFim,
                MapearVisaoGeral(relatorio.VisaoGeral),
                relatorio.VisaoServicos.Select(MapearServico).ToList(),
                MapearEmpresas(relatorio.Empresas));
        }

        private static VisaoGeralResponse MapearVisaoGeral(VisaoGeral visaoGeral)
        {
            var indicadores = new IndicadoresGeraisResponse(
                visaoGeral.Indicadores.AcessosPlataforma,
                visaoGeral.Indicadores.PercentualServicosAcessados,
                visaoGeral.Indicadores.HorasEconomizadas,
                visaoGeral.Indicadores.ProvisionamentosInfraestrutura);

            return new VisaoGeralResponse(
                indicadores,
                visaoGeral.AcessosPorPeriodo.Select(MapearPonto).ToList(),
                visaoGeral.ServicosMaisAcessados
                    .Select(s => new ValorPorServicoResponse(s.Servico, s.Quantidade)).ToList(),
                visaoGeral.ServicosMaiorEconomiaHoras
                    .Select(s => new EconomiaPorServicoResponse(s.Servico, s.Horas)).ToList());
        }

        private static ServicoDetalhadoResponse MapearServico(ServicoDetalhado servico)
        {
            DetalheServicoResponse? detalhe = servico.Detalhe is null
                ? null
                : new DetalheServicoResponse(
                    servico.Detalhe.Indicadores
                        .Select(i => new IndicadorServicoResponse(i.Rotulo, i.Valor, i.Formato.ToString())).ToList(),
                    servico.Detalhe.AtividadePorPeriodo.Select(MapearPonto).ToList(),
                    servico.Detalhe.Especificacoes
                        .Select(e => new ValorPorRotuloResponse(e.Rotulo, e.Quantidade)).ToList());

            return new ServicoDetalhadoResponse(
                servico.Nome,
                servico.Categoria.ObterDescricao(),
                servico.Acessos,
                servico.UltimoUso,
                detalhe);
        }

        private static VisaoEmpresasResponse MapearEmpresas(VisaoEmpresas empresas)
        {
            return new VisaoEmpresasResponse(
                empresas.ConsumoServicosInfraestrutura
                    .Select(e => new ValorPorEmpresaResponse(e.EmpresaId, e.EmpresaNome, e.Quantidade)).ToList(),
                empresas.ProvisionamentosPorPeriodo
                    .Select(s => new SeriePorEmpresaResponse(
                        s.EmpresaId,
                        s.EmpresaNome,
                        s.Pontos.Select(MapearPonto).ToList())).ToList(),
                empresas.ProvisionamentosPorEmpresa
                    .Select(p => new ProvisionamentoEmpresaServicoResponse(
                        p.EmpresaId, p.EmpresaNome, p.Servico, p.Quantidade)).ToList());
        }

        private static PontoTemporalResponse MapearPonto(PontoTemporal ponto) =>
            new(ponto.Data, ponto.Quantidade);
    }
}
