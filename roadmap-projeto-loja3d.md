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

**Teste manual — concluído.** Projeto Console (`GestaoLoja3D.TesteManual`) com 10 cenários cobrindo caminho feliz e casos de erro de `Filamento`, `Insumo`, `ItemEstoque` (polimorfismo), `MaterialUtilizado` e `Produto`. Todos os cenários passaram, confirmando os cálculos e validações antes de avançar pra persistência.

## Fase 2 — Persistência (EF Core) — concluída (estrutura inicial)

Objetivo: substituir listas em memória por EF Core, reaproveitando os aprendizados do projeto anterior (construtores privados para materialização, `Include`/`ThenInclude` para relacionamentos).

- Construtores privados/protegidos sem parâmetros adicionados em `ItemEstoque` (protected, por ser abstrata), `Filamento`, `Insumo`, `Produto` e `MaterialUtilizado`.
- Pacotes instalados: `Pomelo.EntityFrameworkCore.MySql`, `Microsoft.EntityFrameworkCore.Tools` e `Microsoft.EntityFrameworkCore.Design` — este último precisou ser instalado tanto no `GestaoLoja3D.Dominio` quanto no projeto de inicialização (`GestaoLoja3D`, o Blazor), já que o EF Tools exige o pacote também no projeto de startup.
- `AppDbContext` criado em `GestaoLoja3D.Dominio/Data`, com `DbSet` para `Filamento`, `Insumo`, `Produto` e `MaterialUtilizado` (sem `DbSet<ItemEstoque>`, por ela ser abstrata).
- Corrigido um problema de referência de projeto ao configurar: a referência entre `GestaoLoja3D.Dominio` e o projeto Blazor `GestaoLoja3D` estava invertida (Dominio referenciando o Blazor, quando deveria ser o oposto), causando erro de "dependência circular" e falha ao rodar `Add-Migration`. Corrigido removendo a referência errada e adicionando a referência correta (Blazor → Dominio), mantendo o padrão de dependência de mão única já usado no projeto da livraria.
- Primeira migration (`InicialCreate`) criada e aplicada com sucesso. Confirmado no MySQL Workbench que `Filamento` e `Insumo` compartilham uma única tabela, com uma coluna `Discriminator` diferenciando os tipos — comportamento padrão do EF Core (Table-Per-Hierarchy) para classes que herdam de uma base comum (`ItemEstoque`).

## Fase 3 — Interface Blazor — em andamento

