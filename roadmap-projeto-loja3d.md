# Roadmap do Projeto — Gestão da Loja de Impressão 3D

## Visão geral

Sistema em C#/.NET para gerenciar uma loja real de impressão 3D: controle de remessas de filamento e insumos, cálculo de custo de produção, e um módulo de IA para apoio ao atendimento. Estrutura em dois projetos: `GestaoLoja3D.Dominio` (Class Library, regras de negócio) e `GestaoLoja3D` (Blazor Web App, interface).

Meta de MVP: sábado (escopo reduzido — domínio + CRUD Blazor + cálculo de custo + um módulo de IA).

## Fase 1 — Modelagem do domínio (em andamento)

Objetivo: modelar as entidades principais aplicando o mesmo cuidado de encapsulamento e validação já usado no projeto da livraria.

Status atual:

- **Estrutura do projeto — concluída.** Solution com `GestaoLoja3D.Dominio` (Class Library) referenciado pelo projeto Blazor `GestaoLoja3D` (Interactive Server), seguindo o padrão de dependência de mão única já usado antes.
- **Filamento — concluída (modelagem).** Representa uma remessa (lote de compra) de filamento, não um "tipo" genérico — decisão tomada para preservar o preço por kg correto de cada compra (mesmo raciocínio do preço congelado em `ItemPedido`, no projeto da livraria), com consumo por ordem de chegada (FIFO) entre remessas do mesmo tipo/marca/cor. Propriedades: `Id`, `TipoMaterial`, `Cor`, `Marca`, `QuantidadeCompradaKg`, `QuantidadeDisponivelKg`, `PrecoPago`, `DataCompra` — todas `get` público / `set` privado, validadas via `Validador` dentro do construtor (nunca no `set`, para evitar o mesmo bug de re-execução de lógica que o EF Core causou no `Usuario` do projeto anterior). `QuantidadeDisponivelKg` só muda através do método `ConsumirQuantidade(decimal kg)`, que valida quantidade negativa (`ArgumentException`) e quantidade insuficiente (`InvalidOperationException`) antes de descontar. `PrecoPorKg()` é um método calculado (`PrecoPago / QuantidadeCompradaKg`). Pendente para a Fase 2: construtor privado sem parâmetros para materialização via EF Core.
- **Insumo — concluída (modelagem).** Classe genérica pra qualquer insumo (parafuso, caixa, fita etc.), com `Nome` e `UnidadeDeMedida` (unidade, metro...) em vez de propriedades fixas por tipo.
- **ItemEstoque — concluída.** Classe base abstrata (`abstract class`), introduzindo herança no projeto. Reúne tudo que `Filamento` e `Insumo` têm em comum: `Id`, `QuantidadeComprada`, `QuantidadeDisponivel`, `ValorTotal`, `Data`, o método `ConsumirQuantidade()` (validação de quantidade negativa/insuficiente, mesmo padrão nos dois) e `ValorPorUnidade()` (preço por kg ou por unidade — mesma fórmula, `ValorTotal / QuantidadeComprada`). Construtor `protected`, chamado via `base(...)` pelas classes filhas, que só validam o que é específico de cada uma (`TipoMaterial`/`Cor`/`Marca` no Filamento; `Nome`/`UnidadeDeMedida` no Insumo).
- **MaterialUtilizado — concluída.** Classe de associação (não herda de nada, apenas referencia um `ItemEstoque`) ligando um `Produto` a um material/insumo específico consumido, com `QuantidadeUtilizada`. O construtor já desconta automaticamente do estoque (`ItemEstoque.ConsumirQuantidade`) no momento em que o consumo é registrado.
- **Produto — concluída (modelagem).** `Id`, `Nome`, `TempoProducao` (`TimeSpan`, não `TimeOnly` — representa duração, não horário), `QuantidadeEstoque`, e uma lista privada de `MaterialUtilizado` exposta por cópia defensiva (mesmo padrão do `ItensPedido` no projeto da livraria). Novos materiais só entram via `AdicionarMaterialUtilizado(...)`, nunca direto na lista (lista é montada aos poucos, ao longo do tempo — diferente do `Pedido`, que recebia tudo pronto no construtor). `ValorTotalDeFabricacao()` é um método calculado, somando `ValorPorUnidade() * QuantidadeUtilizada` de cada material da lista — nunca um valor fixo digitado manualmente.

## Fase 2 — Persistência (EF Core) — a fazer

Objetivo: substituir listas em memória por EF Core, reaproveitando os aprendizados do projeto anterior (construtores privados para materialização, `Include`/`ThenInclude` para relacionamentos).

## Fase 3 — Interface Blazor — a fazer

Objetivo: CRUD visual para filamentos, insumos e produtos, com Claude construindo a maior parte da camada visual (decisão explícita do usuário, para focar o aprendizado em backend).

## Fase 4 — Cálculo de custo de produção — a fazer

Objetivo: calcular o custo real de cada produto (material consumido + tempo + energia), com base nas remessas de filamento/insumo efetivamente usadas (FIFO).

## Fase 5 — Módulo de IA — a fazer

Objetivo: primeira funcionalidade de IA do sistema — assistente de resposta a avaliações de clientes (fluxo semi-manual: ler avaliação, IA sugere resposta, usuário revisa e posta manualmente na Shopee — sem automação direta na plataforma, por restrição de acesso à API oficial sem CNPJ/MEI). Sugestões preditivas (fase futura, fora do escopo do MVP).

## Por que documentar isso

Mesmo raciocínio do projeto da livraria: mostra decisão de arquitetura por trás do código, não só a solução de um problema pontual — e serve de guia rápido para explicar o projeto em entrevista técnica.
