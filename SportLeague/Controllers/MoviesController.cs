using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportLeague.Models;
using System.IO;

namespace SportLeague.Controllers
{
    public class MoviesController : Controller
    {
        private MyContext db = new MyContext();

        private int pageSize = 3;
        private int numbersSize = 3;

        private void UploadFile(HttpPostedFileBase file, Movies movie)
        {
            if (file != null && file.ContentLength > 0)

            {
                string path = Path.Combine(Server.MapPath("~/Media/Posters"),
                    Path.GetFileName(file.FileName));

                file.SaveAs(path);

                movie.PosterURL = Path.GetFileName(file.FileName);
            }
        }

        private void CalculateAndSetPagination(int page, int pagesCount)
        {
            int leftOffset = numbersSize / 2;
            int rightOffset = numbersSize % 2 == 0 ? leftOffset - 1 : leftOffset;
            int start = 0, end = pagesCount;


            if (numbersSize > 0 && pagesCount > numbersSize)
            {

                if (page <= leftOffset)
                {
                    end = numbersSize;
                }
                else if (pagesCount - page < leftOffset)
                {
                    start = pagesCount - numbersSize;
                }
                else
                {
                    start = page - leftOffset - 1;
                    end = page + rightOffset;
                }
            }

            ViewBag.Start = start;
            ViewBag.End = end;
        }

        public async Task<ActionResult> Index(int page = 1)
        {

            var pagesCount = (int)Math.Ceiling(db.Movies.Count() / (double)pageSize);


            if (page < 1 || page > pagesCount)
            {
                page = 1;
            }

            CalculateAndSetPagination(page, pagesCount);
            
            var movies = db.Movies.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).Include(m => m.Users);
            ViewBag.CurrentPage = page;
            ViewBag.PagesCount = pagesCount;
            return View(await movies.ToListAsync());
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movies movies = await db.Movies.FindAsync(id);
            if (movies == null)
            {
                return HttpNotFound();
            }
            return View(movies);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,Year,Director")] Movies movies, HttpPostedFileBase Poster)
        {
            if (ModelState.IsValid)
            {
                UploadFile(Poster, movies);
                var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                movies.IdUser = currentUser.Id;
                db.Movies.Add(movies);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(movies);
        }


        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movies movies = await db.Movies.FindAsync(id);
            if (movies == null)
            {
                return HttpNotFound();
            }


            var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (!User.Identity.IsAuthenticated || currentUser?.Id != movies.IdUser)
            {
                return View("CustomError", (object)"Only the movie creator is able to edit");
            }

            return View(movies);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,IdUser,Title,Description,Year,Director")] Movies movies, HttpPostedFileBase Poster)
        {
            var currentUser = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (currentUser.Id != movies.IdUser)
            {
                return View("CustomError", (object)"Only the movie creator is able to edit");
            }
            if (ModelState.IsValid)
            {
                db.Entry(movies).State = EntityState.Modified;
                if (Poster == null)
                {
                    db.Entry(movies).Property(m => m.PosterURL).IsModified = false;
                }
                else
                {
                    UploadFile(Poster, movies);
                }
             
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(movies);
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
