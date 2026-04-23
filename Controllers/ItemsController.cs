using Microsoft.AspNetCore.Mvc;
using TestProject.Services;
using TestProject.Models;

namespace TestProject.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ItemsController : ControllerBase
    {

        private readonly IItemService _itemService;
        // TODO: 
        // Add rate limiting
        // Add Authentication
        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;    
        }

        [HttpGet]
        public ActionResult<IEnumerable<Item>> Get([FromQuery] string? path)
        {
            try
            {
                var items = _itemService.GetItems(path ?? "");
                return Ok(items);
            }
            catch (DirectoryNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }   
}