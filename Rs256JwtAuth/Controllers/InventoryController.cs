using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rs256JwtAuth.Models;
using Rs256JwtAuth.Repositories;

namespace Rs256JwtAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryRepository _repo;
        public InventoryController(InventoryRepository repo) => _repo = repo;


        [HttpGet]
        [Authorize]
        public IActionResult GetAll() => Ok(_repo.GetAll());


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] InventoryItem item)
        {
            _repo.Add(item);
            return Ok(item);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Remove(int id)
        {
            if (_repo.Remove(id)) return Ok();
            return NotFound();
        }
    }
}
