using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using System.Web.Security;
using System.Net;
using System.Web.Script.Serialization;

namespace ShopOnline.Areas.Admin.Controllers
{
    public class LoginMemberController : Controller
    {
        menfsEntities db = new menfsEntities();
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["infoAdmin"] != null)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string recaptchaToken)
        {
            if (string.IsNullOrEmpty(recaptchaToken) || !VerifyRecaptcha(recaptchaToken))
            {
                TempData["msgFailed"] = "Xác minh reCAPTCHA không thành công. Vui lòng thử lại.";
                return RedirectToAction("Login");
            }

            password = Encryptor.MD5Hash(password);
            var check = db.Members.Where(model => model.phone == "admin" && model.password == password).SingleOrDefault();

            // Tiếp tục xử lý logic login
            if (check == null)
            {
                TempData["msgFailed"] = "Username or password is incorrect.";
                return View();
            }

            FormsAuthentication.SetAuthCookie(check.phone, false);
            Session["UserRole"] = "Admin";
            Session["infoAdmin"] = check;

            return RedirectToAction("Index", "DashBoard");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
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