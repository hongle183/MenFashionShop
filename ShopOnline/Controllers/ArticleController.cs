using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CSharpVitamins;
using PagedList;
using ShopOnline.Models;

namespace ShopOnline.Controllers
{
    public class ArticleController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        public ActionResult ArticleList(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);

            var ListBlog = db.Articles.OrderByDescending(model => model.dateCreate).ToPagedList(pageNum, pageSize);
            return View(ListBlog);
        }
        public ActionResult ArticleDetail(string meta)
        {
            if (meta == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var tmp = meta.Split('-');
            ShortGuid id = (ShortGuid)tmp[tmp.Length - 1];
            var item = db.Articles.Where(model => model.articleId == id.Guid).Single();
            if (item == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(item);
        }
    }
}