using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;

namespace SunshinePinPairServices.Controller
{
    [Route("pair")]
    public class pairController : ControllerBase
    {
        [HttpGet("{addr}/{usr}/{pwd}/{pin}")]
        public async Task<string> IndexAsync(string addr, string usr,string pwd,string pin)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            var authToken = Encoding.ASCII.GetBytes($"{usr}:{pwd}");
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            PinObject pinObject = new PinObject();
            pinObject.pin = pin;
             var resp = await client.PostAsync($"https://{addr}:47990/api/pin", new StringContent(JsonConvert.SerializeObject(pinObject)));
            return  await resp.Content.ReadAsStringAsync();
        }
        public class PinObject
        {
            public string pin { get; set; }
        }
    }
}
