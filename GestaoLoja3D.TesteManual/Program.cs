using System;
using System.Collections.Generic;
using GestaoLoja3D.Dominio.Models;

// Lembre-se de adicionar a referência de projeto para GestaoLoja3D.Dominio
// antes de rodar este arquivo.

Console.WriteLine("=== Cenário 1: Filamento - criação e cálculo básico ===");
var filamento1 = new Filamento("PLA", "Vermelho", "Voolt", 1m, 120m);
Console.WriteLine($"PrecoPorKg: {filamento1.ValorPorUnidade()} (esperado: 120)");
Console.WriteLine();

Console.WriteLine("=== Cenário 2: Filamento - consumo parcial ===");
filamento1.ConsumirQuantidade(0.3m);
Console.WriteLine($"QuantidadeDisponivelKg apos consumir 0.3: {filamento1.QuantidadeDisponivel} (esperado: 0.7)");
Console.WriteLine($"PrecoPorKg apos consumo: {filamento1.ValorPorUnidade()} (esperado: continua 120)");
Console.WriteLine();

Console.WriteLine("=== Cenário 3: Filamento - consumo além do disponível (deve dar erro) ===");
try
{
    filamento1.ConsumirQuantidade(999m);
    Console.WriteLine("ERRO: não lançou exceção, mas deveria!");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"OK - InvalidOperationException capturada: {ex.Message}");
}
Console.WriteLine();

Console.WriteLine("=== Cenário 4: Filamento - quantidade negativa na criação (deve dar erro) ===");
try
{
    var filamentoInvalido = new Filamento("PLA", "Azul", "Voolt", -5m, 100m);
    Console.WriteLine("ERRO: não lançou exceção, mas deveria!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"OK - ArgumentException capturada: {ex.Message}");
}
Console.WriteLine();

Console.WriteLine("=== Cenário 5: Insumo - mesma lógica, unidade diferente ===");
var insumoParafuso = new Insumo("Parafuso M3", "unidade", 100m, 25m);
Console.WriteLine($"ValorPorUnidade: {insumoParafuso.ValorPorUnidade()} (esperado: 0.25)");
Console.WriteLine();

Console.WriteLine("=== Cenário 6: Herança - Filamento e Insumo como ItemEstoque ===");
var filamentoParaLista = new Filamento("PLA", "Verde", "Voolt", 1m, 100m);
var itens = new List<ItemEstoque> { filamentoParaLista, insumoParafuso };
foreach (var item in itens)
{
    Console.WriteLine($"Item: {item.GetType().Name} - ValorPorUnidade: {item.ValorPorUnidade()}");
}
Console.WriteLine();

Console.WriteLine("=== Cenário 7: MaterialUtilizado - consumo automático ao criar ===");
var filamentoParaMaterial = new Filamento("PLA", "Preto", "Voolt", 1m, 100m);
var materialUtilizado = new MaterialUtilizado(filamentoParaMaterial, 0.2m);
Console.WriteLine($"QuantidadeDisponivelKg apos criar MaterialUtilizado: {filamentoParaMaterial.QuantidadeDisponivel} (esperado: 0.8)");
Console.WriteLine();

Console.WriteLine("=== Cenário 8: Produto - fluxo completo ===");
var produto = new Produto("Vaso Geométrico", TimeSpan.FromHours(3), 10);
var filamentoProduto = new Filamento("PLA", "Branco", "Voolt", 1m, 120m);
var insumoFita = new Insumo("Fita dupla-face", "metro", 10m, 15m);
produto.AdicionarMaterialUtilizado(filamentoProduto, 0.25m);
produto.AdicionarMaterialUtilizado(insumoFita, 0.5m);
Console.WriteLine($"ValorTotalDeFabricacao: {produto.ValorTotalDeFabricao()} (esperado: 30.75)");
Console.WriteLine();

Console.WriteLine("=== Cenário 9: Produto - lista protegida contra alteração externa ===");
Console.WriteLine($"Count antes: {produto.MateriaisUtilizados.Count}");
var filamentoExtra = new Filamento("PLA", "Cinza", "Voolt", 1m, 100m);
produto.MateriaisUtilizados.Add(new MaterialUtilizado(filamentoExtra, 0.1m)); // tenta burlar, adicionando na cópia
Console.WriteLine($"Count depois da tentativa de Add externo: {produto.MateriaisUtilizados.Count} (esperado: igual ao de antes)");
Console.WriteLine();

Console.WriteLine("=== Cenário 10: MaterialUtilizado - material nulo (deve dar erro) ===");
try
{
    produto.AdicionarMaterialUtilizado(null, 1m);
    Console.WriteLine("ERRO: não lançou exceção, mas deveria!");
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"OK - ArgumentNullException capturada: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Testes concluídos. Confira os valores 'esperado' acima com o que foi impresso.");