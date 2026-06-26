using System;
using System.Collections.Generic;
using System.Linq;
using Autoglass.PlataformaHUB.CrossCutting.Enums;

namespace Autoglass.PlataformaHUB.Domain.Configuracoes
{
    /// <summary>
    /// Representa uma linha de "Especificação" no detalhe de um serviço
    /// (ex.: quantidade por tipo Standard/FIFO).
    /// </summary>
    public sealed record EspecificacaoServico(string Rotulo, ChaveMetricaEnum Chave);

    /// <summary>Origem do valor de um indicador (KPI) de serviço.</summary>
    public enum OrigemIndicador
    {
        /// <summary>Soma das chaves dentro do período do relatório (totais principais).</summary>
        SomaNoPeriodo,

        /// <summary>Último valor registrado, ignorando o período (ex.: quantidade de aplicações).</summary>
        ValorMaisRecente
    }

    /// <summary>
    /// Descreve um indicador (KPI) exibido no detalhe de um serviço. Cada serviço pode ter de zero
    /// a vários indicadores, com rótulos e origens diferentes — o front renderiza um card por item.
    /// </summary>
    public sealed record DefinicaoIndicador(
        string Rotulo,
        OrigemIndicador Origem,
        IReadOnlyList<ChaveMetricaEnum> Chaves,
        decimal? HorasPorUnidade = null,
        bool GeraAtividadePeriodo = false,
        IReadOnlyList<EspecificacaoServico>? Especificacoes = null)
    {
        /// <summary>Especificações do indicador (lista vazia quando não houver).</summary>
        public IReadOnlyList<EspecificacaoServico> EspecificacoesSeguras =>
            Especificacoes ?? Array.Empty<EspecificacaoServico>();

        /// <summary>Indica se o indicador gera um KPI derivado de horas economizadas.</summary>
        public bool GeraEconomiaHoras => HorasPorUnidade.HasValue;

        /// <summary>Cria o indicador de quantidade de aplicações (valor mais recente).</summary>
        public static DefinicaoIndicador Aplicacoes(string rotulo = "Aplicações") =>
            new(rotulo, OrigemIndicador.ValorMaisRecente, new[] { ChaveMetricaEnum.AplicacoesQuantidade });

        /// <summary>Cria o indicador principal do serviço (total somado no período).</summary>
        public static DefinicaoIndicador Total(
            string rotulo,
            IReadOnlyList<ChaveMetricaEnum> chaves,
            decimal? horasPorUnidade = null,
            IReadOnlyList<EspecificacaoServico>? especificacoes = null) =>
            new(rotulo, OrigemIndicador.SomaNoPeriodo, chaves,
                horasPorUnidade, GeraAtividadePeriodo: true, especificacoes);
    }

    /// <summary>
    /// Configuração imutável que descreve como um serviço (contexto) deve ser apurado no relatório.
    /// É a fonte da verdade que garante que a estrutura completa do relatório seja retornada,
    /// mesmo quando não houver nenhuma métrica registrada para o serviço no período.
    /// </summary>
    public sealed class DefinicaoServico
    {
        /// <summary>Contexto (serviço) ao qual a definição se refere.</summary>
        public ContextoMetricaEnum Contexto { get; init; }

        /// <summary>Nome de exibição do serviço.</summary>
        public string Nome { get; init; } = string.Empty;

        /// <summary>Categoria de agrupamento do serviço.</summary>
        public CategoriaServicoEnum Categoria { get; init; }

        /// <summary>Indica se o total principal representa provisionamentos (entra nas apurações por empresa).</summary>
        public bool ClassificadoComoProvisionamento { get; init; }

        /// <summary>Indica se o serviço é considerado infraestrutura (KPI e gráfico de consumo por empresa).</summary>
        public bool ClassificadoComoInfraestrutura { get; init; }

        /// <summary>Indicadores (KPIs) exibidos no detalhe do serviço. Vazio quando o serviço não possui detalhes.</summary>
        public IReadOnlyList<DefinicaoIndicador> Indicadores { get; init; } = Array.Empty<DefinicaoIndicador>();

        /// <summary>Indica se o serviço possui seção de detalhes (Claims, por exemplo, não possui).</summary>
        public bool PossuiDetalhes => Indicadores.Count > 0;

        /// <summary>Indicador principal: o total somado no período que alimenta gráfico, economia e apurações por empresa.</summary>
        public DefinicaoIndicador? IndicadorPrincipal =>
            Indicadores.FirstOrDefault(i => i.Origem == OrigemIndicador.SomaNoPeriodo);

        /// <summary>Chaves do indicador principal (usadas nas apurações por empresa/infraestrutura).</summary>
        public IReadOnlyList<ChaveMetricaEnum> ChavesPrincipais =>
            IndicadorPrincipal?.Chaves ?? Array.Empty<ChaveMetricaEnum>();
    }
}
