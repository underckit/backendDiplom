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
        [HttpGet("FieldNameForUser/{Id}")]
        public IActionResult Get(int Id)
        {

            List<field> fields = new List<field>();
            // берутся поля связанные с пользователем из таблицы many to many
            List<user_to_field> userFields = db.user_to_field.Where(x => x.id_user == Id).ToList();
            if (userFields.Count <= 0) 
                return NotFound();
            // добавлеине полей в список  по сверяя по айди 
            foreach (var userFieldsd in userFields)
            {
                fields.Add(db.field.FirstOrDefault(x => x.id == userFieldsd.id_field));

            }
            
            return Ok(fields.Select(x => new {name = x.name, id = x.id , coordinates= x.coordinates}));

        }

        //вывод ndvi карты  и координат поля с датой фото
        [HttpGet("getNdviMap/{Id}")]
        public NdviToCoordinates GetMap(int Id, DateTime startdate, DateTime enddate)
        {
            NdviToCoordinates output = new NdviToCoordinates();
            output.coordinates = db.field.SingleOrDefault(x => x.id == Id).coordinates;

            output.startdate = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate).startdate;
            output.enddate = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.enddate == enddate).enddate;
            output.ndvimap = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate && x.enddate == enddate).ndvimap;

            return output;
        }

        [HttpPost("addField")]
        public void addField ([FromBody] CreateField createField)
        {
            //добавляем поле в бд
            db.field.Add(createField.field);
            var user_to_field = new user_to_field { } ;
            db.SaveChanges();
            //связываем юзера и пользователя
            user_to_field.id_user = createField.id_user;
            user_to_field.id_field = createField.field.id;
            db.user_to_field.Add(user_to_field);
            db.SaveChanges();

        }

        [HttpPost("addDateNDVI")]
        public void addDate(int id_field, DateTime startdate, DateTime enddate)
        {
            //добавляем поле в бд
            ndvi Ndvi = new ndvi();
            
            Ndvi.startdate = startdate;
            Ndvi.enddate = enddate;

            var item = db.field.FirstOrDefault(x => x.id == id_field);
            foreach (var f in db.field)
            {
                if (f.id == id_field)
                {
                    Ndvi.id_field = id_field;
                    db.ndvi.Add(Ndvi);
                    
                    
                }

               
            }


            db.SaveChanges();



        }

        [HttpPut("updateField/{id}")]
        public void updateField(int id , [FromBody] field updateField)
        {
            updateField.id = id;
            db.field.Update(updateField);
            db.SaveChanges();
        }


        [HttpDelete("deleteField/{id}")]
        public void deleteField(int id)
        {
            var item = db.field.FirstOrDefault(x => x.id == id);

      
            foreach (var f in db.user_to_field)
            {
                if (f.id_field == id)
                {
                    db.user_to_field.Remove(f);
                }
            }

           

            if (item != null)
            {
               
                db.field.FirstOrDefault(x => x.id == id).deleted = true;
                db.SaveChanges();
            }


        }




       
    }
}
