using System;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Domain.Relatorios;

namespace Autoglass.PlataformaHUB.Domain.Interfaces.Services
{
    /// <summary>
    /// Monta o relatório de uso da plataforma a partir das métricas.
    /// Cada relatório possui o seu próprio montador, mantendo o serviço de relatórios enxuto.
    /// </summary>
    public interface IMontadorRelatorioUsoPlataforma
    {
        Task<RelatorioUsoPlataforma> MontarAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default);
    }
}
