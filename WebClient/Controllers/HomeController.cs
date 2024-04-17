using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebClient.Models;
using RestSharp;
using Newtonsoft.Json;
using apiclass;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest request = new RestRequest("api/centres");
            RestResponse response = client.Get(request);
            List<Centre> list = JsonConvert.DeserializeObject<List<Centre>>(response.Content);
            return View(list);
        }

        [HttpGet]
        public IActionResult getCentres()
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest req = new RestRequest("api/centres");
            RestResponse response = client.Get(req);
            List<Centre> list = JsonConvert.DeserializeObject<List<Centre>>(response.Content);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult getBookings(string id)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest req = new RestRequest("api/bookings/" + id);
            RestResponse response = client.Get(req);
            if(!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            List<Booking> list = JsonConvert.DeserializeObject<List<Booking>>(response.Content);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult searchCentre(string id)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest req = new RestRequest("api/centres/"+id);
            RestResponse response = client.Get(req);
            List<Centre> list = JsonConvert.DeserializeObject<List<Centre>>(response.Content);
            if(list == null || id == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [HttpPost]
        public IActionResult makeBookings([FromBody] Booking booking)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest restRequest = new RestRequest("api/bookings/" + booking.Centre_name);
            RestResponse restResponse = client.Get(restRequest);
            List<Booking> list = JsonConvert.DeserializeObject<List<Booking>>(restResponse.Content);
            getAvailableDate(ref list);

            Boolean available = false;

            if (booking.Start_date.CompareTo(list[list.Count - 1].Start_date) >= 0)
            {
                available = true;
            }
            else if(booking.Start_date.CompareTo(DateTime.Now) <= 0)
            {
                return NotFound();
            }
            else
            {
                foreach (Booking item in list)
                {
                    if (booking.Start_date.CompareTo(item.Start_date) >= 0 && booking.End_date.CompareTo(item.End_date) <= 0)
                    {
                        available = true;
                    }
                }
            }
            Log("" + booking.Centre_name);
            Log("" + booking.User_name);

            if(available)
            {
                restRequest = new RestRequest("api/bookings");
                restRequest.AddJsonBody(booking);
                restResponse = client.Post(restRequest);
                if (restResponse.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        public IActionResult InsertCentre([FromBody] Centre centre)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest request = new RestRequest("api/centres");
            request.AddJsonBody(centre);
            RestResponse response = client.Post(request);
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult login([FromBody] Admin admin)
        {
            if(admin.User_name.Equals("admin"))
            {
                if(admin.Password.Equals("adminpass"))
                {
                    return Ok();
                }

            }
            return Unauthorized();
        }

        [HttpGet]
        public IActionResult getAvailableDates(string id)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest request = new RestRequest("api/bookings/" + id);
            RestResponse response = client.Get(request);
            List<Booking> list = new List<Booking>();
            if (!response.IsSuccessStatusCode)
            {
                Booking booking = new Booking();
                booking.Start_date = DateTime.Now;
                booking.End_date = DateTime.Now;
                booking.Centre_name = id;
                list.Add(booking);
                return Ok(list);
            }
            list = JsonConvert.DeserializeObject<List<Booking>>(response.Content);

            getAvailableDate(ref list);
            return Ok(list);
        }

        public void getAvailableDate(ref List<Booking> list)
        {
            List<Booking> newList = new List<Booking>();
            if (list.Count == 1)
            {
                Booking booking3 = new Booking();
                booking3.Start_date = DateTime.Now;
                booking3.End_date = list[0].Start_date.AddDays(-1);
                newList.Add(booking3);
            }
            else
            {
                for (int i = 1; i < list.Count; i++)
                {
                    Booking booking1 = new Booking();
                    Booking booking = list[i - 1];
                    booking1.Start_date = booking.End_date.AddDays(1);
                    booking1.End_date = list[i].Start_date.AddDays(-1);
                    booking1.Centre_name = list[i].Centre_name;
                    if (booking1.End_date.CompareTo(booking1.Start_date) < 0)
                    {
                        booking1.End_date = booking1.Start_date;
                    }
                    newList.Add(booking1);
                }
            }
            Booking booking2 = new Booking();
            booking2.Start_date = list[list.Count - 1].End_date.AddDays(1);
            booking2.End_date = default;
            newList.Add(booking2);
            list = newList;
        }

        public static void Log(string logString)
        {
            string line = ". " + logString;
            Console.WriteLine(line);


            using (StreamWriter w = System.IO.File.AppendText("C:\\Users\\20134206\\Downloads\\slog.txt")) { Log(line, w); }

            //using (StreamWriter w = File.AppendText("C:\\Users\\jting\\Desktop\\distributed\\slog.txt")) { Log(line, w); }

        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }
    }
}