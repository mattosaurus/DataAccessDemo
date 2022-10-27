using Newtonsoft.Json;

namespace DataAccessDemo.Shared.Helpers
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateOnly.Parse((string)reader.Value!);
        }

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
