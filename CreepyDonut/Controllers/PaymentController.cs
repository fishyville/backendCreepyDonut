using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CreepyDonut.Models;
using System;
using CreepyDonut.DTO;

namespace CreepyDonut.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("get-snap-token")]
        public async Task<IActionResult> GetSnapToken([FromBody] PaymentRequest request)
        {
            string serverKey = _configuration["Midtrans:ServerKey"];
            string snapUrl = _configuration["Midtrans:SnapBaseUrl"];
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
                        gross_amount = request.GrossAmount
                    },
                    customer_details = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        email = request.Email,
                        phone = request.Phone
                    },
                    enabled_payments = new[] { "gopay", "credit_card" },
                    credit_card = new
                    {
                        secure = true
                    }
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(snapUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                return Content(result, "application/json");
            }
        }
    }
}