using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using ProtoBuf.Meta;

namespace Thinktecture.Web.Http.Formatters
{
    public class ProtoBufFormatter : BufferedMediaTypeFormatter
    {
        private static readonly RuntimeTypeModel model = TypeModel.Create();
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/x-protobuf");

        public ProtoBufFormatter()
        {
            model.UseImplicitZeroDefaults = false;

            SupportedMediaTypes.Add(mediaType);
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return mediaType; }
        }

        protected override bool CanReadType(Type type)
        {
            return CanReadTypeCore(type);
        }

        protected override bool CanWriteType(Type type)
        {
            return CanReadTypeCore(type);
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders,
                                                   FormatterContext formatterContext)
        {
            return model.Deserialize(stream, null, type); //Serializer.NonGeneric.Deserialize(type, stream);
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream,
                                                HttpContentHeaders contentHeaders, FormatterContext formatterContext,
                                                TransportContext transportContext)
        {
            model.Serialize(stream, value); //Serializer.Serialize(stream, value);
        }

        private static bool CanReadTypeCore(Type type)
        {
            if (type == typeof (IKeyValueModel))
            {
                return false;
            };

            return true;
        }
    }
}