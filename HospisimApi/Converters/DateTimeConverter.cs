using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HospisimApi.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd/MM/yyyy";

        private static CultureInfo BrazilianCulture = new CultureInfo("pt-BR");

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Esperado tipo string para DateTime, mas foi recebido {reader.TokenType}.");
            }

            string? dateString = reader.GetString();

            if (string.IsNullOrEmpty(dateString))
            {
                throw new JsonException("String de data vazia ou nula não é permitida.");
            }

            if (DateTime.TryParseExact(dateString, DateFormat, BrazilianCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            throw new JsonException($"Não foi possível converter '{dateString}' para DateTime no formato '{DateFormat}'.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, BrazilianCulture));
        }
    }
}