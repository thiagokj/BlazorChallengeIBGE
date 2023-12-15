# Blazor Challenge IBGE

Olá Dev! 😎

Esse projeto faz parte do desafio proposto no [Discord][DiscordBalta] do balta.io. Data final: 20/12/23.

**Dev Team - Grupo 26:**
[Cláudio Gabriel][ClaudioGabriel],
[Gabriel Tavares][GabrielTavares] e
[Thiago Cajaíba][ThiagoCajaiba].

## Definições

Nosso foco é a entrega de um projeto nível **Júnior**, bem estruturado e funcional.

Codificação realizada em inglês, com comentários em português. Definida linguagem Ubiquá para termos específicos.

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
Objetivo: Entregar um App funcionando, com uma boa arquitetura, bem organizado e com código limpo.

Funcionalidades Adicionais
Importação de Dados: Criar uma página para importar os dados deste Excel:
[INSERTS - API de localidades IBGE.xlsx][PlanilhaIBGE]
Neste caso, o App virá sem dados, e os mesmos serão carregados via endpoint, mediante upload do Excel

## Novo Projeto

Comando para criar o projeto blazor com suporte ao **Identity**, com a solução de login completa:

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
  - `Models`
  - `wwwroot`

Resumo:

- **Components/Pages** -> Páginas e componentes do projeto.
- **Data** -> Contexto do banco de dados, refletindo na aplicação a estrutura de campos e tabelas.
- **Models** -> As Entidades e Modelos dos objetos são organizados aqui.
- **Pages** -> Páginas padrão do app com a rota inicial.
- **wwwroot** -> Arquivos estáticos como scripts, css e imagens.

### Passos para Desenvolvimento

1. **Modelagem** -> Organização das informações sobre as Localidades como Entidades.
2. **Mapeamento** -> Mapeamento e migrações das Entidades no banco de dados.
3. **Componentes / Páginas** -> Telas para interação do usuário com a aplicação.
4. **Navegação** -> Configuração das rotas para acesso as páginas.
5. **Filtros e Paginação** -> Utilização de componentes como o [Quickgrid][Quickgrid] para organizar os dados.

## 01 - Modelagem

1. Com o Identity já configurado, crie as entidades para representar as Localidades do IBGE.

   ```csharp
   namespace BlazorChallengeIBGE.Models
   {
     public class Locality
     {
       public Guid Id { get; set; } = Guid.NewGuid(); // Identificador Único Global
       public string IbgeCode { get; set; } = null!; // Código do IBGE formado por 7 dígitos
       public string State { get; set; } = null!; // Sigla do Estado, representando a Unidade Federativa
       public string City { get; set; } = null!; // Nome da Cidade / Município
     }
   }
   ```

2. Agora, adicione os **Data Annotations**, especificando os requisitos para cada Propriedade.

```csharp
using System.ComponentModel.DataAnnotations;

   namespace BlazorChallengeIBGE.Models
   {
     public class Locality
     {
       [Key] // Chave primária
       public Guid Id { get; set; } = Guid.NewGuid();

       // Campo obrigatório com tamanho definido para 7 dígitos, conforme IBGE
       [Required(ErrorMessage = "Informe o código IBGE")]
       [StringLength(7, MinimumLength = 7, ErrorMessage = "O código deve conter 7 dígitos")]
       [RegularExpression(@"^\d+$")] // Permite apenas dígitos
       public string IbgeCode { get; set; } = null!;

       // Campo obrigatório com tamanho definido para 2 caracteres, sendo a sigla da UF
       [Required(ErrorMessage = "Informe a sigla do Estado")]
       [StringLength(2, MinimumLength = 2, ErrorMessage = "A sigla deve conter 2 caracteres")]
       [RegularExpression(@"^[a-zA-Z]*$")] // Somente letras
       public string State { get; set; } = null!;

       // Campo obrigatório com tamanho definido entre 3 a 100 caracteres
       [Required(ErrorMessage = "Informe a Cidade")]
       [MinLength(3, ErrorMessage = "A cidade deve ter pelo menos 3 caracteres")]
       [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
       [RegularExpression(@"^[a-zA-ZÀ-ÿ ]*$")] // Somente letras, espaço em branco e acentuação pt-BR
       public string City { get; set; } = null!;
     }
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

        // Configura o nome da tabela no banco de dados
        modelBuilder.Entity<Locality>().ToTable("Ibge");

        // Configura um índice único para Estado e Cidade, evitando duplicidade
        modelBuilder.Entity<Locality>()
          .HasIndex(l => new { l.State, l.City })
          .IsUnique();
    }

    public DbSet<Locality> Localities { get; set; } = null!;
}
```

