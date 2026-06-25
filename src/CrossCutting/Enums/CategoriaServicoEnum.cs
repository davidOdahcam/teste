using System.ComponentModel;

namespace Autoglass.PlataformaHUB.CrossCutting.Enums
{
    /// <summary>
    /// Categoria de agrupamento dos serviços exibida na aba "Visão Serviços" do relatório.
    /// </summary>
    public enum CategoriaServicoEnum
    {
        [Description("Integração")]
        Integracao = 1,

        [Description("Utilitários")]
        Utilitarios = 2,

        [Description("Segurança")]
        Seguranca = 3,

        [Description("Dev")]
        Dev = 4
    }
}
