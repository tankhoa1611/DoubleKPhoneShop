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
        [DisplayName("Tình trạng thanh toán")]
        public bool Pay { get; set; }
        [DisplayName("Người nhận")]
        public string Receiver { get; set; }
        [DisplayName("Số Điện thoại người nhận")]
        public string PhoneReceive { get; set; }
        [DisplayName("Địa chỉ nhận hàng")]
        public string AddressReceive { get; set; }
        [DisplayName("Phương thức thanh toán")]
        public string PaymentMethod { get; set; }
        [DisplayName("Tổng cộng")]
        public double TotalPay { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }        
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}