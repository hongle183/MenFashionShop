using System.Web.Mvc;
using System.Web;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string _role;

    public CustomAuthorizeAttribute(string role)
    {
        _role = role;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        if (httpContext.Session["UserRole"] == null)
        {
            return false; // Không cho phép truy cập
        }

        return httpContext.Session["UserRole"].ToString() == _role;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        if (_role == "Admin")
        {
            filterContext.Result = new RedirectResult("~/Admin/LoginMember/Login");
        }
        else
        {
            // Lưu thông báo vào TempData
            filterContext.Controller.TempData["Message"] = "Bạn cần đăng nhập để tiếp tục thao tác.";
            filterContext.Result = new RedirectResult("~/dang-nhap");
        }        
    }
}
