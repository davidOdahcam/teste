using System;
using Autoglass.PlataformaHUB.CrossCutting.Enums;

namespace Autoglass.PlataformaHUB.Domain.Entities
{
    /// <summary>
    /// Registro de uma métrica coletada na plataforma. Cada registro representa o valor
    /// de uma <see cref="ChaveMetricaEnum"/> dentro de um <see cref="ContextoMetricaEnum"/>
    /// (serviço), em uma data e para uma empresa.
    /// </summary>
    public class Metrica
    {
        public long Id { get; set; }

        /// <summary>Serviço ao qual a métrica pertence.</summary>
        public ContextoMetricaEnum Contexto { get; set; }

        /// <summary>Indicador medido dentro do contexto.</summary>
        public ChaveMetricaEnum Chave { get; set; }

        /// <summary>Valor acumulado da métrica na data informada.</summary>
        public long Valor { get; set; }

        /// <summary>Data de referência da métrica.</summary>
        public DateTime Data { get; set; }

        /// <summary>Identificador da empresa associada à métrica.</summary>
        public Guid EmpresaId { get; set; }

        /// <summary>Nome da empresa associada à métrica (desnormalizado para o relatório).</summary>
        public string EmpresaNome { get; set; } = string.Empty;
    }
}
