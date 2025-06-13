using System;

namespace HospisimApi.DTO.ResponseDto
{
    public class AltaHospitalarResponseDto
    {
        public Guid InternacaoId { get; set; }
        public DateTime DataAlta { get; set; }
        public string CondicaoPaciente { get; set; }
        public string InstrucoesPosAlta { get; set; }

        public InternacaoResumidoDto? Internacao { get; set; }
    }
}