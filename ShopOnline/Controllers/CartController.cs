﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace shopOnline.Controllers
{
    public class CartController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public List<Cart> getCart() // Tạo danh sách giỏ hàng và lưu vào session
        {
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart == null)
            {
                listCart = new List<Cart>();
                Session["Cart"] = listCart;
            }
            return listCart;
        }
        public ActionResult AddToCart(Guid id, string strURL, int? quantity) // Thêm item vào giỏ hàng 
        {
            List<Cart> listCart = getCart();
            Cart item = listCart.Find(model => model.idItem == id);
            if (item == null)
            {
                item = new Cart(id);
                item.quantity = quantity ?? 1;
                listCart.Add(item);
                return Redirect(strURL);
            }
            else
            {
                item.quantity = item.quantity + (quantity ?? 1);
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

        [CustomAuthorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            List<Cart> listCart = getCart();
            ViewBag.totalPrice = TotalPrice();
            ViewBag.price = Price();
            ViewBag.discount = DiscountPrice();

            return View(listCart);
        }
        [CustomAuthorize]
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
                        ctdh.price = item.priceItem;
                        ctdh.discount = item.discountTotal;
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
        [CustomAuthorize]
        [HttpGet]
        public ActionResult SubmitBill()
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View();
        }
        [CustomAuthorize]
        [HttpPost]
        public ActionResult SubmitBill(FormCollection form)
        {
            Session.Remove("Cart");
            //Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}