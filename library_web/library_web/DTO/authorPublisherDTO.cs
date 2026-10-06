using System.ComponentModel.DataAnnotations;

namespace library_web.Models.DTO
{
    public class addAuthorDTO
    {
        [Required]
        public string FullName { get; set; }
    }

    public class authorNoIdDTO
    {
        public string FullName { get; set; }
    }

    public class addPublisherDTO
    {
        [Required]
        public string Name { get; set; }
    }
}