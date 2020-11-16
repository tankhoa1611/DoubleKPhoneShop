using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class ViewModel
    {
        //public Category Category { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        //public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}