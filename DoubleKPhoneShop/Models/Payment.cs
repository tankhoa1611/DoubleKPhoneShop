using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class Payment
    {
        [Key]
        [DisplayName("Mã hoá đơn")]
        public int PaymentId { get; set; }
        [DisplayName("Ngày thanh toán")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        [DisplayName("Tổng tiền")]
        public double TotalPay { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}