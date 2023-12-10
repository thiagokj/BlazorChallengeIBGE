using System.ComponentModel.DataAnnotations;

namespace BlazorChallengeIBGE.Models
{
    public class Locality
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a sigla do Estado")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A sigla deve conter 2 caracteres")]
        [RegularExpression(@"^[a-zA-Z]*$")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "Informe a Cidade")]
        [MinLength(3, ErrorMessage = "A cidade deve ter pelo menos 3 caracteres")]
        [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z ]*$")]
        public string City { get; set; } = null!;
    }
}
