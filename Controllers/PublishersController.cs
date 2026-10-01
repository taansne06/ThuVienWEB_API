using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThucHanhWEBAPI.Data;
using ThucHanhWEBAPI.Models.DTO;
using ThucHanhWEBAPI.Repositories;

namespace ThucHanhWEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisherRepository _publisherRepository;

        public PublishersController(AppDbContext dbContext, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-publisher")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAllPublisher()
        {
            var allPublishers = _publisherRepository.GetAllPublishers();
            return Ok(allPublishers);
        }

        [HttpGet("get-publisher-by-id/{id}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetPublisherById(int id)
        {
            var publisherWithId = _publisherRepository.GetPublisherById(id);
            if (publisherWithId == null)
            {
                return NotFound();
            }
            return Ok(publisherWithId);
        }

        [HttpPost("add-publisher")]
        [Authorize(Roles = "Write")]
        public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherAdd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
            return Ok(publisherAdd);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherDTO)
        {
            var publisherUpdate = _publisherRepository.UpdatePublisherById(id, publisherDTO);
            if (publisherUpdate == null)
            {
                return NotFound();
            }
            return Ok(publisherUpdate);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeletePublisherById(int id)
        {
            var publisherDelete = _publisherRepository.DeletePublisherById(id);
            if (publisherDelete == null)
            {
                return NotFound();
            }
            return Ok(publisherDelete);
        }

        [HttpGet("{id}/books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetBooksByPublisherId(int id)
        {
            var books = _publisherRepository.GetBooksByPublisherId(id);
            return Ok(books);
        }
    }
}