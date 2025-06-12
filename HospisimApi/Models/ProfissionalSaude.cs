using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;

namespace HospisimApi.Models
{
    public class ProfissionalSaude
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome completo do profissional é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome completo não pode exceder 200 caracteres.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos. Somente números")]
        public string CPF { get; set; }

        [NotMapped]
        public string CPFFormatado
        {
            get
            {
                string cleanCpf = new string(CPF?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                if (string.IsNullOrEmpty(cleanCpf) || cleanCpf.Length != 11) return CPF;
                return Convert.ToUInt64(cleanCpf).ToString(@"000\.000\.000\-00");
            }
        }

        [Required(ErrorMessage = "O e-mail institucional é obrigatório.")]
        [StringLength(100, ErrorMessage = "O e-mail não pode exceder 100 caracteres.")]
        [EmailAddress(ErrorMessage = "E-mail institucional inválido.")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres.")]
        public string Telefone { get; set; }

        [NotMapped]
        public string TelefoneFormatado
        {
            get
            {
                string cleanPhone = new string(Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                if (cleanPhone.Length == 11) return Convert.ToUInt64(cleanPhone).ToString(@"(00) 00000\-0000");
                else if (cleanPhone.Length == 10) return Convert.ToUInt64(cleanPhone).ToString(@"(00) 0000\-0000");
                return Telefone;
            }
        }

        [Required(ErrorMessage = "O registro do conselho é obrigatório.")]
        [StringLength(50, ErrorMessage = "O registro do conselho não pode exceder 50 caracteres.")]
        public string RegistroConselho { get; set; }

        [Required(ErrorMessage = "O tipo de registro é obrigatório (CRM, COREN, etc.).")]
        [StringLength(10, ErrorMessage = "O tipo de registro não pode exceder 10 caracteres.")]
        public string TipoRegistro { get; set; } // CRM, COREN, etc.

        [Required(ErrorMessage = "O ID da especialidade é obrigatório.")]
        public Guid EspecialidadeId { get; set; }
        [ForeignKey("EspecialidadeId")]
        public Especialidade? Especialidade { get; set; }

        [Required(ErrorMessage = "A data de admissão é obrigatória.")]
        public DateTime DataAdmissao { get; set; }

        [Required(ErrorMessage = "A carga horária semanal é obrigatória.")]
        [Range(1, 168, ErrorMessage = "A carga horária deve estar entre 1 e 168 horas.")] // Máx 24*7 = 168
        public int CargaHorariaSemanal { get; set; }

        [Required(ErrorMessage = "O turno de trabalho é obrigatório.")]
        [StringLength(20, ErrorMessage = "O turno não pode exceder 20 caracteres.")]
        public string Turno { get; set; }

        public bool Ativo { get; set; }

        public ICollection<Atendimento>? Atendimentos { get; set; }
        public ICollection<Prescricao>? Prescricoes { get; set; }
    }
}