using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public class Filamento : ItemEstoque
    {
       
        public string TipoMaterial { get; private set; }

        public string Cor { get; private set; }

        public string Marca { get; private set; }       

        public Filamento(string tipoMaterial, string cor, string marca, decimal quantidade, decimal valorTotal)
            : base(quantidade, valorTotal)
        {
            TipoMaterial = Validador.ValidarTexto(tipoMaterial, "Tipo de Material");
            Cor = Validador.ValidarTexto(cor, "Cor");
            Marca = Validador.ValidarTexto(marca, "Marca");
        }

    }
}
