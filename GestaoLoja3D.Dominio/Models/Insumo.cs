using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public class Insumo : ItemEstoque
    {
       

        public string Nome { get; private set; }

        public string UnidadeDeMedida { get; private set; }

        public Insumo(string nome, string unidadeDeMedida, decimal quantidade, decimal valorTotal)
            : base(quantidade, valorTotal)
        {
            Nome = Validador.ValidarTexto(nome, "Nome");
            UnidadeDeMedida = Validador.ValidarTexto(unidadeDeMedida, "Unidade de Medida");
        }

    }
}
