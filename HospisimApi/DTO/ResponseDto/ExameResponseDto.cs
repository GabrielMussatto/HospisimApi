using HospisimApi.DTO;
using System;

namespace HospisimApi.DTO
{
    public class ExameResponseDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataRealizacao { get; set; }
        public string? Resultado { get; set; }
        public AtendimentoResumidoDto? Atendimento { get; set; }
    }
}