using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Database.Models;

namespace Database.Controllers
{
    public class CentresController : ApiController
    {
        private Database1Entities db = new Database1Entities();

        // GET: api/Centres
        public IQueryable<Centre> GetCentres()
        {
            return db.Centres;
        }

        // GET: api/Centres/5
        [ResponseType(typeof(Centre))]
        public IHttpActionResult GetCentre(string id)
        {
            List<Centre> list = new List<Centre>();
            foreach (Centre item in db.Centres)
            {
                if(item.name.Contains(id))
                {
                    list.Add(item);
                }
            }

            if(list.Count == 0)
            {
                return NotFound();
            }

            return Ok(list);
        }

        // PUT: api/Centres/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCentre(string id, Centre centre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != centre.name)
            {
                return BadRequest();
            }

            db.Entry(centre).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CentreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Centres
        [ResponseType(typeof(Centre))]
        public IHttpActionResult PostCentre(Centre centre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Centres.Add(centre);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CentreExists(centre.name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = centre.name }, centre);
        }

        // DELETE: api/Centres/5
        [ResponseType(typeof(Centre))]
        public IHttpActionResult DeleteCentre(string id)
        {
            Centre centre = db.Centres.Find(id);
            if (centre == null)
            {
                return NotFound();
            }

            db.Centres.Remove(centre);
            db.SaveChanges();

            return Ok(centre);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CentreExists(string id)
        {
            return db.Centres.Count(e => e.name == id) > 0;
        }
    }
}