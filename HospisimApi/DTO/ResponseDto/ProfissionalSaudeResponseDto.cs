using System;
using HospisimApi.Enums;

namespace HospisimApi.DTO.ResponseDto
{
    public class ProfissionalSaudeResponseDto
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; }

        public string CPFFormatado { get; set; }

        public string Email { get; set; }

        public string TelefoneFormatado { get; set; }

        public string RegistroConselho { get; set; }
        public string TipoRegistro { get; set; }

        public EspecialidadeResumidoDto? Especialidade { get; set; }

        public DateTime DataAdmissao { get; set; }
        public int CargaHorariaSemanal { get; set; }
        public string Turno { get; set; }
        public bool Ativo { get; set; }
    }
}