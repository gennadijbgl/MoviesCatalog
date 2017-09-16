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
using System.Web.Security;
using System.Security.Cryptography;                     
using System.Text;
using System.Collections;

namespace SportLeague.Controllers
{
    public class UsersController : Controller
    {
        private MyContext db = new MyContext();

        private void GeneratePasswordHash(Users user)
        {
            byte[] salt = new byte[16];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(30);

            user.Salt = Convert.ToBase64String(salt);
            user.Password = Convert.ToBase64String(hash);

        }

        private bool ValidatePassword(Users user, string password)
        {
            bool isValid = false;

            var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(user.Salt), 10000);
            byte[] hash = pbkdf2.GetBytes(30);

            isValid = StructuralComparisons.StructuralEqualityComparer.Equals(hash, Convert.FromBase64String(user.Password));
            return isValid;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
                return PartialView(model);

            Users user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
            bool isValid = false;
            
            if(user!=null)
                isValid = ValidatePassword(user, model.Password);

            if (user != null && isValid)
            {      
                FormsAuthentication.SetAuthCookie(model.Login, true);
                return RedirectToAction("Index", "Movies");
            }


            ModelState.AddModelError("", "User not found");
            return PartialView(model);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Movies");
        }
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Include = "Id,Login,Password")] Users user)
        {
            if (ModelState.IsValid)
            {
                var userExist = await db.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                if (userExist == null)
                {
                    GeneratePasswordHash(user);
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Movies");
                }
                else
                {
                    ModelState.AddModelError("", "User with the same login exist");
                }
            }

            return View(user);
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
