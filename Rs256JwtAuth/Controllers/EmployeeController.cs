using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rs256JwtAuth.Models;
using Rs256JwtAuth.Repositories;

namespace Rs256JwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _repo;
        public EmployeeController(EmployeeRepository repo) => _repo = repo;


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_repo.GetAll());


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] Employee e)
        {
            _repo.Add(e);
            return Ok(e);
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody] Employee e)
        {
            if (_repo.Update(e)) return Ok(e);
            return NotFound();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (_repo.Delete(id)) return Ok();
            return NotFound();
        }
    }
}
