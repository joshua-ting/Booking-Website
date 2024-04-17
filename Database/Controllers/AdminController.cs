using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using apiclass;

namespace Database.Controllers
{
    public class AdminController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody] Admin admin)
        {
            if(admin.User_name.Equals("admin"))
            {
                if(admin.Password.Equals("adminpass"))
                {
                    return Ok("Login successfull");
                }
            }
            return Unauthorized();
        }
    }
}
