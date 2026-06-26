using System.Threading;
using System.Threading.Tasks;

namespace Autoglass.PlataformaHUB.Domain.Interfaces.Services
{
    /// <summary>
    /// Contrato genérico de montador de relatório. Cada relatório define o seu próprio tipo de
    /// parâmetros de entrada (<typeparamref name="TParametros"/>) e o seu tipo de retorno
    /// (<typeparamref name="TRelatorio"/>), permitindo assinaturas distintas sob a mesma abstração.
    /// </summary>
    public interface IMontadorRelatorio<TParametros, TRelatorio>
    {
        Task<TRelatorio> MontarAsync(TParametros parametros, CancellationToken cancellationToken = default);
    }
}
