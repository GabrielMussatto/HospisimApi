using System;
using System.ComponentModel.DataAnnotations;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class UpdatePrescricaoDto
    {
        [Required(ErrorMessage = "O ID do atendimento é obrigatório.")]
        public Guid AtendimentoId { get; set; }

        [Required(ErrorMessage = "O ID do profissional que prescreveu é obrigatório.")]
        public Guid ProfissionalId { get; set; }

        [Required(ErrorMessage = "O nome do medicamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do medicamento não pode exceder 100 caracteres.")]
        public string Medicamento { get; set; }

        [Required(ErrorMessage = "A dosagem é obrigatória.")]
        [StringLength(50, ErrorMessage = "A dosagem não pode exceder 50 caracteres.")]
        public string Dosagem { get; set; }

        [Required(ErrorMessage = "A frequência de administração é obrigatória.")]
        [StringLength(50, ErrorMessage = "A frequência não pode exceder 50 caracteres.")]
        public string Frequencia { get; set; }

        [Required(ErrorMessage = "A via de administração é obrigatória.")]
        [StringLength(50, ErrorMessage = "A via de administração não pode exceder 50 caracteres.")]
        public string ViaAdministracao { get; set; }

        [Required(ErrorMessage = "A data de início do tratamento é obrigatória.")]
        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string Observacoes { get; set; }

        [Required(ErrorMessage = "O status da prescrição é obrigatório.")]
        public StatusPrescricaoEnum StatusPrescricao { get; set; }

        [StringLength(500, ErrorMessage = "As reações adversas não podem exceder 500 caracteres.")]
        public string? ReacoesAdversas { get; set; }
    }
}