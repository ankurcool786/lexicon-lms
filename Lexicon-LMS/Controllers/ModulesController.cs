﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Lexicon_LMS.Models;

namespace Lexicon_LMS.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        public ActionResult Index()
        {
            var modules = db.Modules.Include(m => m.Course);
            return View(modules.ToList());
        }

        // GET: Modules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        [Authorize(Roles = "teacher")]   
        public ActionResult Create(int? courseId)
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", courseId);
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module)
        {
            // Check if module with this Name already exist            
            if (db.Modules.Any(m => m.Name == module.Name))
            {
                ModelState.AddModelError("Name", "Det finns redan en modul med detta Modulnamn");
            }

            if (module.StartDate.CompareTo(module.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Slutdatum kan inte inträffa innan startdatum");
            }

            Course course = db.Courses.Find(module.CourseId);         

            if (course.StartDate.CompareTo(module.StartDate) == 1)
            {
                ModelState.AddModelError("StartDate", "Modulens Startdatum får inte inträffa innan kursen startar");
            }

            if (course.StartDate.CompareTo(module.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Modulens Slutdatum får inte inträffa innan kursen startar");
            }

            if (module.EndDate.CompareTo(course.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Modulens Slutdatum får inte inträffa efter att kursen har slutat");
            }

            if (module.StartDate.CompareTo(course.EndDate) == 1)
            {
                ModelState.AddModelError("StartDate", "Modulens Startdatum får inte inträffa efter att kursen har slutat");
            }


            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = module.CourseId });
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            return View(module);
        }

        // GET: Modules/Edit/5
        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module)
        {
            if (module.StartDate.CompareTo(module.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Slutdatum får inte inträffa innan Startdatum");
            }

            Course course = db.Courses.Find(module.CourseId);

            if (course.StartDate.CompareTo(module.StartDate) == 1)
            {
                ModelState.AddModelError("StartDate", "Modulens Startdatum får inte inträffa innan kursens Startdatum");
            }

            if (course.StartDate.CompareTo(module.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Modulens Slutdatum får inte inträffa innan kursens Startdatum");
            }

            if (module.EndDate.CompareTo(course.EndDate) == 1)
            {
                ModelState.AddModelError("EndDate", "Modulens Slutdatum får inte inträffa efter kursens Slutdatum");
            }

            if (module.StartDate.CompareTo(course.EndDate) == 1)
            {
                ModelState.AddModelError("StartDate", "Modulens Startdatum får inte inträffa efter kursens Slutdatum");
            }      

            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "Courses", new { id = module.CourseId });
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", module.CourseId);
            return View(module);
        }

        // GET: Modules/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [Authorize(Roles = "teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Index");
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
