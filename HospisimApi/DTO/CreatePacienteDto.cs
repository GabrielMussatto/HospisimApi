using System;
using System.ComponentModel.DataAnnotations;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class CreatePacienteDto
    {
        [Required(ErrorMessage = "O nome completo do paciente é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome completo não pode exceder 200 caracteres.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos. Somente números.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        public SexoEnum Sexo { get; set; }

        [Required(ErrorMessage = "O tipo sanguíneo é obrigatório.")]
        public TipoSanguineoEnum TipoSanguineo { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres.")]
        public string Telefone { get; set; }

        [StringLength(100, ErrorMessage = "O e-mail não pode exceder 100 caracteres.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "O endereço completo não pode exceder 300 caracteres.")]
        public string EnderecoCompleto { get; set; }

        [StringLength(20, ErrorMessage = "O número do cartão SUS não pode exceder 20 caracteres.")]
        public string NumeroCartaoSUS { get; set; }

        [Required(ErrorMessage = "O estado civil é obrigatório.")]
        public EstadoCivilEnum EstadoCivil { get; set; }

        public bool PossuiPlanoSaude { get; set; }
    }
}