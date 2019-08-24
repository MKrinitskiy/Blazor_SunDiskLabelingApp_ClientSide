using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Geometry
{
    public class SizeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Size));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Size size = (Size)value;
            JObject jo = new JObject();
            jo.Add("Width", size.Width);
            jo.Add("Height", size.Height);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            List<string> keys = jo.Properties().Select(p => p.Name).ToList();
            return new Size((int)jo["Width"], (int)jo["Height"]);
        }
    }


    public class SizeFJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(SizeF));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SizeF size = (SizeF)value;
            JObject jo = new JObject();
            jo.Add("Width", size.Width);
            jo.Add("Height", size.Height);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new SizeF((float)jo["Width"], (float)jo["Height"]);
        }
    }


    public class SizeDJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(SizeD));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SizeD size = (SizeD)value;
            JObject jo = new JObject();
            jo.Add("Width", size.Width);
            jo.Add("Height", size.Height);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new SizeD((double)jo["Width"], (double)jo["Height"]);
        }
    }
}
