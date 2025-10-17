using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace shopOnline.Controllers
{
    public class CartController : Controller
    {
        menfsEntities db = new menfsEntities();
        private List<Cart> getCart() // Tạo danh sách giỏ hàng và lưu vào session
        {
            if (!(Session["Cart"] is List<Cart> listCart))
            {
                listCart = new List<Cart>();
                Session["Cart"] = listCart;
            }
            return listCart;
        }
        [HttpPost]
        public JsonResult AddToCart(string id, int? quantity) // Thêm item vào giỏ hàng 
        {
            if (!Guid.TryParse(id, out Guid productId))
                return Json(new { success = false });            

            var cart = getCart();

            var item = cart.FirstOrDefault(c => c.idItem == productId);
            if (item == null)
            {
                item = new Cart(productId)
                {
                    quantity = quantity ?? 1
                };
                cart.Add(item);
            }
            else item.quantity += (quantity ?? 1);            

            // Luôn cập nhật session
            Session["Cart"] = cart;

            // Trả về kết quả cho AJAX
            return Json(new { success = true });
        }
        private int Quanlity() // Lấy tổng số sản phẩm giỏ hàng hiện tại
        {
            int amount = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                amount = listCart.Sum(model => model.quantity);
            }
            return amount;
        }
        private double DiscountPrice() //  Lấy tổng số tiền giảm giá
        {
            double total = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                total = listCart.Sum(model => model.discountTotal);
            }
            return total;
        }
        private double Price() //  Lấy tổng số tiền sản phẩm khi chưa được giảm giá
        {
            double total = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                total = listCart.Sum(model => model.unitPrice * model.quantity);
            }
            return total;
        }
        private double TotalPrice() //  Lấy tổng số tiền sản phẩm
        {
            double total = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                total = listCart.Sum(model => model.priceTotal);
            }
            return total;
        }
        public PartialViewResult Navbar() // Hiển thị số lượng sản phẩm và tiền trên navbar
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return PartialView();
        }

        [HttpGet]
        public ActionResult Cart()
        {
            List<Cart> listCart = getCart();
            Session["Cart"] = listCart;
            ViewBag.totalPrice = TotalPrice();
            ViewBag.price = Price();
            ViewBag.discount = DiscountPrice();
            return View(listCart);
        }
        [HttpPost]
        public PartialViewResult DeleteCart(string id)
        {
            Guid.TryParse(id, out Guid productId);               

            var cart = getCart();
            var item = cart.FirstOrDefault(c => c.idItem == productId);
            if (item != null)
            {
                cart.Remove(item);
                Session["Cart"] = cart;
                ViewBag.totalPrice = TotalPrice();
                ViewBag.price = Price();
                ViewBag.discount = DiscountPrice();
            }

            return PartialView("_ListItem", cart);
        }
        public ActionResult UpdateCart(FormCollection form)
        {
            string[] qualities = form.GetValues("quanlity");
            List<Cart> listCart = getCart();
            for (int i = 0; i < listCart.Count; i++)
            {
                listCart[i].quantity = Convert.ToInt32(qualities[i]);
            }
            if (Quanlity() == 0)
            {
                Session["Cart"] = null;
                return RedirectToAction("NoICart", "Cart");
            }
            else
            {
                return RedirectToAction("Cart", "Cart");
            }
        }

        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult Checkout()
        {
            List<Cart> listCart = getCart();
            if (listCart.Count == 0)
            {
                return RedirectToAction("Cart");
            }
            ViewBag.totalPrice = TotalPrice();
            ViewBag.price = Price();
            ViewBag.discount = DiscountPrice();

            return View(listCart);
        }
        [CustomAuthorize("User")]
        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Invoince bill = new Invoince();
                    Member member = (Member)Session["info"]; // Lấy thông tin tài khoản từ session 
                    List<Cart> listCart = getCart();
                    bill.invoinceId = Guid.NewGuid();
                    bill.memberId = member.memberId;
                    bill.paymentMethod = collection["paymentMethod"];
                    bill.paymentStatus = "pending";
                    bill.note = collection["note"];
                    bill.status = "pending";
                    bill.dateCreate = DateTime.Now;

                    // Biến totalmney lưu tổng tiền sản phẩm từ giỏ hàng
                    int totalmoney = 0;
                    foreach (var item in listCart)
                    {
                        totalmoney += Convert.ToInt32(item.priceTotal);
                    }
                    bill.totalMoney = totalmoney;
                    db.Invoinces.Add(bill);
                    db.SaveChanges();
                    foreach (var item in listCart)
                    {
                        InvoinceDetail ctdh = new InvoinceDetail();
                        ctdh.invoinceId = bill.invoinceId;
                        ctdh.productId = Guid.Parse(item.idItem.ToString());
                        ctdh.quanlity = item.quantity;
                        ctdh.price = item.priceItem;
                        ctdh.discount = item.discountTotal;
                        db.InvoinceDetails.Add(ctdh);
                    }
                    db.SaveChanges();
                    Session.Remove("Cart");

                    if (collection["paymentMethod"] == "vnpay")
                    {
                        return RedirectToAction("CreatePaymentUrl", "Payment", new { invoinceId = bill.invoinceId });
                    }

                    TempData["msgOrder"] = "Đặt hàng thành công!";
                    return RedirectToAction("MyOrder", "Home", new { memberId = member.memberId });
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["msgOrderFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index");
            }
        }

        [CustomAuthorize("User")]
        [HttpGet]
        public ActionResult SubmitBill()
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View();
        }
    }
}