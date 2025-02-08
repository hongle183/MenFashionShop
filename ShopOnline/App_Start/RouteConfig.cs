using System.Web.Mvc;
using System.Web.Routing;

namespace ShopOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route cho Trang chủ
            routes.MapRoute(
                name: "Home",
                url: "trang-chu/{meta}",
                defaults: new { controller = "Home", action = "Index", meta = UrlParameter.Optional }
            );

            // Route cho Sản phẩm
            routes.MapRoute(
                name: "Shop",
                url: "san-pham/{meta}",
                defaults: new { controller = "Shop", action = "Shop", meta = UrlParameter.Optional }
            );

            // Route cho Bài viết
            routes.MapRoute(
                name: "ArticleList",
                url: "bai-viet/{meta}",
                defaults: new { controller = "Article", action = "ArticleList", meta = UrlParameter.Optional }
            );

            // Route cho Về chúng tôi
            routes.MapRoute(
                name: "About",
                url: "ve-chung-toi/{meta}",
                defaults: new { controller = "Home", action = "About", meta = UrlParameter.Optional }
            );

            // Route cho Liên hệ
            routes.MapRoute(
                name: "Contact",
                url: "lien-he/{meta}",
                defaults: new { controller = "Home", action = "Contact", meta = UrlParameter.Optional }
            );

            // Route cho Chi tiết bài viết
            routes.MapRoute(
                name: "ArticleDetail",
                url: "chi-tiet-bai-viet/{meta}--{id}",
                defaults: new { controller = "Article", action = "ArticleDetail", id = UrlParameter.Optional }
            );

            // Route cho Chi tiết sản phẩm
            routes.MapRoute(
                name: "ProductDetail",
                url: "chi-tiet-san-pham/{meta}--{id}",
                defaults: new { controller = "Shop", action = "ProductDetail", id = UrlParameter.Optional }
            );

            // Route cho Giỏ hàng
            routes.MapRoute(
                name: "Cart",
                url: "gio-hang",
                defaults: new { controller = "Cart", action = "Cart" }
            );

            // Route cho Đặt hàng
            routes.MapRoute(
                name: "Checkout",
                url: "gio-hang/dat-hang",
                defaults: new { controller = "Cart", action = "Checkout" }
            );

            // Route cho Xác nhận đơn hàng
            routes.MapRoute(
                name: "SubmitBill",
                url: "gio-hang/xac-nhan-don-hang",
                defaults: new { controller = "Cart", action = "SubmitBill" }
            );

            // Route cho Theo dõi đơn hàng
            routes.MapRoute(
                name: "MyOrder",
                url: "don-hang-cua-toi/{memberId}",
                defaults: new { controller = "Home", action = "MyOrder", memberId = UrlParameter.Optional }
            );

            // Route cho Chi tiết đơn hàng
            routes.MapRoute(
                name: "InvoinceDetail",
                url: "don-hang-cua-toi/{memberId}/chi-tiet/{invoinceId}",
                defaults: new { controller = "Home", action = "InvoinceDetail", memberId = UrlParameter.Optional, invoinceId = UrlParameter.Optional }
            );

            // Route cho Đăng nhập
            routes.MapRoute(
                name: "SignIn",
                url: "dang-nhap",
                defaults: new { controller = "User", action = "SignIn" }
            );

            // Route cho Đăng ký
            routes.MapRoute(
                name: "Register",
                url: "dang-ky",
                defaults: new { controller = "User", action = "Register" }
            );

            // Route cho Đăng xuất
            routes.MapRoute(
                name: "LogOut",
                url: "dang-xuat",
                defaults: new { controller = "User", action = "LogOut" }
            );

            // Route cho Đổi mật khẩu
            routes.MapRoute(
                name: "ChangePassword",
                url: "doi-mat-khau/{memberId}",
                defaults: new { controller = "Home", action = "ChangePassword", memberId = UrlParameter.Optional }
            );

            // Route cho Thông tin cá nhân
            routes.MapRoute(
                name: "EditProfie",
                url: "thong-tin-ca-nhan/{memberId}",
                defaults: new { controller = "Home", action = "EditProfie", memberId = UrlParameter.Optional }
            );

            // Route mặc định
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
