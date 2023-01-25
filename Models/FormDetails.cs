using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Assignment_1.Models
{
    public class FormDetails
    {
       
       
        [BsonId]
        [Required(ErrorMessage = "Name is required")]
        public string ?Name { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public string ?Country { get; set; }

     

    }
}
