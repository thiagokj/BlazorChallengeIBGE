using System.ComponentModel.DataAnnotations;

namespace BlazorChallengeIBGE.Models
{
  public class Locality
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Informe o código IBGE")]
    [StringLength(7, MinimumLength = 7, ErrorMessage = "O código deve conter 7 digitos")]
    [RegularExpression(@"^\d+$")] // Somente dígitos
    public string IbgeCode { get; set; } = null!;

    [Required(ErrorMessage = "Informe a sigla do Estado")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A sigla deve conter 2 caracteres")]
    [RegularExpression(@"^[a-zA-Z]*$")] // Somente letras
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "Informe a Cidade")]
    [MinLength(3, ErrorMessage = "A cidade deve ter pelo menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ ]*$")] // Somente letras e acentuação pt-BR
    public string City { get; set; } = null!;
  }
}