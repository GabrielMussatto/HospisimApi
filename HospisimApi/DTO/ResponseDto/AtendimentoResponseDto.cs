using System;
using HospisimApi.Enums;

namespace HospisimApi.DTO.ResponseDto
{
    public class AtendimentoResponseDto
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public TipoAtendimentoEnum Tipo { get; set; }
        public StatusAtendimentoEnum Status { get; set; }
        public string Local { get; set; }

        public PacienteResumidoDto? Paciente { get; set; }
        public ProfissionalSaudeResumidoDto? ProfissionalSaude { get; set; }
        public ProntuarioResumidoDto? Prontuario { get; set; }
    }    
}