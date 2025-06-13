using System;
using System.ComponentModel.DataAnnotations;

namespace HospisimApi.DTOs
{
    public class UpdateEspecialidadeDto
    {
        [Required(ErrorMessage = "O nome da especialidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da especialidade não pode exceder 100 caracteres.")]
        public string Nome { get; set; }
    }
}