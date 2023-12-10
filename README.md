# Blazor Challenge IBGE

Olá Dev! 😎

Esse projeto faz parte do desafio proposto no [Discord][DiscordBalta] do balta.io.

**Dev Team - Grupo 26:** Cláudio Gabriel, Gabriel Tavares e Thiago Cajaíba.

## Descrição do desafio

### Funcionalidades Base

Todos os projetos, independente do nível, precisaram entregar as seguintes funcionalidades:

- Autenticação usando Identity
- CRUD de Localidade (Código, Estado, Cidade -- Id, State, City)
- Pesquisa por código (IBGE)
- Pesquisa por estado
- Pesquisa por cidade

### Classificação das Equipes

As equipes serão classificadas em Júnior, Pleno e Sênior, sendo esta classificação dada pelo integrante da equipe mais experiente.

Por exemplo, se sua equipe tem três pessoas, duas júniores e uma sênior, ela será classificada como sênior.

De acordo com a classificação da sua equipe, você deverá seguir as entregas abaixo:

#### Júnior

Todas as funcionalidades base
.NET 8
Arquitetura: N/A
Objetivo: Entregar um App funcionando!

#### Pleno

Todas as funcionalidades base
.NET 8
Arquitetura: Aberto
Objetivo: Entregar um App funcionando, com uma boa arquitetura, bem organizado e com código limpo

#### Sênior

Todas as funcionalidades base .NET 8
Arquitetura: Clean Arch + MVVM
Objetivo: Entregar um App funcionando, com uma boa arquitetura, bem organizado e com código limpo
Funcionalidades Adicionais
Importação de Dados: Criar uma página para importar os dados deste Excel:
[INSERTS - API de localidades IBGE.xlsx][PlanilhaIBGE]
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

1. Com o Identity já configurado, vamos criar as entidades para representar as localidades do IBGE.

   ```csharp
   namespace BlazorChallengeIBGE.Models;

   public class Locality(int id, string state, string city)
   {
       public int Id = id;
       public string State = state;
       public string City = city;
   }
   ```

2. Agora, vamos adicionar os **Data Annotations**, especificando os requisitos para cada Propriedade.

   ```csharp
   using System.ComponentModel.DataAnnotations;

   namespace BlazorChallengeIBGE.Models;

   public class Locality(int id, string state, string city)
   {
       [Key] // Chave primária
       public int Id = id;

       // Campo obrigatório com tamanho definido para 2 caracteres.
       // Somente caracteres de A até Z.
       [Required(ErrorMessage = "Informe a sigla do Estado")]
       [StringLength(2, MinimumLength = 2, ErrorMessage = "A sigla deve conter 2 caracteres")]
       [RegularExpression(@"^[a-zA-Z]*$")]
       public string State { get; set; } = null!;

       // Campo obrigatório, com valores entre 3 a 100 caracteres.
       // Somente caracteres A-Z e espaços em branco
       [Required(ErrorMessage = "Informe a Cidade")]
       [MinLength(3, ErrorMessage = "A cidade deve ter pelo menos 3 caracteres")]
       [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
       [RegularExpression(@"^[a-zA-Z ]*$")]
       public string City { get; set; } = null!;
   }
   ```

## 02 - Mapeamento para o banco de dados

1. Com a modelagem definida, é hora de mapear a Entidade no Banco.

```csharp
using BlazorChallengeIBGE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorChallengeIBGE.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mantém as configurações padrão do Identity
        base.OnModelCreating(modelBuilder);

        // Configura o nome da tabela
        modelBuilder.Entity<Locality>().ToTable("Ibge");

        // Configura um índice único para Estado e Cidade, evitando duplicidade
        modelBuilder.Entity<Locality>()
          .HasIndex(l => new { l.State, l.City })
          .IsUnique();
    }

    public DbSet<Locality> Localities { get; set; } = null!;
}
```

1. Utilizando o Terminal, crie uma migração para refletir a Entidade no Banco e atualize os dados.

```csharp
dotnet ef migrations add "v1 - Schema IBGE"

dotnet ef database update
```

[DiscordBalta]: https://discord.gg/nnbPDR9d
[PlanilhaIBGE]: https://github.com/andrebaltieri/ibge/blob/main/SQL%20INSERTS%20-%20API%20de%20localidades%20IBGE.xlsx
