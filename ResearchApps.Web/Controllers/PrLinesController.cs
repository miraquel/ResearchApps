using Microsoft.AspNetCore.Mvc;

namespace ResearchApps.Web.Controllers
{
    public class PrLinesController : Controller
    {
        // GET: PrLinesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PrLinesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PrLinesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrLinesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrLinesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PrLinesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PrLinesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PrLinesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
