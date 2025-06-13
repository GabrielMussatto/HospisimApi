namespace HospisimApi.DTO
{
    public class InternacaoResumidoDto
    {
        public Guid Id { get; set; }
        public string MotivoInternacao { get; set; }
        public string Leito { get; set; }
        public string Quarto { get; set; }
    }
}