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
        public ActionResult<DirectoryListing> Get([FromQuery] string? path)
        {
            try
            {
                DirectoryListing directoryListing = _itemService.GetItems(path ?? "");
                return Ok(directoryListing);
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