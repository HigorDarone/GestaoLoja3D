using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public static class Validador
    {
        public static string ValidarTexto(string valor, string nomePropriedade)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException($"O campo {nomePropriedade} não pode ser vazio.");
            }
            return valor;
        }

        public static decimal ValidarNumeroDecimal(decimal valor, string nomePropriedade)
        {
            if (valor <= 0)
            {
                throw new ArgumentException($"O campo {nomePropriedade} não pode ser negativo.");
            }
            return valor;
        }

        public static int ValidarNumeroint(int valor, string nomePropriedade)
        {
            if (valor < 0)
            {
                throw new ArgumentException($"O campo {nomePropriedade} não pode ser negativo.");
            }
            return valor;
        }


    }
}
