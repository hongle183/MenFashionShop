using ShopOnline.Models;
using ShopOnline.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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

                var role = check.Role.roleName;
                Session["UserRole"] = role;
                if (role == "Manager")
                {
                    Session["infoAdmin"] = check;
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "DashBoard", new { area = "Admin" }), message = "Đăng nhập thành công!" });
                }

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
                    member.avatar = "/Content/img/avatar/avatar.png";
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

        [CustomAuthorize("Customer Member")]
        [HttpGet]
        public ActionResult EditProfile()
        {
            Member user = (Member)Session["info"];
            Session["imgPath"] = user.avatar;
            return View(user);
        }
        [CustomAuthorize("Customer Member")]
        [HttpPost]
        public ActionResult EditProfile(Member member, HttpPostedFileBase uploadFile)
        {
            try
            {
                Member user = (Member)Session["info"];
                if (ModelState.IsValid)
                {
                    // Lấy entity thật từ DB để cập nhật
                    var dbMember = db.Members.FirstOrDefault(m => m.memberId == user.memberId);

                    // Cập nhật thông tin được phép thay đổi
                    dbMember.firstName = member.firstName?.Trim();
                    dbMember.lastName = member.lastName?.Trim();
                    dbMember.email = member.email?.Trim();
                    dbMember.phone = member.phone?.Trim();
                    dbMember.address = member.address?.Trim();
                    dbMember.birthday = member.birthday;
                    
                    db.SaveChanges();

                    // Cập nhật session với dữ liệu mới
                    Session["info"] = dbMember;
                    TempData["msgEditProfile"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName", user.roleId);
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["msgEditProfileFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index", "Home");
            }
        }
        [CustomAuthorize("Customer Member")]
        [HttpPost]
        public JsonResult UpdateAvatar(HttpPostedFileBase uploadFile)
        {
            try
            {
                Member user = (Member)Session["info"];
                // Lấy entity thật từ DB để cập nhật
                var dbMember = db.Members.FirstOrDefault(m => m.memberId == user.memberId);

                string oldAvatarRel = dbMember.avatar;
                string oldAvatarAbs = !string.IsNullOrEmpty(oldAvatarRel) ? Server.MapPath(oldAvatarRel) : null;

                string url = "/Content/img/avatar/";
                var uploadDir = Server.MapPath(url);
                ImageServices imageServices = new ImageServices();
                string newAvatarRel = url + imageServices.SaveCroppedImage(uploadDir, uploadFile, 300, 300);

                if (string.IsNullOrEmpty(newAvatarRel))
                {
                    return Json(new { success = false, message = "Lỗi khi xử lý ảnh. Vui lòng thử lại." });
                }
                else
                {
                    dbMember.avatar = newAvatarRel;
                    if (System.IO.File.Exists(oldAvatarAbs))
                    {
                        System.IO.File.Delete(oldAvatarAbs);
                    }
                }
                db.SaveChanges();

                // Cập nhật session với dữ liệu mới
                Session["info"] = dbMember;
                return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công!", avatarUrl = newAvatarRel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message + "." });
            }
        }
        [CustomAuthorize("Customer Member")]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [CustomAuthorize("Customer Member")]
        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            try
            {
                Member user = (Member)Session["info"];
                var CurrentPassword = collection["CurPassword"]; // Mật khẩu hiện tại
                var NewPassword = collection["NewPassword"]; // Mật khẩu mới

                CurrentPassword = Encryptor.MD5Hash(CurrentPassword.Trim());
                NewPassword = Encryptor.MD5Hash(NewPassword.Trim());

                // Lấy lại bản ghi user từ DB
                var member = db.Members.FirstOrDefault(model => model.memberId == user.memberId);
                if (member != null && member.password == CurrentPassword)
                {
                    member.password = NewPassword;
                    db.SaveChanges();

                    // Cập nhật lại session
                    Session["info"] = member;
                    TempData["msgChangePassword"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Sai mật khẩu!");
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["msgChangePasswordFailed"] = "Đã xảy ra lỗi: " + ex.Message + ".";
                return RedirectToAction("Index", "Home");
            }
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