using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using March_Madness.Models;
using March_Madness.Models.ViewModels;

namespace March_Madness.Controllers
{
	[Authorize(Roles = Roles.Admin)]
	public class UsersController : Controller
    {
		
        private ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(_context.Users.ToList());
        }

		protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
