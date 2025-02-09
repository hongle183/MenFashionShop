using System.Web.Mvc;
using System.Web;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        if (httpContext.Session["info"] == null)
        {
            return false; // Không cho phép truy cập
        }

        return true; // Đã đăng nhập
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        // Lưu thông báo vào TempData
        filterContext.Controller.TempData["Message"] = "Bạn cần đăng nhập để tiếp tục thao tác.";
        filterContext.Result = new RedirectResult("~/dang-nhap");
    }
}
