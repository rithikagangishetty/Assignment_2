using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Assignment_1.Models
{
    public class CreateImage
    {
         

            [Key]
            public int ID { get; set; }
          
            public byte[]?  Image { get; set; }
            public string? filename { get; set; }
        
    }
    


}
