using System;
using System.Threading;
using System.Threading.Tasks;
using Autoglass.PlataformaHUB.Domain.Interfaces.Services;
using Autoglass.PlataformaHUB.Domain.Relatorios;

namespace Autoglass.PlataformaHUB.Domain.Services
{
    /// <summary>
    /// Porta de entrada dos relatórios do domínio. Apenas orquestra: delega a montagem
    /// de cada relatório ao seu montador especializado. Novos relatórios = novo montador
    /// injetado e mais um método de delegação aqui.
    /// </summary>
    public class RelatorioService : IRelatorioService
    {
        private readonly IMontadorRelatorioUsoPlataforma _montadorUsoPlataforma;

        public RelatorioService(IMontadorRelatorioUsoPlataforma montadorUsoPlataforma)
        {
            _montadorUsoPlataforma = montadorUsoPlataforma;
        }

        public Task<RelatorioUsoPlataforma> GerarRelatorioUsoPlataformaAsync(
            DateTime inicio,
            DateTime fim,
            CancellationToken cancellationToken = default)
            => _montadorUsoPlataforma.MontarAsync(inicio, fim, cancellationToken);
    }
}
