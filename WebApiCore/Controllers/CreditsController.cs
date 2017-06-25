using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/credits")]
    [Authorize(Policy = "UserApi")]
    public class CreditsController : Controller
    {
        private readonly Database _database;
        public CreditsController(Database database)
        {
            _database = database;
        }

        [HttpGet]
        public async Task<IEnumerable<Credit>> Get()
        {
            return await _database
                .Credit
                .ToListAsync();
        }
                
        [HttpGet("{id}", Name = "Get")]
        public Credit Get(int id)
        {
            var _model = _database
                .Credit
                .Find(id);

            return _model;
        }
        
        [HttpPost]
        public ObjectResult Post([FromBody]Credit value)
        {
            _database.Credit.Add(value);
            if (_database.SaveChanges() > 0)
            {
                return Ok(new { Status = "Success", Model = value });
            }
            return new ObjectResult(new { Status = "Error", Model = value });
        }
                
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromBody]Credit value)
        {
            if (id == value.Id)
            {
                _database.Entry(value).State = EntityState.Modified;
                if (_database.SaveChanges() > 0)
                {
                    return Ok(new { Model = value, Status = "Success" });
                }
                else
                    return new ObjectResult(new { Status = "Error", Model = value });
            }      
            return new NotFoundObjectResult(value);
        }
                
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            Credit _model = _database.Credit.Find(id);
            if (_model == null) return new NotFoundObjectResult(id);
            _database.Credit.Remove(_model);
            if (_database.SaveChanges() > 0)
            {
                return Ok(new { Status = "Success", Id = id });
            }
            return new ObjectResult(new { Status = "Error", Id = id });
        }
    }
}
