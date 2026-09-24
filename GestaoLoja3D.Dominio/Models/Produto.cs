using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public class Produto
    {
        public int Id { get; private set; }

        public string Nome { get; private set; }

        public TimeSpan TempoProducao { get; private set; }

        public int QuantidadeEstoque { get; private set; }

        private List<MaterialUtilizado> materiaisUtilizados = new List<MaterialUtilizado>();

        public List<MaterialUtilizado> MateriaisUtilizados
        {
            get { return new List<MaterialUtilizado>(materiaisUtilizados); }
        }

        public decimal ValorTotalDeFabricao()
        {
            decimal valorTotal = 0;
            foreach (var material in MateriaisUtilizados)
            {
                valorTotal += material.Material.ValorPorUnidade() * material.QuantidadeUtilizada;
            }
            return valorTotal;
        }

        public void AdicionarMaterialUtilizado(ItemEstoque material, decimal quantidadeUtilizada)
        {
            if (quantidadeUtilizada <= 0)
            {
                throw new ArgumentException("A quantidade utilizada deve ser maior que zero.", nameof(quantidadeUtilizada));
            }
            var materialUtilizado = new MaterialUtilizado(material, quantidadeUtilizada);
            materiaisUtilizados.Add(materialUtilizado);
        }

        public Produto(string nome, TimeSpan tempoProducao, int quantidadeEstoque)
        {
            Nome = Validador.ValidarTexto(nome, "Nome");
            TempoProducao = tempoProducao;
            QuantidadeEstoque = Validador.ValidarNumeroint(quantidadeEstoque, "Quantidade em Estoque");
        }
    }
}
