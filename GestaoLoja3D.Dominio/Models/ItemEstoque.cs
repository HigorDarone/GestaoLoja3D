using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoLoja3D.Dominio.Models
{
    public abstract class ItemEstoque
    {
        public int Id { get; private set; }

        public decimal QuantidadeComprada { get; private set; }

        public decimal QuantidadeDisponivel { get; private set; }

        public decimal ValorTotal { get; private set; }

        public DateTime Data { get; private set; } = DateTime.Now;


        public decimal ConsumirQuantidade(decimal quantidadeConsumida)
        {
            if (quantidadeConsumida <= 0)
            {
                throw new ArgumentException("A quantidade consumida deve ser maior que zero.");
            }
            if (quantidadeConsumida > QuantidadeDisponivel)
            {
                throw new InvalidOperationException("Não há filamento suficiente disponível para consumir a quantidade solicitada.");
            }
            QuantidadeDisponivel -= quantidadeConsumida;
            return QuantidadeDisponivel;
        }

        public decimal ValorPorUnidade()
        {
            if (QuantidadeComprada <= 0)
            {
                throw new InvalidOperationException("A quantidade deve ser maior que zero para calcular o valor por unidade.");
            }
            return ValorTotal / QuantidadeComprada;
        }

        protected ItemEstoque(decimal quantidadeComprada, decimal valorTotal)
        {
            QuantidadeComprada = Validador.ValidarNumeroDecimal(quantidadeComprada, "Quantidade Comprada");
            QuantidadeDisponivel = QuantidadeComprada;
            ValorTotal = Validador.ValidarNumeroDecimal(valorTotal, "Valor Total");
        }

    }
}
