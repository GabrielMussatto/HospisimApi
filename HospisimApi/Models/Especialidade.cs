using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HospisimApi.Models
{
    public class Especialidade
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da especialidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da especialidade não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        public ICollection<ProfissionalSaude>? ProfissionaisSaude { get; set; }
    }
}