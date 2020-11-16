using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class Order
    {
        [DisplayName("Mã đặt hàng")]
        [Key]        
        public int OrderId { get; set; }
        [DisplayName("Ngày đặt hàng")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [DisplayName("Trạng thái đơn hàng")]
        public string Status { get; set; }
        [DisplayName("Tổng cộng")]
        public double TotalPay { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }        
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}