using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public class MaterialUtilizado
    {
        public int Id { get; private set; }
        public ItemEstoque Material { get; private set; }

        public decimal QuantidadeUtilizada { get; private set; }

        public MaterialUtilizado(ItemEstoque material, decimal quantidadeUtilizada)
        {
            Material = material ?? throw new ArgumentNullException(nameof(material), "O material não pode ser nulo.");
            QuantidadeUtilizada = quantidadeUtilizada;

            material.ConsumirQuantidade(quantidadeUtilizada);
        }
    }
}
