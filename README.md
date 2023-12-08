# Blazor Challenge IBGE

Olá Dev! 😎

Esse projeto faz parte do desafio proposto no [Discord][DiscordBalta] do balta.io.

Dev Team: Cláudio Gabriel e Thiago Cajaíba - Grupo 26.

## Descrição do desafio

### Funcionalidades Base

Todos os projetos, independente do nível, precisaram entregar as seguintes funcionalidades:
Autenticação usando Identity
CRUD de Localidade (Código, Estado, Cidade -- Id, City, State)
Pesquisa por cidade
Pesquisa por estado
Pesquisa por código (IBGE)

### Classificação das Equipes

As equipes serão classificadas em Júnior, Pleno e Sênior, sendo esta classificação dada pelo integrante da equipe mais experiente.

Por exemplo, se sua equipe tem três pessoas, duas júniores e uma sênior, ela será classificada como sênior.

De acordo com a classificação da sua equipe, você deverá seguir as entregas abaixo:

Júnior
Todas as funcionalidades base
.NET 8
Arquitetura: N/A
Objetivo: Entregar um App funcionando!

Pleno
Todas as funcionalidades base
.NET 8
Arquitetura: Aberto
Objetivo: Entregar um App funcionando, com uma boa arquitetura, bem organizado e com código limpo

Sênior
Todas as funcionalidades base
.NET 8
Arquitetura: Clean Arch + MVVM
Objetivo: Entregar um App funcionando, com uma boa arquitetura, bem organizado e com código limpo
Funcionalidades Adicionais
Importação de Dados: Criar uma página para importar os dados deste Excel:
https://github.com/andrebaltieri/ibge/blob/main/SQL INSERTS - API de localidades IBGE.xlsx
Neste caso, o App virá sem dados, e os mesmos serão carregados via endpoint, mediante upload do Excel

## Novo Projeto

Vamos iniciar criando um projeto blazor com suporte ao **Identity**, que fornece solução de login completa:

```csharp
dotnet new blazor -o BlazorChallengeIBGE -int Auto -au Individual

//  Parâmetros do comando:
//  -int Auto -> Adiciona opções de interatividade Server e WebAssembly, gerando 2 projetos na Solution.
//  -au -> Adiciona a autenticação do usuário via Identity.
```

**Arquitetura:** Desenvolvimento orientado a dados - Data Driven Design (DDD):

**Estrutura:**

- **BlazorChallengeIBGE**
  - `Components`
  - `Data`
  - `wwwroot`

Resumo:

- **Components** -> Páginas e componentes gerados no projeto. Aqui temos acesso as configurações do Identity.
- **Data** -> Contexto do banco de dados, refletindo na aplicação a estrutura de campos e tabelas.
- **Pages** -> Paginas e componentes para visualizar e interagir com os dados.
- **wwwroot** -> Arquivos estáticos como scripts, css e imagens.

## 01 - Modelagem

Com o Identity já configurado, vamos criar as entidades para representar as localidades do IBGE.

```csharp
namespace BlazorChallengeIBGE.Models;

public class Locality(int id, string city, string state)
{
    public int Id = id;
    public string City = city;
    public string State = state;
}
```

[DiscordBalta]: https://discord.gg/nnbPDR9d
