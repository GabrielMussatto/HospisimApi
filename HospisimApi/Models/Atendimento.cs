using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HospisimApi.Enums;
using System.Collections.Generic;
using System;

namespace HospisimApi.Models
{
    public class Atendimento
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A data do atendimento é obrigatória.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A hora do atendimento é obrigatória.")]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "O tipo de atendimento é obrigatório.")]
        public TipoAtendimentoEnum Tipo { get; set; }

        [Required(ErrorMessage = "O status do atendimento é obrigatório.")]
        public StatusAtendimentoEnum Status { get; set; }

        [StringLength(100, ErrorMessage = "O local do atendimento não pode exceder 100 caracteres.")]
        public string Local { get; set; }

        [Required(ErrorMessage = "O ID do paciente é obrigatório.")]
        public Guid PacienteId { get; set; }
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }

        [Required(ErrorMessage = "O ID do profissional de saúde é obrigatório.")]
        public Guid ProfissionalSaudeId { get; set; }
        [ForeignKey("ProfissionalSaudeId")]
        public ProfissionalSaude? ProfissionalSaude { get; set; }

        [Required(ErrorMessage = "O ID do prontuário é obrigatório.")]
        public Guid ProntuarioId { get; set; }
        [ForeignKey("ProntuarioId")]
        public Prontuario? Prontuario { get; set; }

        // Relacionamentos
        public ICollection<Prescricao>? Prescricoes { get; set; }
        public ICollection<Exame>? Exames { get; set; }

        // Relacionamento 0..1:1 com Internacao
        public Internacao? Internacao { get; set; }
    }
}