using DoubleKPhoneShop.Models;
using PayPal.Api;
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
        private const double tigia = 23000;
        public ActionResult Index()
        {
            var order = db.Orders.Include(o => o.OrderDetails.Select(a => a.Product)).Where(o => o.ApplicationUser.Email == User.Identity.Name).ToList();
            return View(order);            
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
                Session["cartcustomer"] = list.Sum(o => o.Quantity);
            }            
            return RedirectToAction("CustomerCart","Cart");
        }

        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            Session["cartcustomer"] = null;
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
            Session["cartcustomer"] = sessionCart.Sum(o => o.Quantity);
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
            Session["cartcustomer"] = sessionCart.Sum(o => o.Quantity);
            return Json(new
            {
                status = true
            });
        }       
        //httpget
        public ActionResult Payment(string paymentmethod, int? id,double? point)
        {
            var code = db.PromotionCodeModels.Find(id);
            if(point != null)
            {
                ViewBag.point = point;
            }
            if (code != null)
            {
                ViewBag.code = code.Code;
                ViewBag.codeid = code.CodeID;
                ViewBag.amount = code.Value;
            }      
            if (User.Identity.IsAuthenticated)
            {
                var u = db.Users.Where(p => p.Email == User.Identity.Name).First();                
                ViewBag.name = u.FullName;
                ViewBag.add = u.Address;
                ViewBag.phone = u.PhoneNumber;
            }
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            Session["cartcustomer"] = list.Sum(o => o.Quantity);
            return View(list);
        }
        [HttpPost]        
        public ActionResult Payment([Bind(Include = "OrderId,OrderDate,UserId")] Models.Order order, string receiver,string phonereceive, string addressreceive, string paymentmethod, int? codeid,double? point)
        {            
            if (ModelState.IsValid)
            {                
                var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                var code = db.PromotionCodeModels.Find(codeid);
                if (paymentmethod == "cash")
                {
                    var sessionCart = (List<CartItem>)Session[CartSession];
                    if (User.Identity.IsAuthenticated)
                    {
                        order.UserId = user.Id;
                        order.Receiver = user.FullName;
                        order.PhoneReceive = user.PhoneNumber;
                        order.AddressReceive = user.Address;                        
                    }
                    else
                    {
                        order.Receiver = receiver;
                        order.PhoneReceive = phonereceive;
                        order.AddressReceive = addressreceive;                        
                    }
                    order.Pay = false;
                    order.PaymentMethod = paymentmethod;
                    order.OrderDate = DateTime.Now;
                    order.Status = "notconfirm";
                    db.Orders.Add(order);
                    db.SaveChanges();
                    foreach (var item in sessionCart)
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
                    var sum =  order.OrderDetails.Sum(s => s.SumPay);
                    if (code != null)
                    {
                        order.TotalPay = sum - (sum * code.Value);
                    }
                    if (point != null)
                    {
                        order.TotalPay = sum - (double)point;
                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        order.TotalPay = order.OrderDetails.Sum(s => s.SumPay);
                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();
                    }             
                    if(User.Identity.IsAuthenticated)
                    {
                        user.Point = order.TotalPay * 0.05;
                    }
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session[CartSession] = null;                              
                    return View("Success");
                }
                else
                {
                    var sessionCart = (List<CartItem>)Session[CartSession];                          
                    var orderVM = new Models.Order()
                    {
                        OrderDate = DateTime.Now,                        
                        PaymentMethod = paymentmethod
                    };
                    return Checkout( orderVM,  sessionCart, codeid, point);
                }                              
            }
            return View(order);
        }     
        private Models.Order pay;
        private List<CartItem> cart;
        [Authorize]
        public ActionResult Checkout(Models.Order orderVM, List<CartItem> sessionCart, int? codeid, double? point)
        {
            pay = orderVM;
            cart = sessionCart;
            if (orderVM != null && sessionCart != null)
            {
                var user = db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
                var code = db.PromotionCodeModels.Find(codeid);
                orderVM.OrderDate = DateTime.Now;
                orderVM.Status = "wait";                
                orderVM.UserId = user.Id;
                orderVM.Pay = true;
                db.Orders.Add(orderVM);
                db.SaveChanges();
                foreach (var item in sessionCart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = orderVM.OrderId;
                    orderDetail.ProductId = item.Product.ProductId;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.SumPay = item.ThanhTien;
                    db.OrderDetails.Add(orderDetail);
                    Product qua = db.Products.Where(q => q.ProductId == item.Product.ProductId).FirstOrDefault();
                    qua.Quantity = qua.Quantity - item.Quantity;
                    db.Entry(qua).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                var sum = orderVM.OrderDetails.Sum(s => s.SumPay);
                if (code != null)
                {
                    orderVM.TotalPay = sum - (sum * code.Value);
                }
                if (point != null)
                {
                    orderVM.TotalPay = sum - (double)point;
                }
                else
                {
                    orderVM.TotalPay = orderVM.OrderDetails.Sum(s => s.SumPay);
                }
                db.Entry(orderVM).State = EntityState.Modified;
                db.SaveChanges();
                user.Point = orderVM.TotalPay * 0.05;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Session[CartSession] = null;
                var cartcustomer = db.Orders.Count(c => c.ApplicationUser.Email == User.Identity.Name);
                //Session["cartcustomer"] = cartcustomer;
                Session["cartcustomer"] = null;
                return PaymentWithPaypal();
            }
            return View(orderVM);
        }

        public ActionResult CustomerCart()
        {
            //var order = db.Orders.Include(o => o.OrderDetails.Select(a => a.Product)).Where(o => o.ApplicationUser.Email == User.Identity.Name);          
            //return View(order);
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;                
            }
            Session["cartcustomer"] = list.Sum(o => o.Quantity);
            return View(list);
        }

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult UserPromotionCode()
        {
            var u = db.Users.Where(p => p.Email == User.Identity.Name).Include(c=>c.promotionCodeModels).First();            
            return View(u);
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/Success";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "/guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("Success");
        }

        private PayPal.Api.Payment payment;
        private PayPal.Api.Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new PayPal.Api.Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private PayPal.Api.Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            foreach (var item in cart)
            {
                itemList.items.Add(new Item()
                {
                    name = item.Product.ProductName,
                    price = Math.Round(item.Product.Price / tigia, 2).ToString(),
                    quantity = item.Quantity.ToString(),
                    currency = "USD",
                    sku = "sku"
                });
            };
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = Math.Round(pay.TotalPay / tigia, 2).ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = Math.Round(pay.TotalPay / tigia, 2).ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = "your generated invoice number", //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }       
    }
}