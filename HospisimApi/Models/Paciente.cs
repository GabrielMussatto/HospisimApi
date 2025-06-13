using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HospisimApi.Enums;
using System.Linq;
using System.Collections.Generic;

namespace HospisimApi.Models
{
    public class Paciente
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome completo do paciente é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome completo não pode exceder 200 caracteres.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos. Somente numeros.")]
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

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        public SexoEnum Sexo { get; set; }

        [Required(ErrorMessage = "O tipo sanguíneo é obrigatório.")]
        public TipoSanguineoEnum TipoSanguineo { get; set; }

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

        [StringLength(100, ErrorMessage = "O e-mail não pode exceder 100 caracteres.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [StringLength(300, ErrorMessage = "O endereço completo não pode exceder 300 caracteres.")]
        public string EnderecoCompleto { get; set; }

        [StringLength(20, ErrorMessage = "O número do cartão SUS não pode exceder 20 caracteres.")]
        public string NumeroCartaoSUS { get; set; }

        [Required(ErrorMessage = "O estado civil é obrigatório.")]
        public EstadoCivilEnum EstadoCivil { get; set; }

        public bool PossuiPlanoSaude { get; set; }

        // Relacionamentos
        public ICollection<Prontuario>? Prontuarios { get; set; }
        public ICollection<Internacao>? Internacoes { get; set; }
        public ICollection<Atendimento>? Atendimentos { get; set; }
    }
}