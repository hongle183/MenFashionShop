﻿using System;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using CaptchaMvc.HtmlHelpers;

namespace ShopOnline.Controllers
{
    public class UserController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        [HttpGet]
        public ActionResult SignIn()
        {
            if (Session["info"] != null) // Nếu đã đăng nhập rồi thì sửa link vào đăng nhập sẽ điều hướng sang trang chủ
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult SignIn(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];
            password = Encryptor.MD5Hash(password);

            var check = db.Members.Where(model => (model.phone == username || model.email == username) && model.password == password).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (check == null)
                {
                    ModelState.AddModelError("", "Số điện thoại/ Email hoặc mật khẩu không hợp lệ.");
                }
                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.captcha = "Captcha không hợp lệ.";
                    }
                    else
                    {
                        Session["info"] = check;
                        return RedirectToAction("Index", "Home");
                    }

                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Members.Where(model => model.phone == member.phone || model.email == member.email).FirstOrDefault();

                    if (check != null)
                    {
                        // check username constained in database
                        ModelState.AddModelError("", "Số điện thoại hoặc email đã được sử dụng");
                        return View(member);
                    }
                    else
                    {
                        member.password = Encryptor.MD5Hash(member.password);
                        member.dateCreate = DateTime.Now;
                        member.roleId = Guid.Parse("54ed1855-5103-4121-811c-3997ce4c2241");
                        member.avatar = "~/Content/img/avatar.png";
                        member.status = false;
                        db.Members.Add(member);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Tạo tài khoản thành công! Vui lòng đăng nhập.";
                            return RedirectToAction("SignIn");
                        }
                    }
                }
                return View(member);
            }
            catch(Exception ex)
            {
                TempData["msgFailed"] = "Xảy ra lỗi! " +ex.Message;
                return RedirectToAction("SignIn");
            }
        }
        public ActionResult LogOut()
        {
            //Session.Remove("info");
            //Session["info"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}