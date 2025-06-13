using System;
using HospisimApi.DTO.ResponseDto;
using HospisimApi.Enums;

namespace HospisimApi.DTO.ResponseDto
{
    public class PrescricaoResponseDto
    {
        public Guid Id { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string Dosagem { get; set; } = string.Empty;
        public string Frequencia { get; set; } = string.Empty;
        public string ViaAdministracao { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public StatusPrescricaoEnum StatusPrescricao { get; set; }
        public string? ReacoesAdversas { get; set; }

        public AtendimentoResumidoDto? Atendimento { get; set; }
        public ProfissionalSaudeResumidoDto? Profissional { get; set; }
    }

}