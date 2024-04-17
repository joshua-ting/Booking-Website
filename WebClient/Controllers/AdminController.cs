using Microsoft.AspNetCore.Mvc;
using apiclass;
using RestSharp;

namespace WebClient.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertCentre([FromBody] Centre centre)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest request = new RestRequest("api/admin");
            request.AddJsonBody(centre);
            RestResponse response = client.Post(request);
            if(response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IActionResult DeleteCentre(string id)
        {
            RestClient client = new RestClient("http://localhost:56143/");
            RestRequest req = new RestRequest("api/centres/" + id);
            RestResponse response = client.Delete(req);
            if(!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
