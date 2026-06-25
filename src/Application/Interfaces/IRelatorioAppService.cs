using System;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Application.ViewModels.Responses;

namespace Autoglass.PlataformaHUB.Application.Interfaces
{
    public interface IRelatorioAppService
    {
        /// <summary>
        /// Orquestra a geração do relatório de uso da plataforma e devolve o contrato pronto para a API.
        /// </summary>
        Task<RelatorioUsoPlataformaResponse> ObterUsoPlataformaAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default);
    }
}
