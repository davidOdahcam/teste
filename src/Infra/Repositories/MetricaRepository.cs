using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.CrossCutting.Enums;
using Autoglass.PlataformaHUB.Domain.Entities;
using Autoglass.PlataformaHUB.Domain.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;

namespace Autoglass.PlataformaHUB.Infra.Repositories
{
    /// <summary>
    /// Implementação do repositório de métricas sobre o NHibernate.
    /// </summary>
    public class MetricaRepository : IMetricaRepository
    {
        private readonly ISession _session;

        public MetricaRepository(ISession session)
        {
            _session = session;
        }

        private IQueryable<Metrica> Metricas => _session.Query<Metrica>();

        public async Task<IReadOnlyList<Metrica>> ObterPorPeriodoAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default)
        {
            return await Metricas
                .Where(m => m.Data >= inicio && m.Data <= fim)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyDictionary<ContextoMetricaEnum, DateTime>> ObterDataUltimoAcessoPorContextoAsync(
            CancellationToken cancellationToken = default)
        {
            List<ContextoUltimoAcesso> registros = await Metricas
                .Where(m => m.Chave == ChaveMetricaEnum.AcessosQuantidade)
                .GroupBy(m => m.Contexto)
                .Select(g => new ContextoUltimoAcesso(g.Key, g.Max(m => m.Data)))
                .ToListAsync(cancellationToken);

            return registros.ToDictionary(r => r.Contexto, r => r.UltimaData);
        }

        public async Task<IReadOnlyDictionary<ContextoMetricaEnum, long>> ObterValorMaisRecentePorContextoAsync(
            ChaveMetricaEnum chave,
            CancellationToken cancellationToken = default)
        {
            // Filtra no banco e resolve o "mais recente por contexto" em memória,
            // garantindo tradução previsível.
            List<RegistroValor> registros = await Metricas
                .Where(m => m.Chave == chave)
                .Select(m => new RegistroValor(m.Contexto, m.Data, m.Valor))
                .ToListAsync(cancellationToken);

            return registros
                .GroupBy(r => r.Contexto)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(r => r.Data).First().Valor);
        }

        private sealed record ContextoUltimoAcesso(ContextoMetricaEnum Contexto, DateTime UltimaData);

        private sealed record RegistroValor(ContextoMetricaEnum Contexto, DateTime Data, long Valor);
    }
}
