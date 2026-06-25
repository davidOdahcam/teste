using System;
using System.ComponentModel;
using System.Reflection;

namespace Autoglass.PlataformaHUB.CrossCutting.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Obtém o texto definido no atributo <see cref="DescriptionAttribute"/> do valor de enum.
        /// Caso o atributo não exista, retorna o próprio nome do valor.
        /// </summary>
        public static string ObterDescricao(this Enum valor)
        {
            FieldInfo? campo = valor.GetType().GetField(valor.ToString());

            if (campo is null)
                return valor.ToString();

            var atributo = (DescriptionAttribute?)Attribute.GetCustomAttribute(campo, typeof(DescriptionAttribute));

            return atributo?.Description ?? valor.ToString();
        }
    }
}
