using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class OrderDetail
    {
        [Key]
        [DisplayName("Mã chi tiết")]
        public int OrderDetailId { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [DisplayName("Số lượng")]
        [Range(1,100)]
        public int Quantity { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [DisplayName("Thành tiền")]
        public double SumPay { get; set; }

    }
}