using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoubleKPhoneShop.Models
{
    public class Product
    {
        [Key]
        [DisplayName("Mã sản phẩm")]
        public int ProductId { get; set; }
        [DisplayName("Tên sản phẩm")]
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string ProductName { get; set; }
        [DisplayName("Miêu tả")]
        [StringLength(5000, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string Description { get; set; }
        [DisplayName("Hình ảnh")]
        public string Image { get; set; }
        [DisplayName("Giá sản phẩm")]
        [Range(1,10000000000)]
        public double Price { get; set; }
        [DisplayName("Số lượng")]
        [Range(0,1000)]
        public int Quantity { get; set; }
        [DisplayName("Màu sắc")]
        public string Color { get; set; }
        [DisplayName("Ngày tạo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        //them field
        [DisplayName("Màn hình")]
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string Screen { get; set; }
        [DisplayName("Hệ điều hành")]
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string OperatingSystem { get; set; }
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string Cameras { get; set; }
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string CPU { get; set; }
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string RAM { get; set; }
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        public string ROM { get; set; }
        [StringLength(50, ErrorMessage = "{0} phải ít nhất {2} và ít hơn {1} kí tự.", MinimumLength = 2)]
        [DisplayName("Pin")]
        public string Batery { get; set; }
    }
}