using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using ShopOnline.App_Start;

namespace ShopOnline
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["SoNguoiTruyCap"] = 0; // Đếm số lượt người đã truy cập vào website
            Application["SoNguoiDangOnline"] = 0; // Đếm số người hiện đang online.

        }
        protected void Session_Start()
        {
            Application.Lock(); // Dùng để xử lí tuần tự
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock(); // Dùng để đồng bộ hóa
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] - 1;
            Application.UnLock();
        }
    }
}
