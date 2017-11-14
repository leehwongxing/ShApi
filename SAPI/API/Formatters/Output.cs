using Jil;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace API.Formatters
{
    public class Output : OutputFormatter
    {
        public Output()
        {
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;

            using (var Writer = new StreamWriter(response.Body, Encoding.UTF8, 10240))
            {
                Writer.WriteAsync(JSON.Serialize(context.Object));
            }

            return Task.FromResult(response);
        }
    }
}