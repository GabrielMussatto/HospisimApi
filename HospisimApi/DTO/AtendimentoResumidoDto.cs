using HospisimApi.Enums;

namespace HospisimApi.DTO
{
    public class AtendimentoResumidoDto
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public TipoAtendimentoEnum Tipo { get; set; }
    }
}
