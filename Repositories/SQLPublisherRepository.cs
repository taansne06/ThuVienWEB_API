using Microsoft.EntityFrameworkCore;
using ThucHanhWEBAPI.Data;
using ThucHanhWEBAPI.Models.Domain;
using ThucHanhWEBAPI.Models.DTO;

namespace ThucHanhWEBAPI.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PublisherDTO> GetAllPublishers()
        {
            var allPublishersDomain = _dbContext.Publishers.ToList();

            var allPublisherDTO = new List<PublisherDTO>();
            foreach (var publisherDomain in allPublishersDomain)
            {
                allPublisherDTO.Add(new PublisherDTO()
                {
                    Id = publisherDomain.Id,
                    Name = publisherDomain.Name
                });
            }

            return allPublisherDTO;
        }

        public PublisherNoIdDTO GetPublisherById(int id)
        {
            var publisherWithIdDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == id);
            if (publisherWithIdDomain != null)
            {
                var publisherNoIdDTO = new PublisherNoIdDTO
                {
                    Name = publisherWithIdDomain.Name,
                };
                return publisherNoIdDTO;
            }
            return null;
        }

        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherDomainModel = new Publisher
            {
                Name = addPublisherRequestDTO.Name,
            };

            _dbContext.Publishers.Add(publisherDomainModel);
            _dbContext.SaveChanges();

            return addPublisherRequestDTO;
        }

        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                publisherDomain.Name = publisherNoIdDTO.Name;
                _dbContext.SaveChanges();
            }

            return publisherNoIdDTO;
        }

        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
            if (publisherDomain != null)
            {
                _dbContext.Publishers.Remove(publisherDomain);
                _dbContext.SaveChanges();
                return publisherDomain;
            }

            return null;
        }

        public List<BookWithAuthorAndPublisherDTO> GetBooksByPublisherId(int publisherId)
        {
            var books = _dbContext.Books
                .Where(b => b.PublisherID == publisherId)
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