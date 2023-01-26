using MongoDB.Bson;
namespace Assignment_1.Models
{
    public class CreateImage
    {
         

            
        public ObjectId Id { get; set; }
        public IFormFile Image { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }


    }
    


}
