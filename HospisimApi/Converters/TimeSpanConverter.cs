using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HospisimApi.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        private const string TimeFormatForParse = @"hh\:mm";

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Esperado tipo string para TimeSpan, mas foi recebido {reader.TokenType}");
            }

            string? timeString = reader.GetString();
            if (string.IsNullOrEmpty(timeString))
            {
                throw new JsonException("String de hora vazia ou nula não é permitida.");
            }

            if (TimeSpan.TryParseExact(timeString, TimeFormatForParse, CultureInfo.InvariantCulture, out TimeSpan result))
            {
                return result;
            }
            if (TimeSpan.TryParseExact(timeString, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            throw new JsonException($"Não foi possível converter '{timeString}' para TimeSpan no formato '{TimeFormatForParse}'.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm", CultureInfo.InvariantCulture));
        }
    }
}