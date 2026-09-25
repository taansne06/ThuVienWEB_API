using ThucHanhWEBAPI.Models.Domain;
using ThucHanhWEBAPI.Models.DTO;

namespace ThucHanhWEBAPI.Repositories
{
    public interface IPublisherRepository
    {
        List<PublisherDTO> GetAllPublishers();
        PublisherNoIdDTO GetPublisherById(int id);
        AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO);
        PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO);
        Publisher? DeletePublisherById(int id);

        List<BookWithAuthorAndPublisherDTO> GetBooksByPublisherId(int publisherId);
    }
}