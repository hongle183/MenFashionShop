using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Controllers
{
    public class MenusController : Controller
    {
        private menfsEntities db = new menfsEntities();

        // GET: Menus
        public ActionResult Index()
        {
            return View(db.Menus.Where(model => model.status == true).OrderBy(model => model.order).ToList());
        }
        public PartialViewResult Categories()
        {
            return PartialView(db.ProductCategories.Where(model => model.status == true).OrderBy(model => model.order).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
