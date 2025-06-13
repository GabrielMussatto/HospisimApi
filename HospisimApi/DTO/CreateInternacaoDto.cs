using System;
using System.ComponentModel.DataAnnotations;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class CreateInternacaoDto
    {
        [Required(ErrorMessage = "O ID do paciente é obrigatório.")]
        public Guid PacienteId { get; set; }

        [Required(ErrorMessage = "O ID do atendimento que originou a internação é obrigatório.")]
        public Guid AtendimentoId { get; set; }

        [Required(ErrorMessage = "A data de entrada é obrigatória.")]
        public DateTime DataEntrada { get; set; }

        public DateTime? PrevisaoAlta { get; set; }

        [Required(ErrorMessage = "O motivo da internação é obrigatório.")]
        [StringLength(200, ErrorMessage = "O motivo da internação não pode exceder 200 caracteres.")]
        public string MotivoInternacao { get; set; }

        [Required(ErrorMessage = "O leito é obrigatório.")]
        [StringLength(50, ErrorMessage = "O leito não pode exceder 50 caracteres.")]
        public string Leito { get; set; }

        [Required(ErrorMessage = "O número ou código do quarto é obrigatório.")]
        [StringLength(50, ErrorMessage = "O quarto não pode exceder 50 caracteres.")]
        public string Quarto { get; set; }

        [Required(ErrorMessage = "O setor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O setor não pode exceder 100 caracteres.")]
        public string Setor { get; set; }

        [StringLength(100, ErrorMessage = "O plano de saúde utilizado não pode exceder 100 caracteres.")]
        public string? PlanoSaudeUtilizado { get; set; }

        [StringLength(1000, ErrorMessage = "As observações clínicas não podem exceder 1000 caracteres.")]
        public string ObservacoesClinicas { get; set; }

        [Required(ErrorMessage = "O status da internação é obrigatório.")]
        public StatusInternacaoEnum StatusInternacao { get; set; }
    }
}