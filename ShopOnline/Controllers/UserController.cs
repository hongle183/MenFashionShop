using System;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ShopOnline.Controllers
{
    public class UserController : Controller
    {
        menfsEntities db = new menfsEntities();
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
        public JsonResult SignIn(string username, string password, string recaptchaToken)
        {
            if (string.IsNullOrEmpty(recaptchaToken) || !VerifyRecaptcha(recaptchaToken))
            {
                // TempData["msgFailed"] = "Xác minh reCAPTCHA không thành công. Vui lòng thử lại.";
                return Json(new { success = false, message = "Xác minh reCAPTCHA không thành công. Vui lòng thử lại." });
            }

            password = Encryptor.MD5Hash(password);
            var check = db.Members.Where(model => (model.phone == username || model.email == username) && model.password == password).SingleOrDefault();
            if (check != null)
            {
                FormsAuthentication.SetAuthCookie(check.phone, false);
                Session["UserRole"] = "User";
                Session["info"] = check;
                // TempData["msgLoginSuccess"] = "Đăng nhập thành công!";
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home"), message = "Đăng nhập thành công!" });
            }
            else
            {
                // TempData["msgFailed"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
            }
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Register(Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Members.Where(model => model.phone == member.phone || model.email == member.email).FirstOrDefault();

                    if (check != null)
                    {
                        // check username constained in database
                        return Json(new { success = false, message = "Số điện thoại hoặc email đã được sử dụng" });
                    }
                    // 👇 Gán GUID thủ công trong code
                    member.memberId = Guid.NewGuid();

                    member.password = Encryptor.MD5Hash(member.password);
                    member.dateCreate = DateTime.Now;
                    member.roleId = Guid.Parse("54ed1855-5103-4121-811c-3997ce4c2241");
                    member.avatar = "~/Content/img/avatar.png";
                    member.status = true;
                    db.Members.Add(member);
                    var result = db.SaveChanges();
                    
                    if (result > 0)
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("SignIn", "User"), message = "Đăng ký thành công! Vui lòng đăng nhập." });
                    }
                }
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Xảy ra lỗi! " + ex.Message });
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("SignIn");
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