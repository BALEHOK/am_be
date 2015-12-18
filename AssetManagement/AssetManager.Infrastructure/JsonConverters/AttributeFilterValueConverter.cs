using System;
using AssetManager.Infrastructure.Models.Search;
using Newtonsoft.Json;

namespace AssetManager.Infrastructure.JsonConverters
{
    public class AttributeFilterValueConverter : JsonConverter
    {
        private readonly Type _supportedType = typeof(IdNamePair<string, string>);

        public override bool CanConvert(Type objectType)
        {
            return objectType == _supportedType;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var result = new IdNamePair<string, string>();

            // need to get 2 properties: id and name
            var propertiesRead = 0;
            bool readTillEndOfArray = reader.TokenType == JsonToken.StartArray;

            while (propertiesRead < 2 && reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (ParseProperty(reader, result))
                {
                    propertiesRead++;
                }
            }

            if (!readTillEndOfArray)
            {
                ReadTillEndOfObject(reader);
            }
            else
            {
                ReadTillEndOfArray(reader);
            }

            return result;
        }

        private static bool ParseProperty(JsonReader reader, IdNamePair<string, string> obj)
        {
            if (reader.TokenType != JsonToken.PropertyName)
            {
                return false;
            }

            switch (reader.Value.ToString().ToLower())
            {
                case "id":
                    obj.Id = reader.ReadAsString();
                    return true;
                case "name":
                    obj.Name = reader.ReadAsString();
                    return true;
            }

            return false;
        }

        private void ReadTillEndOfObject(JsonReader reader)
        {
            while (reader.TokenType != JsonToken.EndObject && reader.Read())
            {
            }
        }

        private void ReadTillEndOfArray(JsonReader reader)
        {
            while (reader.TokenType != JsonToken.EndArray && reader.Read())
            {
            }
        }
    }
}