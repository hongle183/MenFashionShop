using System;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using System.Net;
using System.Web.Script.Serialization;

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
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string username, string password, string recaptchaToken)
        {
            if (string.IsNullOrEmpty(recaptchaToken) || !VerifyRecaptcha(recaptchaToken))
            {
                TempData["msgFailed"] = "Xác minh reCAPTCHA không thành công. Vui lòng thử lại.";
                return RedirectToAction("SignIn");
            }

            password = Encryptor.MD5Hash(password);
            var check = db.Members.Where(model => (model.phone == username || model.email == username) && model.password == password).SingleOrDefault();
            if (check != null)
            {
                Session["info"] = check;
                TempData["msgSuccess"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msgFailed"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return RedirectToAction("SignIn");
            }
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
                        member.status = true;
                        db.Members.Add(member);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                            return RedirectToAction("SignIn");
                        }
                    }
                }
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["msgFailed"] = "Xảy ra lỗi! " + ex.Message;
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

        private bool VerifyRecaptcha(string token)
        {
            string secretKey = "6LcwOrIqAAAAAEcsgF-BkkfPmckoqbwfMOI57L_G";
            string apiUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}";

            using (var client = new WebClient())
            {
                var result = client.DownloadString(apiUrl);
                var recaptchaResult = new JavaScriptSerializer().Deserialize<ReCaptchaResponse>(result);
                return recaptchaResult.Success && recaptchaResult.Score >= 0.5; // Kiểm tra điểm số
            }
        }
    }
}