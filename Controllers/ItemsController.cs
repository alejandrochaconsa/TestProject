using Microsoft.AspNetCore.Mvc;
using TestProject.Services;
using TestProject.Models;

namespace TestProject.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ItemsController : ControllerBase
    {

        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string path)
        {
            try
            {
                Stream fileStream = _itemService.DownloadFile(path);
                return File(fileStream, "application/octet-stream", Path.GetFileName(path));
            }
            catch (FileNotFoundException)
            {
                return NotFound();

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }

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
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("search")]
        public ActionResult<DirectoryListing> SearchItems([FromQuery] string? path, [FromQuery] string query)
        { 
            try
            {
                DirectoryListing directoryListing = _itemService.SearchItems(path, query);
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
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }

        }
    }   
}