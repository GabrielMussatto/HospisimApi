using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospisimApi.Models
{
    public class Exame
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O tipo de exame é obrigatório.")]
        [StringLength(100, ErrorMessage = "O tipo de exame não pode exceder 100 caracteres.")]
        public string Tipo { get; set; } // Sangue, Imagem, etc.

        [Required(ErrorMessage = "A data de solicitação do exame é obrigatória.")]
        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataRealizacao { get; set; } // Pode ser nula se ainda não foi realizado

        [StringLength(1000, ErrorMessage = "O resultado do exame não pode exceder 1000 caracteres.")]
        public string? Resultado { get; set; } // Pode ser nulo se ainda não há resultado

        [Required(ErrorMessage = "O ID do atendimento é obrigatório.")]
        public Guid AtendimentoId { get; set; }
        [ForeignKey("AtendimentoId")]
        public Atendimento? Atendimento { get; set; }
    }
}