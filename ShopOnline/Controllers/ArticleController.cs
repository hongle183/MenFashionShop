using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
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
        public ActionResult ArticleDetail(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ShortGuid gid = (ShortGuid)id;
            var item = db.Articles.Where(model => model.articleId == gid.Guid).Single();
            if (item == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(item);
        }
    }
}