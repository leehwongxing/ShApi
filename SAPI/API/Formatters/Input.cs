using Jil;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace API.Formatters
{
    public class Input : InputFormatter
    {
        public Input()
        {
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var InputType = context.ModelType;
            var Request = context.HttpContext.Request;

            MediaTypeHeaderValue.TryParse(Request.ContentType, out MediaTypeHeaderValue RequestMediaType);

            using (var Reader = new StreamReader(Request.Body, Encoding.UTF8, true, 10240))
            {
                var Deserialized = JSON.Deserialize(Reader, InputType, Options.ISO8601IncludeInherited);/// REF: Nancy's Jil Serializer

                return InputFormatterResult.SuccessAsync(Deserialized);
            }
        }
    }
}