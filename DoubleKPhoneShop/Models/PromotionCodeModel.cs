using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class PromotionCodeModel
    {
        [Key]
        public int CodeID { get; set; }
        [DisplayName("Mã khuyến mãi")]
        public string Code { get; set; }
        [DisplayName("Hạn sử dụng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UseDate { get; set; }
        [DisplayName("Lượng Giảm")]        
        public double Value { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}