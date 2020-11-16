using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [DisplayName("Loại sản phẩm")]
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}