Objetivo: CRUD visual para filamentos, insumos e produtos. Decisão de divisão de trabalho revista no meio do caminho: inicialmente Claude construiria toda a camada visual, mas o usuário optou por escrever o `@code` (lógica C#) ele mesmo, com Claude só fornecendo o HTML/CSS puro e orientação Socrática — mantendo o aprendizado de backend mesmo dentro do Blazor.

- **Filamentos.razor e Insumos.razor — concluídas.** CRUD completo (criar remessa/compra + listar), usando `@bind` nos formulários, `@foreach` nas tabelas, e `try/catch` reaproveitando a validação já existente nas classes de domínio.
- **Produtos.razor — em andamento, funcional.** Criação de produto, listagem com materiais utilizados (usando o novo `ObterDescricao()` polimórfico), e formulário de "Adicionar material" com: seleção de Filamento/Insumo, seleção do item específico (filtrado por tipo, só itens com estoque disponível), quantidade usada, e um SELETOR DE UNIDADE (kg/g para Filamento; metro/cm ou só a própria unidade para Insumo, dependendo do que foi cadastrado) que converte a quantidade digitada pra unidade base antes de descontar do estoque — resolvendo o problema real de precisão (comprar em kg, consumir em gramas).
- Lições técnicas novas aprendidas nessa fase: ciclo de vida de componente Blazor (`OnInitializedAsync`), `@bind`/`@onchange`/lambdas em atributos Razor (e o conflito de aspas que isso pode causar — resolvido extraindo lógica pra métodos nomeados em vez de lambdas inline complexas), `Dictionary<TChave,TValor>` para guardar estado por item numa lista renderizada em `@foreach` (evitando "vazamento" de estado entre itens), `Cast<T>()` para tratar listas de tipos derivados como uma lista do tipo base, `virtual`/`override` aplicado a um método `ObterDescricao()` em `ItemEstoque`/`Filamento`/`Insumo`, e `TryParse`/`GetValueOrDefault` com valor padrão explícito (evitando o padrão perigoso de cair no `default` do tipo, como `0` para `decimal`).
- **Exclusão de material e de produto — concluída.** Decisão de negócio: excluir devolve a quantidade ao estoque do item, como se nunca tivesse sido usada. No domínio: `ItemEstoque.DevolverQuantidade()` (sem revalidar, pois a quantidade já foi validada ao consumir), `Produto.RemoverMaterialUtilizado(id)` e `Produto.RemoverTodosMateriais()`, que percorre uma cópia da lista (`ToList()`) para poder remover da lista real durante o `foreach`. No Razor: `RemoverMaterial` (botão X por material) e `RemoverProduto` (botão no cabeçalho do card).
- **Bug encontrado e corrigido: registros órfãos no banco.** Remover um `MaterialUtilizado` da coleção do `Produto` não apagava a linha: a relação é opcional (chave estrangeira aceita `NULL`), então o EF só zerava `ProdutoId`, e a tela parecia correta. Correção: antes de remover do domínio, guardar o(s) `MaterialUtilizado` e mandar o EF apagar explicitamente (`Remove` / `RemoveRange`). Lição: conferir o banco, não só a tela. Linhas órfãs que já existiam foram limpas manualmente (`DELETE ... WHERE ProdutoId IS NULL`).
- **Bug encontrado e corrigido: estado velho nos seletores.** Ao trocar de Filamento para Insumo, o item e o fator de unidade escolhidos antes continuavam guardados nos `Dictionary`, e o `Adicionar` consumia o item errado (um filamento) ou dividia pelo fator errado (ex.: 1000 de "g"). Correção: `SelecionarTipo` grava o tipo e chama `LimparSelecao`; `SelecionarItem` limpa só a unidade; os `<select>` leem os dictionaries via `value="@..."` para a tela refletir o que está guardado.
- Lições técnicas dessa etapa: `DbContext.Remove`/`RemoveRange`, comportamento do EF em relação opcional vs obrigatória, identidade de objetos rastreados (a lista de "disponível" acompanha o estoque porque são as mesmas instâncias que o `FindAsync` devolve), validação de `null` antes de `Remove`, extração de lógica repetida em método próprio (`LimparSelecao`).
- Melhoria futura anotada: recarregar `filamentosDisponiveis` e `insumosDisponiveis` após mudanças de estoque (hoje um item que zera continua no dropdown como "0 disponível", e um item restaurado de 0 só aparece ao recarregar a página). O `Adicionar` também não mostra mensagem quando falta item ou quantidade.
- **Próximo: CRUD de Filamento e Insumo (excluir/editar).** Regra de negócio em aberto: o que fazer ao excluir um item que já está em uso por algum produto (impedir, ou apagar mesmo assim).

## Fase 4 — Cálculo de custo de produção — a fazer

Objetivo: calcular o custo real de cada produto (material consumido + tempo + energia), com base nas remessas de filamento/insumo efetivamente usadas (FIFO).

## Fase 5 — Módulo de IA — a fazer

Objetivo: primeira funcionalidade de IA do sistema — assistente de resposta a avaliações de clientes (fluxo semi-manual: ler avaliação, IA sugere resposta, usuário revisa e posta manualmente na Shopee — sem automação direta na plataforma, por restrição de acesso à API oficial sem CNPJ/MEI). Sugestões preditivas (fase futura, fora do escopo do MVP).

## Por que documentar isso

Mesmo raciocínio do projeto da livraria: mostra decisão de arquitetura por trás do código, não só a solução de um problema pontual — e serve de guia rápido para explicar o projeto em entrevista técnica.
