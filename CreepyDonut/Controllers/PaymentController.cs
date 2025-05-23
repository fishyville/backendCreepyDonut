using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CreepyDonut.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpPost("get-snap-token")]
        public async Task<IActionResult> GetSnapToken()
        {
            string serverKey = "SB-Mid-server-UKfeRACLdBo88p4AATKK7p6u"; // Ganti dengan server key kamu
            string encodedAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(serverKey + ":"));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(new
                {
                    transaction_details = new
                    {
                        order_id = $"order-{Guid.NewGuid()}",
                        gross_amount = 15000
                    },
                    customer_details = new
                    {
                        first_name = "NamaDepan",
                        last_name = "NamaBelakang",
                        email = "emailkamu@example.com",
                        phone = "08123456789"
                    },
                    enabled_payments = new[] { "gopay", "credit_card" },
                    credit_card = new
                    {
                        secure = true
                    }
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://app.sandbox.midtrans.com/snap/v1/transactions", content);
                var result = await response.Content.ReadAsStringAsync();

                return Content(result, "application/json");
            }
        }
    }
}