1. Utilizando o Terminal, crie uma migração para refletir a Entidade no Banco. Em seguida, atualize os dados.

```csharp
dotnet ef migrations add "v1 - Schema IBGE"

dotnet ef database update
```

## 03 - Componentes da Localidade

Crie um CRUD para permitir a manipulação dos dados pelo usuário. Siga a estrutura **Components -> Localities**, criando as paginas dentro da pasta.

### Create.razor

Página para criar uma nova localidade.

```csharp
@page "/localities/create" // Rota para pagina
@inject ApplicationDbContext Context // Injeta o contexto com o banco
@inject NavigationManager Navigation // Injeta a navegação entre as rotas
@rendermode InteractiveServer // Renderiza no lado do servidor

<h1>Nova Cidade</h1>

// Formulário usando a tag do blazor EditForm, permitindo o bind (vínculo) entre o form e a Localidade
<EditForm Model="@Model" OnValidSubmit="OnValidSubmitAsync" FormName="localities-create">

  // Validações aplicadas com base na definição dos Data Annotations
  <DataAnnotationsValidator />
  <ValidationSummary />

  <div class="mb-3">
    <label class="form-label">Estado</label>
    <InputText @bind-Value="Model.State" class="form-control" />
    <ValidationMessage For="@(() => Model.State)" />
  </div>

  <div class="mb-3">
    <label class="form-label">Cidade</label>
    <InputText @bind-Value="Model.City" class="form-control" />
    <ValidationMessage For="@(() => Model.City)" />
  </div>

  <div class="mb-3">
    <label class="form-label">Código IBGE</label>

    // O código do IBGE foi modelado como string, facilitando a validação da quantidade de digitos.
    <InputText @bind-Value="Model.IbgeCode" class="form-control" />
    <ValidationMessage For="@(() => Model.IbgeCode)" />
  </div>

  <button type="submit" class="btn btn-primary">
    Criar
  </button>
  <a href="/categories">Cancelar</a>
</EditForm>

@code {
  // Cria um objeto do tipo Localidade chamado Model, permitindo o bind com o formulário
  public Locality Model { get; set; } = new();

  // Método chamado ao clicar no botão Criar, criando uma novo objeto do tipo Localidade no banco de dados
  public async Task OnValidSubmitAsync()
  {
    await Context.Localities.AddAsync(Model);
    await Context.SaveChangesAsync();

    // Após salvar os dados, redireciona a pagina para rota abaixo
    Navigation.NavigateTo("localities");
  }
}
```

### Details.razor

Página para exibir os detalhes de uma localidade. Aqui não é necessária validação dos campos.

```csharp
@page "/localities/{id:guid}" // Validação. O Id na rota deve ser do tipo Guid
@inject ApplicationDbContext Context
@rendermode InteractiveServer

// Se o Id informado não existir, exibe a mensagem abaixo
@if (Model is null)
{
  <p class="text-center">
    <em>Cidade não encontrada</em>
  </p>
}
else
{
  // Caso exista a localidade, exibe o form com detalhes
  <h1>Detalhes | @Model.City</h1>

  // Especifique o nome de cada form usando o FormName
  <EditForm Model="@Model" FormName="localities-details">

    <div class="mb-3">
      <label class="form-label">Estado</label>

      // readonly restringe alterações, sendo somente leitura
      <InputText @bind-Value="Model.State" class="form-control" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label">Cidade</label>
      <InputText @bind-Value="Model.City" class="form-control" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label">Código IBGE</label>
      <InputText @bind-Value="Model.IbgeCode" class="form-control" readonly />
    </div>

    <a href="/localities">Voltar</a>
  </EditForm>
}

@code {

  // Especifica que o id será recuperado na URL da página
  [Parameter]
  public Guid Id { get; set; }

  public Locality? Model { get; set; }

  // Ao iniciar a página, recupera a localidade conforme o Id informado
  protected override async Task OnInitializedAsync()
  {
    Model = await Context
        .Localities
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == Id);
  }
}
```

### Edit.razor

Página para editar os dados de uma localidade. Essa pagina é similar a Create.razor.

