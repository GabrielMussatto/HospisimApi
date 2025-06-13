using System;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class PacienteResponseDto
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; }
        public string CPFFormatado { get; set; }
        public DateTime DataNascimento { get; set; }
        public SexoEnum Sexo { get; set; }
        public TipoSanguineoEnum TipoSanguineo { get; set; }
        public string TelefoneFormatado { get; set; }
        public string Email { get; set; }
        public string EnderecoCompleto { get; set; }
        public string NumeroCartaoSUS { get; set; }
        public EstadoCivilEnum EstadoCivil { get; set; }
        public bool PossuiPlanoSaude { get; set; }
    }
}