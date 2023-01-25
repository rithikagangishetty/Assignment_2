using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Assignment_1.Models
{
    public class CreateImage
    {
         

            
        public ObjectId Id { get; set; }
        public IFormFile Image { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }

        
    }
    


}