```csharp
@page "/localities/edit/{id:guid}"
@inject ApplicationDbContext Context
@inject NavigationManager Navigation
@rendermode InteractiveServer

<h1>Editar Cidade</h1>
<EditForm Model="@Model" OnValidSubmit="OnValidSubmitAsync" FormName="localities-edit">
  <DataAnnotationsValidator />
  <ValidationSummary />

  <div class="mb-3">
    <label class="form-label">Estado</label>
    <InputText @bind-Value="Model.State" class="form-control" />
    <ValidationMessage For="@(() => Model.State)" />
  </div>

  <div class="mb-3">
    <label class="form-label">Cidade</label>
    <InputText @bind-Value="Model.City" class="form-control" />
    <ValidationMessage For="@(() => Model.City)" />
  </div>

  <div class="mb-3">
    <label class="form-label">Código IBGE</label>
    <InputText @bind-Value="Model.IbgeCode" class="form-control" />
    <ValidationMessage For="@(() => Model.IbgeCode)" />
  </div>

  <button type="submit" class="btn btn-primary">
    Salvar
  </button>
  <a href="/localities">Cancelar</a>
</EditForm>

@code {

  [Parameter]
  public Guid Id { get; set; }

  public Locality Model { get; set; } = new();

  // Recupera a localidade conforme o Id informado. Caso não encontre o Id, cria uma nova localidade
  protected override async void OnInitialized()
  {
    Model = await Context
        .Localities
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == Id) ?? new();
  }

  // Ao clicar em salvar, atualiza os dados da localidade e redireciona o usuário para rota abaixo
  public async Task OnValidSubmitAsync()
  {
    Context.Localities.Update(Model);
    await Context.SaveChangesAsync();

    // Limpa o rastreamento do EF após salvar.
    Context.ChangeTracker.Clear();
    Navigation.NavigateTo("localities");
  }
}
```

### Delete.razor

Página para deletar uma localidade. A página de exclusão é similar as anteriores.

```csharp
@page "/localities/delete/{id:guid}"
@inject ApplicationDbContext Context
@inject NavigationManager Navigation
@rendermode InteractiveServer

@if (Model is null)
{
  <p class="text-center">
    <em>Cidade não encontrada</em>
  </p>
}
else
{
  <h1>Excluir Cidade</h1>
  <EditForm Model="@Model" OnValidSubmit="OnValidSubmit" FormName="localities-delete">

    <div class="mb-3">
      <label class="form-label">Código IBGE</label>
      <InputText @bind-Value="Model.IbgeCode" class="form-control" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label">Estado</label>
      <InputText @bind-Value="Model.State" class="form-control" readonly />
    </div>

    <div class="mb-3">
      <label class="form-label">Cidade</label>
      <InputText @bind-Value="Model.City" class="form-control" readonly />
    </div>

    <button type="submit" class="btn btn-danger">
      Excluir
    </button>
    <a href="/localities">Cancelar</a>
  </EditForm>
}

@code {

  [Parameter]
  public Guid Id { get; set; }

  public Locality? Model { get; set; }

  protected override async Task OnInitializedAsync()
  {
    Model = await Context
    .Localities
    .AsNoTracking()
    .FirstOrDefaultAsync(x => x.Id == Id);
  }

  // Ao clicar em excluir, apaga o registro do banco de dados e navega para a rota abaixo
  public async Task OnValidSubmit()
  {
    Context.Localities.Remove(Model);
    await Context.SaveChangesAsync();
    Navigation.NavigateTo("localities");
  }
}
```

### Index.razor

Página com a rota inicial para exibição de todas as localidades. Filtros e demais operações estão concentrados aqui.

