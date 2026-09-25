using ThucHanhWEBAPI.Models.Domain;

using ThucHanhWEBAPI.Models.DTO;


namespace ThucHanhWEBAPI.Repositories
{
    public interface IAuthorRepository
    {
        List<AuthorDTO> GellAllAuthors();
        AuthorNoIdDTO GetAuthorById(int id);
        AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO);
        AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO);
        Author? DeleteAuthorById(int id);

        List<BookWithAuthorAndPublisherDTO> GetBooksByAuthorId(int authorId);
    }
}