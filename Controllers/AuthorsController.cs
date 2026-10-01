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
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        [HttpGet("get-all-author")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAllAuthor([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var allAuthors = _authorRepository.GellAllAuthors(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(allAuthors);
        }

        [HttpGet("get-author-by-id/{id}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            return Ok(authorWithId);
        }

        [HttpPost("add-author")]
        [Authorize(Roles = "Write")]
        public IActionResult AddAuthors([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok(authorAdd);
        }

        [HttpPut("update-author-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            return Ok(authorUpdate);
        }

        [HttpDelete("delete-author-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteAuthorById(int id)
        {
            var authorDelete = _authorRepository.DeleteAuthorById(id);
            return Ok(authorDelete);
        }

        [HttpGet("{id}/books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetBooksByAuthorId(int id)
        {
            var books = _authorRepository.GetBooksByAuthorId(id);
            return Ok(books);
        }
    }
}