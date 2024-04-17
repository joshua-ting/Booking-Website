using Microsoft.AspNetCore.Mvc;
using apiclass;
using RestSharp;
using Newtonsoft.Json;

namespace WebClient.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest request = new RestRequest("api/bookings");
            RestResponse response = client.Get(request);
            List<Booking> list = new List<Booking>();
            if(response.IsSuccessStatusCode)
            {
                list = JsonConvert.DeserializeObject<List<Booking>>(response.Content);
            }
            return View(list);
        }


    }
}
