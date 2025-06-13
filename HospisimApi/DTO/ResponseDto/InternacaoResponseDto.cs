using System;
using HospisimApi.DTO;
using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class InternacaoResponseDto
    {
        public Guid Id { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? PrevisaoAlta { get; set; }
        public string MotivoInternacao { get; set; }
        public string Leito { get; set; }
        public string Quarto { get; set; }
        public string Setor { get; set; }
        public string? PlanoSaudeUtilizado { get; set; }
        public string ObservacoesClinicas { get; set; }
        public StatusInternacaoEnum StatusInternacao { get; set; }

        public PacienteResumidoDto? Paciente { get; set; }
        public AtendimentoResumidoDto? Atendimento { get; set; }
    }

}