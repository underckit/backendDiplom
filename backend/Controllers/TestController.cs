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
               fields.Add(db.field.FirstOrDefault(x => x.id == userFieldsd.id_field && x.deleted  == false));  
            }
            return Ok(fields.Select(x => new {name = x.name, id = x.id , coordinates= x.coordinates}));

        }

        //вывод ndvi карты  и координат поля с датой фото
        [HttpGet("getNdviMap/{Id}")]
        public IActionResult GetMap(int Id, DateTime startdate, DateTime enddate, string type)
        {
            NdviToCoordinates output = new NdviToCoordinates();
            try
            {
                output.coordinates = db.field.SingleOrDefault(x => x.id == Id).coordinates;
                output.startdate = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate
                && x.enddate == enddate && x.type == type).startdate;
                output.enddate = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate
                && x.enddate == enddate && x.type == type).enddate;
                output.ndvimap = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate
                && x.enddate == enddate && x.type == type).ndvimap;
                output.type = db.ndvi.SingleOrDefault(x => x.id_field == Id && x.startdate == startdate
                && x.enddate == enddate && x.type == type).type;

            }

            catch(Exception)
            {
                return NotFound();

            }

            return Ok(output);

        }

        //вывод всех contrast ndvi юзера
        [HttpGet("getAllContrastNdviMaps")]
        public IActionResult GetContrastNdvi(int Id)
        {
            List<ndvi> ndvis = db.ndvi.Where(x => x.id_field == Id && x.type == "contrast").ToList(); ;

            if (ndvis.Count <= 0)
                return NotFound();
            else
            {
                return Ok(ndvis.Select(x => new {
                    ndvimap = x.ndvimap,
                    startdate = x.startdate,
                    enddate = x.enddate
                }));
            }
        }

        //вывод всех contrast ndvi юзера
        [HttpGet("getAllColorNdviMaps")]
        public IActionResult GetColorNdvi(int Id)
        {
            List<ndvi> ndvis = db.ndvi.Where(x => x.id_field == Id && x.type == "color").ToList(); ;

            if (ndvis.Count <= 0)
                return NotFound();
            else 
            { 
                return Ok(ndvis.Select(x => new {
                    ndvimap = x.ndvimap,
                    startdate = x.startdate,
                    enddate = x.enddate
                }));
            }

        }

        //вывод всех contrast ndvi юзера с апсемплингом
        [HttpGet("getAllContrastUpNdviMaps")]
        public IActionResult GetContrastUpNdvi(int Id)
        {
            List<ndvi> ndvis = db.ndvi.Where(x => x.id_field == Id && x.type == "contrastUpsampling").ToList(); ;

            if (ndvis.Count <= 0)
                return NotFound();
            else
            {
                return Ok(ndvis.Select(x => new {
                    ndvimap = x.ndvimap,
                    startdate = x.startdate,
                    enddate = x.enddate
                }));
            }
        }

        //вывод всех color ndvi юзера с апсемплингом
        [HttpGet("getAllColorUpNdviMaps")]
        public IActionResult GetColorUpNdvi(int Id)
        {
            List<ndvi> ndvis = db.ndvi.Where(x => x.id_field == Id && x.type == "colorUpsampling").ToList(); ;

            if (ndvis.Count <= 0)
                return NotFound();
            else
            {
                return Ok(ndvis.Select(x => new {
                    ndvimap = x.ndvimap,
                    startdate = x.startdate,
                    enddate = x.enddate
                }));
            }
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


        [HttpPut("updateField/{id}")]  
        public void updateField(int id , [FromBody] field updateField)
        {
            updateField.id = id;
            if (updateField.coordinates == null)
                updateField.coordinates = db.field.SingleOrDefault(x => x.id == id).coordinates;
            if (updateField.name == null)
                updateField.name = db.field.SingleOrDefault(x => x.id == id).name;
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
