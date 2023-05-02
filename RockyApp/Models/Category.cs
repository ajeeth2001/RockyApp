using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockyApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage ="Name can't be blank !!")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,int.MaxValue,ErrorMessage ="Invalid Value i.e Value should be greate than zero")]
        [Required(ErrorMessage = "Order can't be blank !!")]
        public int DisplayOrder { get; set; }

    }
}
