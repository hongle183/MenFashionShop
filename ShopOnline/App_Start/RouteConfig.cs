using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Trang-chu",
                "{type}/{meta}",
                new { controller = "Home", action = "Index", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "trang-chu"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "San-pham",
                "{type}",
                new { controller = "Shop", action = "Shop", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", @"san-pham"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Bai-viet",
                "{type}/{meta}",
                new { controller = "Article", action = "ArticleList", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "bai-viet"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Ve-chung-toi",
                "{type}/{meta}",
                new { controller = "Home", action = "About", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "ve-chung-toi"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Lien-he",
                "{type}/{meta}",
                new { controller = "Home", action = "Contact", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "lien-he"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Chi-tiet-bai-viet",
                "{type}/{meta}",
                new { controller = "Article", action = "ArticleDetail", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "chi-tiet-bai-viet"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Chi-tiet-san-pham",
                "{type}/{meta}",
                new { controller = "Shop", action = "ProductDetail", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "chi-tiet-san-pham"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Gio-hang",
                "{type}/{meta}",
                new { controller = "Cart", action = "Cart", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "gio-hang"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Dang-nhap",
                "{type}/{meta}",
                new { controller = "User", action = "SignIn", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "dang-nhap"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                "Dang-ky",
                "{type}/{meta}",
                new { controller = "User", action = "Register", meta = UrlParameter.Optional },
                new RouteValueDictionary
                {
                    {"type", "dang-ky"}
                },
                new[] { "ShopOnline.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
