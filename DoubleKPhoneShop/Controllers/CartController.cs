using DoubleKPhoneShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DoubleKPhoneShop.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string CartSession = "CartSession";
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }            
            return View(list);
        }

        public ActionResult AddItem(int id, int quantity)
        {
            Product product = db.Products.Find(id);  // tim sp theo sanPhamID
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.ProductId == id))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ProductId == id)
                        {
                            item.Quantity+=quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới giỏ hàng nếu chưa có
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới giỏ hàng nếu chưa có
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }

        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ProductId == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ProductId == item.Product.ProductId);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        [Authorize]
        public ActionResult Payment()
        {
            //var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            ViewBag.UserId = new SelectList(db.Users.Where(u=>u.Email == User.Identity.Name), "Id", "FullName");
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Payment([Bind(Include = "OrderId,OrderDate,UserId")] Order order)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];            
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                order.OrderDate = DateTime.Now;
                order.Status = "Paid";
                order.UserId = user.Id;
                db.Orders.Add(order);
                db.SaveChanges();
                foreach(var item in sessionCart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;                    
                    orderDetail.ProductId = item.Product.ProductId;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.SumPay = item.ThanhTien;
                    db.OrderDetails.Add(orderDetail);
                    Product qua = db.Products.Where(q => q.ProductId == item.Product.ProductId).FirstOrDefault();
                    qua.Quantity = qua.Quantity - item.Quantity;
                    db.Entry(qua).State = EntityState.Modified;
                    db.SaveChanges();                       
                }
                
                order.TotalPay = order.OrderDetails.Sum(s => s.SumPay);                
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                Session[CartSession] = null;
                var cartcustomer = db.Orders.Count(c => c.ApplicationUser.Email == User.Identity.Name);
                Session["cartcustomer"] = cartcustomer;
                //return View("/hoan-thanh");
                return View("Success");
            }
            return View(order);
        }

        public ActionResult CustomerCart()
        {
            var order = db.Orders.Include(o => o.OrderDetails.Select(a => a.Product)).Where(o => o.ApplicationUser.Email == User.Identity.Name);          
            return View(order);
        }
        
        public ActionResult Success()
        {
            return View();
        }

        //public ActionResult Index()
        //{
        //    List<CartItem> giohang = Session["giohang"] as List<CartItem>;
        //    return View(giohang);
        //}
        //public RedirectToRouteResult AddItem(int id)
        //{
        //    if (Session["giohang"] == null) // Nếu giỏ hàng chưa được khởi tạo
        //    {
        //        Session["giohang"] = new List<CartItem>();  // Khởi tạo Session["giohang"] là 1 List<CartItem>
        //    }

        //    List<CartItem> giohang = Session["giohang"] as List<CartItem>;  // Gán qua biến giohang dễ code

        //    // Kiểm tra xem sản phẩm khách đang chọn đã có trong giỏ hàng chưa

        //    if (giohang.FirstOrDefault(m => m.Product.ProductId == id) == null) // ko co sp nay trong gio hang
        //    {
        //        Product sp = db.Products.Find(id);  // tim sp theo sanPhamID

        //        CartItem newItem = new CartItem()
        //        {
        //            Product = sp,
        //            Quantity = 1
        //        };  // Tạo ra 1 CartItem mới                
        //        giohang.Add(newItem);  // Thêm CartItem vào giỏ 
        //    }
        //    else
        //    {
        //        // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
        //        CartItem cardItem = giohang.FirstOrDefault(m => m.Product.ProductId == id);
        //        cardItem.Quantity++;
        //    }
        //    return RedirectToAction("Index", "Cart", new { id = id });
        //}
    }
}