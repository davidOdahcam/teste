using System.Collections.Generic;
using Autoglass.PlataformaHUB.CrossCutting.Enums;

namespace Autoglass.PlataformaHUB.Domain.Configuracoes
{
    /// <summary>
    /// Representa uma linha de "Especificação" no detalhe de um serviço
    /// (ex.: quantidade por tipo Standard/FIFO).
    /// </summary>
    public sealed record EspecificacaoServico(string Rotulo, ChaveMetricaEnum Chave);

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

        /// <summary>Chaves somadas para compor o total principal do serviço no período.</summary>
        public IReadOnlyList<ChaveMetricaEnum> ChavesTotalPrincipal { get; init; } = new List<ChaveMetricaEnum>();

        /// <summary>Horas economizadas por unidade do total principal. Nulo quando o serviço não gera economia.</summary>
        public decimal? HorasPorUnidade { get; init; }

        /// <summary>Indica se o detalhe deve exibir a quantidade de aplicações mais recente.</summary>
        public bool IncluiAplicacoes { get; init; }

        /// <summary>Indica se o total principal representa provisionamentos (entra nas apurações por empresa).</summary>
        public bool ClassificadoComoProvisionamento { get; init; }

        /// <summary>Indica se o serviço é considerado infraestrutura (KPI e gráfico de consumo por empresa).</summary>
        public bool ClassificadoComoInfraestrutura { get; init; }

        /// <summary>Quebra do total principal por tipo (ordem preservada). Vazio quando não houver especificação.</summary>
        public IReadOnlyList<EspecificacaoServico> Especificacoes { get; init; } = new List<EspecificacaoServico>();

        /// <summary>Indica se o serviço possui seção de detalhes (Claims, por exemplo, não possui).</summary>
        public bool PossuiDetalhes => IncluiAplicacoes || ChavesTotalPrincipal.Count > 0;

        /// <summary>Indica se o serviço apura economia de horas.</summary>
        public bool GeraEconomiaHoras => HorasPorUnidade.HasValue;
    }
}
