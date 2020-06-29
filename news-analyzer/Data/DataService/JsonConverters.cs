using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Data.DataService.Enums;

namespace Data.DataService
{
    public struct JsonConverters
    {
        public class DateConverter : JsonConverter<DateTime>
        {
            public override DateTime ReadJson(
                JsonReader reader, 
                Type objectType, 
                DateTime existingValue, 
                bool hasExistingValue, 
                JsonSerializer serializer) =>
                DateTime.ParseExact(
                    reader.Value?.ToString() ?? throw new TypeLoadException(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            public override void WriteJson(
                JsonWriter writer,
                DateTime dateTimeValue,
                JsonSerializer serializer) =>
                writer.WriteValue(dateTimeValue.ToString(
                    "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }
        
        public class TagsConverter : JsonConverter<ICollection<string>>
        {
            public override void WriteJson(JsonWriter writer, ICollection<string> value, JsonSerializer serializer) =>
                writer.WriteValue(string.Join(',', value));

            public override ICollection<string> ReadJson(JsonReader reader, Type objectType, ICollection<string> existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var tagsString = reader.Value?.ToString();
                var tags = tagsString?.Split(',');
                return tags;
            }
        }
        
        public class SiteConverter : JsonConverter<Site>
        {
            public override void WriteJson(JsonWriter writer, Site value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override Site ReadJson(JsonReader reader, Type objectType, Site existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var siteString = reader.Value?.ToString();
                return siteString switch
                {
                    "DennikN" => Site.DennikN,
                    "HlavneSpravy" => Site.HlavneSpravy,
                    "SME" => Site.SME,
                    "Plus7Dni" => Site.Plus7Dni,
                    "ZemAVek" => Site.ZemAVek,
                    _ => throw new NotImplementedException()
                };
            }
        }
        
        public class CategoryConverter : JsonConverter<Category>
        {
            public override void WriteJson(JsonWriter writer, Category value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override Category ReadJson(JsonReader reader, Type objectType, Category existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var siteString = reader.Value?.ToString();
                return siteString switch
                {
                    "HomeNews" => Category.HomeNews,
                    "ForeignNews" => Category.ForeignNews,
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}