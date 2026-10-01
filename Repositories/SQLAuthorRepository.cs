using ThucHanhWEBAPI.Data;
using ThucHanhWEBAPI.Models.Domain;
using ThucHanhWEBAPI.Models.DTO;

namespace ThucHanhWEBAPI.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AuthorDTO> GellAllAuthors(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //Get Data From Database -Domain Model & map to DTO
            var allAuthors = _dbContext.Authors.Select(a => new AuthorDTO()
            {
                Id = a.Id,
                FullName = a.FullName
            }).AsQueryable();

            //filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("fullname", StringComparison.OrdinalIgnoreCase))
                {
                    allAuthors = allAuthors.Where(x => x.FullName.Contains(filterQuery));
                }
            }

            //sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("fullname", StringComparison.OrdinalIgnoreCase))
                {
                    allAuthors = isAscending ? allAuthors.OrderBy(x => x.FullName)
                                             : allAuthors.OrderByDescending(x => x.FullName);
                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return allAuthors.Skip(skipResults).Take(pageSize).ToList();
        }

        public AuthorNoIdDTO GetAuthorById(int id)
        {
            var authorWithIdDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == id);
            if (authorWithIdDomain == null)
            {
                return null;
            }

            var authorNoIdDTO = new AuthorNoIdDTO
            {
                FullName = authorWithIdDomain.FullName,
            };
            return authorNoIdDTO;
        }

        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomainModel = new Author
            {
                FullName = addAuthorRequestDTO.FullName,
            };

            _dbContext.Authors.Add(authorDomainModel);
            _dbContext.SaveChanges();
            return addAuthorRequestDTO;
        }

        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                authorDomain.FullName = authorNoIdDTO.FullName;
                _dbContext.SaveChanges();
            }
            return authorNoIdDTO;
        }

        public Author? DeleteAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);
            if (authorDomain != null)
            {
                _dbContext.Authors.Remove(authorDomain);
                _dbContext.SaveChanges();
                return authorDomain;
            }
            return null;
        }

        public List<BookWithAuthorAndPublisherDTO> GetBooksByAuthorId(int authorId)
        {
            var books = _dbContext.Books_Authors
                .Where(ba => ba.AuthorId == authorId)
                .Select(ba => ba.Book)
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead.Value : null,
                    Rate = book.IsRead ? book.Rate.Value : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                }).ToList();

            return books;
        }
    }
}