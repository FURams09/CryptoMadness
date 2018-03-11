using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace March_Madness.Controllers
{
	public class PoolController : Controller
	{
		private ApplicationDbContext _context;
		public PoolController() {
			_context = new ApplicationDbContext();
		}
        // GET: Pools
        public ActionResult Index()
        {
            return View("Index", GetPoolViewModel());
        }

		public ActionResult Create(Pool newPool)
		{
			if (newPool.Nickname != null)
			{
				newPool.OwnerId = User.Identity.GetUserId();
				_context.Pools.Add(newPool);

				if (ModelState.IsValid)
				
				{
					_context.SaveChanges();
					return View("Index", GetPoolViewModel());
				}
				
			}

			return View(newPool);
		}

		public ActionResult View(int poolId)
		{
	
			List<BracketEntry> poolBrackets = _context.BracketEntries.Where(be => be.PoolId == poolId).Include(u => u.BracketOwner).ToList();
			var pool = _context.Pools.SingleOrDefault(p => p.Id == poolId);
			var poolBracketViewModel = new PoolBracketViewModel()
			{
				PoolId = pool.Id,
				PoolNickname = pool.Nickname,
				EntriesInPool = poolBrackets
			};

			return View(poolBracketViewModel);
			
		}

		private OwnedPoolsViewModel GetPoolViewModel()
		{
			var ownerId = User.Identity.GetUserId();
			var ownedPools = _context.Pools.Include(t => t.Owner).Where(T => T.OwnerId == ownerId);

			OwnedPoolsViewModel viewModel = new OwnedPoolsViewModel
			{
				OwnerName = User.Identity.Name,
				OwnerId = ownerId,
				OwnedPools = ownedPools.ToList()
			};

			return viewModel;
		}
    }

}