using ThucHanhWEBAPI.Models.Domain;

namespace ThucHanhWEBAPI.Repositories
{
    public interface IImageRepository
    {
        Image Upload(Image image);
        List<Image> GetAllInfoImages();
        (byte[], string, string) DownloadFile(int Id);
    }
}