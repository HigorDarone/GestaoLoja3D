# Sistema de Gestão — Loja de Impressão 3D

Sistema de gestão para uma loja real de impressão 3D (vendas ativas na Shopee), cobrindo controle de materiais/insumos, cálculo de custo de produção e um módulo de IA para apoio no atendimento e nas decisões do dia a dia da loja.

Diferente do projeto anterior (livraria, um exercício de aprendizado), esse projeto nasce de um problema de negócio real: hoje o controle de material, custo e prazo é feito manualmente, sem nenhum sistema de apoio.

## Objetivo

- Controlar remessas de filamento e insumos (o que foi comprado, quanto resta, quanto custou).
- Calcular o custo real de produção de cada peça (material + tempo + energia).
- Ter uma interface visual (Blazor) para uso no dia a dia, sem depender de planilhas.
- Incorporar IA como parte central do sistema, não só como recurso pontual — primeiro apoiando na resposta a avaliações de clientes, depois (fase futura) sugerindo decisões com base nos dados acumulados.

## Tecnologias

**Já em uso:**
- C# / .NET 10
- Blazor Web App (Interactive Server)
- Entity Framework Core (persistência — a definir provider)

**Planejadas:**
- Integração com IA (a definir provedor/API) para o módulo de apoio a avaliações
- xUnit para testes da camada de domínio

## Estrutura do projeto

- `GestaoLoja3D.Dominio` — Class Library com as regras de negócio (models, validações, cálculos), sem nenhuma dependência de UI.
- `GestaoLoja3D` (Blazor Web App) — interface visual, consome o `Dominio` via referência de projeto.

## Como rodar

*A completar conforme o projeto avança.*

## Roadmap

Ver `roadmap-projeto-loja3d.md` para o detalhamento fase a fase das decisões de arquitetura e do progresso.
