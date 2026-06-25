using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.CrossCutting.Enums;
using Autoglass.PlataformaHUB.Domain.Entities;

namespace Autoglass.PlataformaHUB.Domain.Interfaces.Repositories
{
    public interface IMetricaRepository
    {
        /// <summary>
        /// Retorna todas as métricas com data dentro do intervalo informado (inclusive).
        /// </summary>
        Task<IReadOnlyList<Metrica>> ObterPorPeriodoAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna a data do último acesso de cada serviço considerando todo o histórico
        /// (ignora o período do relatório). Contextos sem acesso não aparecem no dicionário.
        /// </summary>
        Task<IReadOnlyDictionary<ContextoMetricaEnum, DateTime>> ObterDataUltimoAcessoPorContextoAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna o valor mais recente (por data) de uma chave para cada serviço, considerando
        /// todo o histórico. Usado, por exemplo, para a "quantidade de aplicações" mais recente.
        /// </summary>
        Task<IReadOnlyDictionary<ContextoMetricaEnum, long>> ObterValorMaisRecentePorContextoAsync(
            ChaveMetricaEnum chave,
            CancellationToken cancellationToken = default);
    }
}
