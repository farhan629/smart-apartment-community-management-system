using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmenityBookingService.API.Converters;

/// <summary>
/// Converts TimeSpan values to and from string representation in JSON serialization/deserialization.
/// </summary>
/// <remarks>
/// Serializes TimeSpan to "hh:mm:ss" format and deserializes from the same string format.
/// </remarks>
public class TimeSpanToStringConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Reads and converts a TimeSpan from a JSON string representation.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader to read from.</param>
    /// <param name="typeToConvert">The type to convert (TimeSpan).</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <returns>A TimeSpan parsed from the JSON string.</returns>
    /// <exception cref="FormatException">Thrown when the string format is invalid for TimeSpan parsing.</exception>
    public override TimeSpan Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => TimeSpan.Parse(reader.GetString()!);

    /// <summary>
    /// Writes a TimeSpan value as a JSON string in "hh:mm:ss" format.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="value">The TimeSpan value to serialize.</param>
    /// <param name="options">The JSON serializer options.</param>
    public override void Write(
        Utf8JsonWriter writer,
        TimeSpan value,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
}
