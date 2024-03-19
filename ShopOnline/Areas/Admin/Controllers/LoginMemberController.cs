using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using CaptchaMvc.HtmlHelpers;
using System.Web.Security;

namespace ShopOnline.Areas.Admin.Controllers
{
    public class LoginMemberController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tk = collection["username"];
            var mk = collection["password"];
            mk = Encryptor.MD5Hash(mk);

            var notMember = db.Members.SingleOrDefault(model => model.phone == tk && model.password == "");
            var check = db.Members.SingleOrDefault(model => model.phone == tk && model.password == mk);
            if (ModelState.IsValid)
            {
                if (check == null)
                {
                    if (notMember != null)
                    {
                        ModelState.AddModelError("", "Role not invalid.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was a problem logging in. Check your username and password or create an account.");
                    }
                }

                else
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.captcha = "Captcha is not valid";
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(check.memberId.ToString(), false);
                        Session["userNameAdmin"] = check.memberId;
                        Session["infoAdmin"] = check;
                        return RedirectToAction("Index", "DashBoard");
                    }
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}