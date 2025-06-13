using System;
using System.ComponentModel.DataAnnotations;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class CreateAtendimentoDto
    {
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

        [Required(ErrorMessage = "O ID do profissional de saúde é obrigatório.")]
        public Guid ProfissionalSaudeId { get; set; }

        [Required(ErrorMessage = "O ID do prontuário é obrigatório.")]
        public Guid ProntuarioId { get; set; }
    }
}