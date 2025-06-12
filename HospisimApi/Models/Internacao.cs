using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HospisimApi.Enums;

namespace HospisimApi.Models
{
    public class Internacao
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O ID do paciente é obrigatório.")]
        public Guid PacienteId { get; set; }
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }

        // Relacionamento 1:1 com Atendimento
        [Required(ErrorMessage = "O ID do atendimento que originou a internação é obrigatório.")]
        public Guid AtendimentoId { get; set; }
        [ForeignKey("AtendimentoId")]
        public Atendimento? Atendimento { get; set; }

        [Required(ErrorMessage = "A data de entrada da internação é obrigatória.")]
        public DateTime DataEntrada { get; set; }

        public DateTime? PrevisaoAlta { get; set; } // Data prevista para alta (pode ser nula)

        [Required(ErrorMessage = "O motivo da internação é obrigatório.")]
        [StringLength(200, ErrorMessage = "O motivo da internação não pode exceder 200 caracteres.")]
        public string MotivoInternacao { get; set; }

        [Required(ErrorMessage = "O leito é obrigatório.")]
        [StringLength(50, ErrorMessage = "O leito não pode exceder 50 caracteres.")]
        public string Leito { get; set; }

        [Required(ErrorMessage = "O número ou código do quarto é obrigatório.")]
        [StringLength(50, ErrorMessage = "O quarto não pode exceder 50 caracteres.")]
        public string Quarto { get; set; }

        [Required(ErrorMessage = "O setor é obrigatório (UTI, Clínica Geral, etc.).")]
        [StringLength(100, ErrorMessage = "O setor não pode exceder 100 caracteres.")]
        public string Setor { get; set; } // UTI, Clínica Geral, Pediatria etc.

        [StringLength(100, ErrorMessage = "O plano de saúde utilizado não pode exceder 100 caracteres.")]
        public string? PlanoSaudeUtilizado { get; set; } // Convênio utilizado (se houver, pode ser nulo)

        [StringLength(1000, ErrorMessage = "As observações clínicas não podem exceder 1000 caracteres.")]
        public string ObservacoesClinicas { get; set; }

        [Required(ErrorMessage = "O status da internação é obrigatório.")]
        public StatusInternacaoEnum StatusInternacao { get; set; }

        // Relacionamento 0..1:1 com AltaHospitalar
        public AltaHospitalar? AltaHospitalar { get; set; }
    }
}