using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HospisimApi.Models
{
    public class Prontuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O número do prontuário é obrigatório.")]
        [StringLength(50, ErrorMessage = "O número do prontuário não pode exceder 50 caracteres.")]
        public string NumeroProntuario { get; set; }

        [Required(ErrorMessage = "A data de abertura do prontuário é obrigatória.")]
        public DateTime DataAbertura { get; set; }

        [StringLength(1000, ErrorMessage = "As observações gerais não podem exceder 1000 caracteres.")]
        public string ObservacoesGerais { get; set; }

        [Required(ErrorMessage = "O ID do paciente é obrigatório.")]
        public Guid PacienteId { get; set; }
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }

        public ICollection<Atendimento>? Atendimentos { get; set; }
    }
}