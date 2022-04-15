using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private ApplicationContext db;
        public TestController(ApplicationContext context)
        {
            db = context;
        } 
        // GET: api/<TestController>
       
       
         // вывод полей по айди юзера
         [HttpGet("FieldName/{Id}")]
         public IActionResult Get(int Id)
         {
           
            List<Field> fields = new List<Field>();
            List<User_to_field> userFields = db.User_to_field.Where(x => x.Id_user == Id).ToList();
            if (userFields.Count <= 0)
                return NotFound();
            foreach(var userFieldsd in userFields)
            {
                fields.Add(db.Field.FirstOrDefault(x => x.Id == userFieldsd.Id_field));
            }
             return Ok(fields.Select(x=>x.Name));
         }

        //вывод ndvi карты  и координат поля с датой фото
        [HttpGet("getNdviMap/{Id}")]
        public NdviToCoordinates GetMap(int Id, DateTime date)
        {
            NdviToCoordinates output = new NdviToCoordinates();
            output.Coordinates = db.Field.SingleOrDefault(x => x.Id == Id).Coordinates;
            
            output.Date = db.Ndvi.SingleOrDefault(x => x.Id_field == Id && x.Date == date).Date;

            output.Ndvimap = db.Ndvi.SingleOrDefault(x => x.Id_field == Id && x.Date == date).Ndvimap;

            return output ;
        }


        




        // GET api/<TestController>/5
        /* [HttpGet("{id}")]
         public string Get(int id)
         {
             return "value";
         }

         // POST api/<TestController>
         [HttpPost]
         public void Post([FromBody] string value)
         {
         }

         // PUT api/<TestController>/5
         [HttpPut("{id}")]
         public void Put(int id, [FromBody] string value)
         {
         }

         // DELETE api/<TestController>/5
         [HttpDelete("{id}")]
         public void Delete(int id)
         {
         }*/
    }
}
