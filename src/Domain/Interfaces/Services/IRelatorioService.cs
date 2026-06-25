using System;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Domain.Relatorios;

namespace Autoglass.PlataformaHUB.Domain.Interfaces.Services
{
    public interface IRelatorioService
    {
        /// <summary>
        /// Monta o relatório completo de uso da plataforma para o período informado,
        /// sempre retornando a estrutura completa (todos os serviços), mesmo sem métricas.
        /// </summary>
        Task<RelatorioUsoPlataforma> GerarRelatorioUsoPlataformaAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default);
    }
}
