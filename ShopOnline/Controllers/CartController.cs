using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using PayPal.Api;

namespace shopOnline.Controllers
{
    public class CartController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public List<Cart> getCart() // Create list cart and save in session 
        {
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart == null)
            {
                // If the list item in the cart empty, it will create a list contains items
                listCart = new List<Cart>();
                Session["Cart"] = listCart;
            }
            return listCart;
        }
        public ActionResult AddToCart(Guid id, string strURL) //  Add item in cart 
        {
            List<Cart> listCart = getCart();
            Cart item = listCart.Find(model => model.idItem == id);
            if (item == null)
            {
                item = new Cart(id);
                listCart.Add(item);
                return Redirect(strURL);
            }
            else
            {
                item.quantity++;
                return Redirect(strURL);
            }
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
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View(listCart);
        }
        public ActionResult DeteteCart(Guid id)
        {
            List<Cart> listCart = getCart();
            Cart item = listCart.SingleOrDefault(model => model.idItem == id);
            if (item != null)
            {
                listCart.RemoveAll(model => model.idItem == id);
                return RedirectToAction("Cart", "Cart");
            }
            if (listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cart", "Cart");
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
        public static string ConvertTimeTo24(string hour)
        {
            string h = "";
            switch (hour)
            {
                case "1":
                    h = "13";
                    break;
                case "2":
                    h = "14";
                    break;
                case "3":
                    h = "15";
                    break;
                case "4":
                    h = "16";
                    break;
                case "5":
                    h = "17";
                    break;
                case "6":
                    h = "18";
                    break;
                case "7":
                    h = "19";
                    break;
                case "8":
                    h = "20";
                    break;
                case "9":
                    h = "21";
                    break;
                case "10":
                    h = "22";
                    break;
                case "11":
                    h = "23";
                    break;
                case "12":
                    h = "0";
                    break;
            }
            return h;
        }
        public static string CreateKey(string tiento) // Tạo chuỗi mã hóa ngày - giờ cho mã hóa đơn
        {
            string key = tiento;
            string[] partsDay;
            partsDay = DateTime.Now.ToShortDateString().Split('/');
            //Ví dụ 07/08/2009
            string d = String.Format("{0}{1}{2}", partsDay[0], partsDay[1], partsDay[2]);
            key = key + d;
            string[] partsTime;
            partsTime = DateTime.Now.ToLongTimeString().Split(':');
            //Ví dụ 7:08:03 PM hoặc 7:08:03 AM
            if (partsTime[2].Substring(3, 2) == "PM")
                partsTime[0] = ConvertTimeTo24(partsTime[0]);
            if (partsTime[2].Substring(3, 2) == "AM")
                if (partsTime[0].Length == 1)
                    partsTime[0] = "0" + partsTime[0];
            //Xóa ký tự trắng và PM hoặc AM
            partsTime[2] = partsTime[2].Remove(2, 3);
            string t;
            t = String.Format("_{0}{1}{2}", partsTime[0], partsTime[1], partsTime[2]);
            key = key + t;
            return key;
        }
        
        // Trang checkout khi đã đăng nhập tài khoản
        [HttpGet]
        public ActionResult Checkout()
        {
            List<Cart> listCart = getCart();
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();

            return View(listCart);
        }
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
                    bill.dateCreate = DateTime.Now;
                    bill.status = false;
                    bill.deliveryDate = null;
                    bill.deliveryStatus = false;

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
                        ctdh.price = item.unitPrice;
                        ctdh.discount = item.discount * item.quantity;
                        db.InvoinceDetails.Add(ctdh);
                    }
                    db.SaveChanges();
                    return RedirectToAction("SubmitBill", "Cart");
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }
        
        //Trang checkout khi không đăng nhập tài khoản
        [HttpGet]
        public ActionResult CheckoutNoAccount()
        {
            List<Cart> listCart = getCart();
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View(listCart);
        }
        [HttpPost]
        public ActionResult CheckoutNoAccount(Member customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Members.Where(model => model.lastName == customer.lastName && model.email == customer.email).FirstOrDefault();
                    // Kiểm tra xem tên khách hàng có email nhập đã tồn tại hay chưa
                    if (check == null) // Nếu chưa thì lưu lại thông tin vào bảng customer
                    {
                        db.Members.Add(customer);
                        db.SaveChanges();
                        Invoince bill = new Invoince();
                        List<Cart> listCart = getCart();
                        bill.memberId = customer.memberId;
                        bill.invoinceId = Guid.NewGuid();
                        bill.dateCreate = DateTime.Now;
                        bill.status = false;
                        bill.deliveryDate = null;
                        bill.deliveryStatus = false;
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
                            ctdh.productId = item.idItem;
                            ctdh.quanlity = item.quantity;
                            ctdh.price = item.unitPrice;
                            /*ctdh.totalPrice = (int?)(long)item.PriceTotal;*/
                            ctdh.discount = item.discount * item.quantity;
                            db.InvoinceDetails.Add(ctdh);
                        }
                        db.SaveChanges();
                        return RedirectToAction("SubmitBill", "Cart");
                    }
                    else
                    {
                        Invoince bill = new Invoince();
                        List<Cart> listCart = getCart();
                        bill.memberId = check.memberId;
                        bill.invoinceId = Guid.NewGuid();
                        bill.dateCreate = DateTime.Now;
                        bill.status = false;
                        bill.deliveryDate = null;
                        bill.deliveryStatus = false;
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
                            ctdh.productId = item.idItem;
                            ctdh.quanlity = item.quantity;
                            ctdh.price = item.unitPrice;
                            /*ctdh.totalPrice = (int?)(long)item.PriceTotal;*/
                            ctdh.discount = item.discount * item.quantity;
                            db.InvoinceDetails.Add(ctdh);
                        }
                        db.SaveChanges();
                        return RedirectToAction("SubmitBill", "Cart");
                    }

                }

                return View();
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public ActionResult SubmitBill()
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View();
        }
        [HttpPost]
        public ActionResult SubmitBill(FormCollection form)
        {
            Session.Remove("Cart");
            //Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }

        // Paypal 
        private Payment payment;
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listItems = new ItemList() { items = new List<Item>() };
            List<Cart> listCart = getCart();
            foreach(var cart in listCart)
            {
                listItems.items.Add(new Item()
                {
                    name = cart.nameItem,
                    currency = "USD",
                    price = cart.priceItem.ToString(),
                    quantity = cart.quantity.ToString(),
                    sku = "sku"
                });
            }

            var payer = new Payer() { payment_method = "paypal"};

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = listCart.Sum(model => model.quantity * model.priceItem).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(), // tax + shipping + subtotal
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Menfashion transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = listItems
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURL + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;

                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        Session.Remove("Cart");
                        return RedirectToAction("Error","Home");
                    }
                }
            }
            catch (Exception ex)
            {
                PaypalLogger.Log("Error: " + ex.Message);
                Session.Remove("Cart");
                return RedirectToAction("Error", "Home");
            }


            Member member = (Member)Session["info"];
            if(member!= null) // Nếu đã đăng nhập thì lấy thông tin thành viên trong session info
            {
                Invoince bill = new Invoince();
                List<Cart> listCart = getCart();
                bill.invoinceId = Guid.NewGuid();
                bill.memberId = member.memberId;
                bill.dateCreate = DateTime.Now;
                bill.status = false;
                bill.deliveryDate = null;
                bill.deliveryStatus = false;
                // Biến totalmoney lưu tổng tiền sản phẩm từ giỏ hàng
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
                    ctdh.productId = item.idItem;
                    ctdh.quanlity = item.quantity;
                    ctdh.price = item.unitPrice;
                   /* ctdh.totalPrice = (int?)(long)item.PriceTotal;*/
                    ctdh.discount = item.discount * item.discount;
                    db.InvoinceDetails.Add(ctdh);
                }
                db.SaveChanges();
                return RedirectToAction("SubmitBill", "Cart");
            }
            else
            {
                // Xử lí dữ liệu khách hàng nhập
                //Session.Remove("Cart");
                return RedirectToAction("SubmitBill", "Cart");
            }
        }
    }
}