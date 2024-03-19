using System;
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
                    ModelState.AddModelError("", "Your phone/email or password is invalid.");
                }
                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.captcha = "Captcha is not valid";
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
                        ModelState.AddModelError("", "Your phone or email already exists.");
                        return View(member);
                    }
                    else
                    {
                        member.password = Encryptor.MD5Hash(member.password);
                        member.dateCreate = DateTime.Now;
                        member.roleId = Guid.Parse("c07429e6-4fcf-4fa7-8514-6a9a406c01e8");
                        member.avatar = "~/Content/img/avatar.png";
                        member.status = false;
                        db.Members.Add(member);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Successfully create account!";
                            return RedirectToAction("SignIn");
                        }
                    }
                }
                return View(member);
            }
            catch(Exception ex)
            {
                TempData["msgFailed"] = "Failed create account! " +ex.Message;
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