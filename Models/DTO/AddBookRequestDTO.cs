using System.ComponentModel.DataAnnotations;

namespace ThucHanhWEBAPI.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required(ErrorMessage = "Title không được để trống")]
        [MinLength(10, ErrorMessage = "Title phải có ít nhất 10 ký tự")]
        [RegularExpression(@"^[\p{L}\p{N} ]+$", ErrorMessage = "Title không được chứa ký tự đặc biệt")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Description không được để trống")]
        [MinLength(50, ErrorMessage = "Descriptionphải có ít nhất 50 ký tự")]
        public string? Description { get; set; }

        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        //navigation Properties –
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}