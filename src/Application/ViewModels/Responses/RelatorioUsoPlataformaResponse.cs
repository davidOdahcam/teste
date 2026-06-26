using System;
using System.Collections.Generic;

namespace Autoglass.PlataformaHUB.Application.ViewModels.Responses
{
    // ---------- Objetos de valor compartilhados ----------

    public sealed record PontoTemporalResponse(DateTime Data, long Quantidade);

    public sealed record ValorPorServicoResponse(string Servico, long Quantidade);

    public sealed record EconomiaPorServicoResponse(string Servico, decimal Horas);

    public sealed record ValorPorRotuloResponse(string Rotulo, long Quantidade);

    public sealed record ValorPorEmpresaResponse(Guid EmpresaId, string EmpresaNome, long Quantidade);

    public sealed record SeriePorEmpresaResponse(Guid EmpresaId, string EmpresaNome, IReadOnlyList<PontoTemporalResponse> Pontos);

    public sealed record ProvisionamentoEmpresaServicoResponse(Guid EmpresaId, string EmpresaNome, string Servico, long Quantidade);

    // ---------- Aba: Visão Geral ----------

    public sealed record IndicadoresGeraisResponse(
        long AcessosPlataforma,
        decimal PercentualServicosAcessados,
        decimal HorasEconomizadas,
        long ProvisionamentosInfraestrutura);

    public sealed record VisaoGeralResponse(
        IndicadoresGeraisResponse Indicadores,
        IReadOnlyList<PontoTemporalResponse> AcessosPorPeriodo,
        IReadOnlyList<ValorPorServicoResponse> ServicosMaisAcessados,
        IReadOnlyList<EconomiaPorServicoResponse> ServicosMaiorEconomiaHoras);

    // ---------- Aba: Visão Serviços ----------

    public sealed record DetalheServicoResponse(
        long? Aplicacoes,
        long Total,
        decimal? HorasEconomizadas,
        IReadOnlyList<PontoTemporalResponse> AtividadePorPeriodo,
        IReadOnlyList<ValorPorRotuloResponse> Especificacoes);

    public sealed record ServicoDetalhadoResponse(
        string Nome,
        string Categoria,
        long Acessos,
        DateTime? UltimoUso,
        DetalheServicoResponse? Detalhe);

    // ---------- Aba: Empresas ----------

    public sealed record VisaoEmpresasResponse(
        IReadOnlyList<ValorPorEmpresaResponse> ConsumoServicosInfraestrutura,
        IReadOnlyList<SeriePorEmpresaResponse> ProvisionamentosPorPeriodo,
        IReadOnlyList<ProvisionamentoEmpresaServicoResponse> ProvisionamentosPorEmpresa);

    // ---------- Raiz ----------

    public sealed record RelatorioUsoPlataformaResponse(
        DateTime PeriodoInicio,
        DateTime PeriodoFim,
        VisaoGeralResponse VisaoGeral,
        IReadOnlyList<ServicoDetalhadoResponse> VisaoServicos,
        VisaoEmpresasResponse Empresas);
}
