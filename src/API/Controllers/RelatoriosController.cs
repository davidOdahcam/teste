using System;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Application.Interfaces;
using Autoglass.PlataformaHUB.Application.ViewModels.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Autoglass.PlataformaHUB.API.Controllers
{
    [ApiController]
    [Route("api/relatorios")]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioAppService _relatorioAppService;

        public RelatoriosController(IRelatorioAppService relatorioAppService)
        {
            _relatorioAppService = relatorioAppService;
        }

        /// <summary>
        /// Retorna o relatório completo de uso da plataforma para o período informado.
        /// A estrutura completa é sempre devolvida, mesmo sem métricas registradas no período.
        /// </summary>
        /// <param name="inicio">Data inicial do período (inclusive).</param>
        /// <param name="fim">Data final do período (inclusive).</param>
        [HttpGet("uso-plataforma")]
        [ProducesResponseType(typeof(RelatorioUsoPlataformaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterUsoPlataforma(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim,
            CancellationToken cancellationToken)
        {
            if (inicio > fim)
                return BadRequest("A data inicial não pode ser maior que a data final.");

            RelatorioUsoPlataformaResponse relatorio = await _relatorioAppService
                .ObterUsoPlataformaAsync(inicio, fim, cancellationToken);

            return Ok(relatorio);
        }
    }
}
