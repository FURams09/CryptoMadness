using March_Madness.Models;
using March_Madness.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace March_Madness.Controllers.API
{
    public class PoolController : ApiController
    {
		private ApplicationDbContext _context;
		public PoolController()
		{
			_context = new ApplicationDbContext();
		}

		[Authorize]
		[HttpPost]
		[Route("api/Pool/Update")]
		public IHttpActionResult Update(PoolViewModel updatePool)
		{
			Pool updatingPool = _context.Pools.SingleOrDefault(p => p.Id == updatePool.PoolId);

			if (updatingPool == null)
			{
				return NotFound();
			}
			updatingPool.Nickname = updatePool.Nickname;
			updatingPool.EntryFee = updatePool.EntryFee ;
			//Don't have reason to update address yet. too important to risk overwriting
			if (ModelState.IsValid)
			{
				_context.SaveChanges();
				return Ok(updatePool);
			}

			return BadRequest();
		}
    }
}
