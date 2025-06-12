using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospisimApi.Models
{
    public class AltaHospitalar
    {
        [Key]
        [Required(ErrorMessage = "O ID da internação é obrigatório para a alta hospitalar.")]
        public Guid InternacaoId { get; set; }

        [Required(ErrorMessage = "A data da alta é obrigatória.")]
        public DateTime DataAlta { get; set; }

        [Required(ErrorMessage = "A condição do paciente na alta é obrigatória.")]
        [StringLength(200, ErrorMessage = "A condição do paciente não pode exceder 200 caracteres.")]
        public string CondicaoPaciente { get; set; }

        [StringLength(1000, ErrorMessage = "As instruções pós-alta não podem exceder 1000 caracteres.")]
        public string InstrucoesPosAlta { get; set; }

        [Required]
        [ForeignKey("InternacaoId")]
        public Internacao? Internacao { get; set; }
    }
}