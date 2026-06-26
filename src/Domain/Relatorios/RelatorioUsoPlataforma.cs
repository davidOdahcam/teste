using System;
using System.Collections.Generic;
using Autoglass.PlataformaHUB.CrossCutting.Enums;

namespace Autoglass.PlataformaHUB.Domain.Relatorios
{
    // ---------- Objetos de valor compartilhados ----------

    /// <summary>Ponto em uma série temporal (data e quantidade).</summary>
    public sealed record PontoTemporal(DateTime Data, long Quantidade);

    /// <summary>Quantidade associada a um serviço.</summary>
    public sealed record ValorPorServico(string Servico, long Quantidade);

    /// <summary>Horas economizadas por serviço.</summary>
    public sealed record EconomiaPorServico(string Servico, decimal Horas);

    /// <summary>Quantidade associada a um rótulo (usado nas especificações).</summary>
    public sealed record ValorPorRotulo(string Rotulo, long Quantidade);

    /// <summary>Quantidade associada a uma empresa.</summary>
    public sealed record ValorPorEmpresa(Guid EmpresaId, string EmpresaNome, long Quantidade);

    /// <summary>Série temporal associada a uma empresa.</summary>
    public sealed record SeriePorEmpresa(Guid EmpresaId, string EmpresaNome, IReadOnlyList<PontoTemporal> Pontos);

    /// <summary>Quantidade de provisionamentos de um serviço para uma empresa.</summary>
    public sealed record ProvisionamentoEmpresaServico(Guid EmpresaId, string EmpresaNome, string Servico, long Quantidade);

    // ---------- Aba: Visão Geral ----------

    public sealed record IndicadoresGerais(
        long AcessosPlataforma,
        decimal PercentualServicosAcessados,
        decimal HorasEconomizadas,
        long ProvisionamentosInfraestrutura);

    public sealed record VisaoGeral(
        IndicadoresGerais Indicadores,
        IReadOnlyList<PontoTemporal> AcessosPorPeriodo,
        IReadOnlyList<ValorPorServico> ServicosMaisAcessados,
        IReadOnlyList<EconomiaPorServico> ServicosMaiorEconomiaHoras);

    // ---------- Aba: Visão Serviços ----------

    /// <summary>Formato de exibição do valor de um indicador (KPI).</summary>
    public enum FormatoIndicador
    {
        Quantidade,
        Horas
    }

    /// <summary>Indicador (KPI) calculado de um serviço, pronto para exibição.</summary>
    public sealed record IndicadorServico(string Rotulo, decimal Valor, FormatoIndicador Formato);

    public sealed record DetalheServico(
        IReadOnlyList<IndicadorServico> Indicadores,
        IReadOnlyList<PontoTemporal> AtividadePorPeriodo,
        IReadOnlyList<ValorPorRotulo> Especificacoes);

    public sealed record ServicoDetalhado(
        ContextoMetricaEnum Contexto,
        string Nome,
        CategoriaServicoEnum Categoria,
        long Acessos,
        DateTime? UltimoUso,
        DetalheServico? Detalhe);

    // ---------- Aba: Empresas ----------

    public sealed record VisaoEmpresas(
        IReadOnlyList<ValorPorEmpresa> ConsumoServicosInfraestrutura,
        IReadOnlyList<SeriePorEmpresa> ProvisionamentosPorPeriodo,
        IReadOnlyList<ProvisionamentoEmpresaServico> ProvisionamentosPorEmpresa);

    // ---------- Raiz ----------

    public sealed record RelatorioUsoPlataforma(
        DateTime PeriodoInicio,
        DateTime PeriodoFim,
        VisaoGeral VisaoGeral,
        IReadOnlyList<ServicoDetalhado> VisaoServicos,
        VisaoEmpresas Empresas);

    /// <summary>Parâmetros de entrada do relatório de uso da plataforma.</summary>
    public sealed record RelatorioUsoPlataformaParametros(DateTime Inicio, DateTime Fim);
}
