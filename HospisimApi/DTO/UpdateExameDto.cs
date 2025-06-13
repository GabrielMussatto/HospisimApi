using System;
using System.ComponentModel.DataAnnotations;

namespace HospisimApi.DTO
{
    public class UpdateExameDto
    {
        [Required(ErrorMessage = "O tipo de exame é obrigatório.")]
        [StringLength(100, ErrorMessage = "O tipo de exame não pode exceder 100 caracteres.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "A data de solicitação do exame é obrigatória.")]
        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataRealizacao { get; set; }

        [StringLength(1000, ErrorMessage = "O resultado do exame não pode exceder 1000 caracteres.")]
        public string? Resultado { get; set; }

        [Required(ErrorMessage = "O ID do atendimento é obrigatório.")]
        public Guid AtendimentoId { get; set; }
    }
}