```csharp
@page "/localities"

@inject ApplicationDbContext Context

// Renderização de streaming, melhorando a UX enquanto aguarda o carregamento dos dados
@attribute [StreamRendering(true)]

<h1>Cidades</h1>
<a href="/localities/create" class="btn btn-primary">Nova Cidade</a>
<br>

// Enquanto não houverem localidades, exibe a mensagem carregando...
@if (!Localities.Any())
{
  <p class="text-center">
    <em>Carregando as cidades...</em>
  </p>
}

// Quando os dados estiverem disponíveis, exibe em forma de tabela os dados da localidade
else
{
  <table class="table">
    <thead>
      <tr>
        <th>Código IBGE</th>
        <th>Estado</th>
        <th>Cidade</th>
        <th></th>
      </tr>
    </thead>
    <tbody>

      // Loop do blazor para listar cada item dentro da lista
      @foreach (var locality in Localities)
      {
        <tr>
          // Cria hiperlink no Código do IBGE, permitindo o acesso a pagina Details usando o Id
          <td>
            <a href="/localities/@locality.Id">
              @locality.IbgeCode
            </a>
          </td>

          // Recupera a UF do Estado
          <td>
            @locality.State
          </td>

          // Recupera a Cidade
          <td>
            @locality.City
          </td>

          <td>
            // Link em forma de botão, permitindo editar a localidade
            <a href="/localities/edit/@locality.Id" class="btn btn-primary">
              Editar
            </a>

            // Espaço em branco
            &nbsp;&nbsp;

            // Link em forma de botão, permitindo excluir a localidade
            <a href="/localities/delete/@locality.Id" class="btn btn-danger">
              Excluir
            </a>
          </td>
        </tr>
      }
    </tbody>
  </table>
}

@code {

  // Inicializa uma lista de localidades vazia
  public IEnumerable<Locality> Localities { get; set; } = Enumerable.Empty<Locality>();

  // Ao carregar a página, preenche a lista com todas as localidades registradas no banco
  protected override async Task OnInitializedAsync()
  {
    Localities = await Context
        .Localities
        .AsNoTracking()
        .ToListAsync();
  }
}
```

## 04 - Navegação no Menu

Atualize Components -> Layout -> NavMenu.razor com a rota para localidade

```csharp
@implements IDisposable

@inject NavigationManager NavigationManager

<div class="top-row ps-3 navbar navbar-dark">
  <div class="container-fluid">
    <a class="navbar-brand" href="">BlazorChallengeIBGE</a>
  </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
  <nav class="flex-column">
    <div class="nav-item px-3">
      <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
        <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
      </NavLink>
    </div>

    // Nova rota definida para acesso
    <div class="nav-item px-3">
      <NavLink class="nav-link" href="localities">
        <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> IBGE
      </NavLink>
    </div>

    <div class="nav-item px-3">
      <NavLink class="nav-link" href="auth">
        <span class="bi bi-lock-nav-menu" aria-hidden="true"></span> Auth Required
      </NavLink>
    </div>

    <AuthorizeView>
      <Authorized>
        <div class="nav-item px-3">
          <NavLink class="nav-link" href="Account/Manage">
            <span class="bi bi-person-fill-nav-menu" aria-hidden="true"></span> @context.User.Identity?.Name
          </NavLink>
        </div>
        <div class="nav-item px-3">
          <form action="Account/Logout" method="post">
            <AntiforgeryToken />
            <input type="hidden" name="ReturnUrl" value="@currentUrl" />
            <button type="submit" class="nav-link">
              <span class="bi bi-arrow-bar-left-nav-menu" aria-hidden="true"></span> Logout
            </button>
          </form>
        </div>
      </Authorized>
      <NotAuthorized>
        <div class="nav-item px-3">
          <NavLink class="nav-link" href="Account/Register">
            <span class="bi bi-person-nav-menu" aria-hidden="true"></span> Register
          </NavLink>
        </div>
        <div class="nav-item px-3">
          <NavLink class="nav-link" href="Account/Login">
            <span class="bi bi-person-badge-nav-menu" aria-hidden="true"></span> Login
          </NavLink>
        </div>
      </NotAuthorized>
    </AuthorizeView>
  </nav>
</div>

@code {
  private string? currentUrl;

  protected override void OnInitialized()
  {
    currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
    NavigationManager.LocationChanged += OnLocationChanged;
  }

  private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
  {
    currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
    StateHasChanged();
  }

  public void Dispose()
  {
    NavigationManager.LocationChanged -= OnLocationChanged;
  }
}
```

## 05 - Paginação e Filtros

Para facilitar a análise dos dados pelo usuário, será utilizado o componente Quickgrid. Instalação:

```csharp
dotnet add package Microsoft.AspNetCore.Components.QuickGrid
```

<!-- Links -->

[ClaudioGabriel]: https://github.com/Claudio-0x4347
[GabrielTavares]: https://github.com/gabrielctavares
[ThiagoCajaiba]: https://github.com/thiagokj/
[DiscordBalta]: https://discord.gg/nnbPDR9d
[PlanilhaIBGE]: https://github.com/andrebaltieri/ibge/blob/main/SQL%20INSERTS%20-%20API%20de%20localidades%20IBGE.xlsx
[Quickgrid]: https://aspnet.github.io/quickgridsamples/
