using System;
using System.ComponentModel.DataAnnotations;

namespace HospisimApi.DTOs
{
    public class UpdateProfissionalSaudeDto
    {
        [Required(ErrorMessage = "O nome completo do profissional é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome completo não pode exceder 200 caracteres.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos. Somente números.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O e-mail institucional é obrigatório.")]
        [StringLength(100, ErrorMessage = "O e-mail não pode exceder 100 caracteres.")]
        [EmailAddress(ErrorMessage = "E-mail institucional inválido.")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode exceder 20 caracteres. Somente números.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O registro do conselho é obrigatório.")]
        [StringLength(50, ErrorMessage = "O registro do conselho não pode exceder 50 caracteres.")]
        public string RegistroConselho { get; set; }

        [Required(ErrorMessage = "O tipo de registro é obrigatório (CRM, COREN, etc.).")]
        [StringLength(10, ErrorMessage = "O tipo de registro não pode exceder 10 caracteres.")]
        public string TipoRegistro { get; set; }

        [Required(ErrorMessage = "O ID da especialidade é obrigatório.")]
        public Guid EspecialidadeId { get; set; }

        [Required(ErrorMessage = "A data de admissão é obrigatória.")]
        public DateTime DataAdmissao { get; set; }

        [Required(ErrorMessage = "A carga horária semanal é obrigatória.")]
        [Range(1, 168, ErrorMessage = "A carga horária deve estar entre 1 e 168 horas.")]
        public int CargaHorariaSemanal { get; set; }

        [Required(ErrorMessage = "O turno de trabalho é obrigatório.")]
        [StringLength(20, ErrorMessage = "O turno não pode exceder 20 caracteres.")]
        public string Turno { get; set; }

        public bool Ativo { get; set; }
    }
}