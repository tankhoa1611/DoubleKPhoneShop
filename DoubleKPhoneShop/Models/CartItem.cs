using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    [Serializable]
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity;
        public double ThanhTien
        {
            get
            {
                return Quantity * Product.Price;
            }
        }
    }
}