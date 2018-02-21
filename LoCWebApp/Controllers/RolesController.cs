using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoCWebApp.Models;
using Microsoft.AspNet.Identity.Owin;

namespace LoCWebApp.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public RolesController()
        {
        }

        public RolesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Roles/RemoveUserFromRole
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public ActionResult RemoveUserFromRole(string userId, string roleName)
        {
            try
            {
                var results = UserManager.RemoveFromRoleAsync(userId, roleName).Result;
                if (results.Succeeded)
                {
                    ViewBag.StatusMessage = true;
                    ViewBag.Message = "User was removed from that role successfully.";
                }
                else
                {
                    ViewBag.StatusMessage = false;
                    ViewBag.Message = "There was a problem removing that user from the role.";
                }
            }
            catch
            {
                ViewBag.StatusMessage = false;
                ViewBag.Message = "There was a problem removing that user from the role.";
            }
            return View("UserRoles", db.Roles.ToList());
        }

        // GET: Roles/AddUserToRole
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public ActionResult AddUserToRole(FormCollection collection)
        {
            try
            {
                string userId = collection["userId"], roleId = collection["roleName"];
                db.UserRoles.Add(new ApplicationUserRole { UserId = userId, RoleId = roleId });
                //var results = UserManager.AddToRoleAsync(userId, roleName).Result;
                if (db.SaveChangesAsync().Result > 0)
                {
                    ViewBag.StatusMessage = true;
                    ViewBag.Message = "User was added to that role successfully.";
                }
                else
                {
                    ViewBag.StatusMessage = false;
                    ViewBag.Message = "There was a problem adding that user to the role.";
                }
            }
            catch
            {
                ViewBag.StatusMessage = false;
                ViewBag.Message = "There was a problem adding that user to the role.";
            }
            return View("UserRoles", db.Roles.ToList());
        }

        // GET: Roles/UserRoles
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public ActionResult UserRoles()
        {
            return View(db.Roles.ToList());
        }

        // GET: Roles
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            return View(await db.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = await db.Roles.FindAsync(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // GET: Roles/Create
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ApplicationRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(applicationRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(applicationRole);
        }

        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = await db.Roles.FindAsync(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ApplicationRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationRole);
        }

        // GET: Roles/Delete/5
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationRole applicationRole = await db.Roles.FindAsync(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // POST: Roles/Delete/5
        [Authorize(Roles ="Leader")]
        //[AllowAnonymous]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ApplicationRole applicationRole = await db.Roles.FindAsync(id);
            db.Roles.Remove(applicationRole);
            await db.SaveChangesAsync();
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
