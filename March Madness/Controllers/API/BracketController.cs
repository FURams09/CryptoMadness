using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;

namespace March_Madness.Controllers.API
{
    public class BracketController : ApiController
    {

		[Route("api/bracket")]
		[HttpPost]
		public IHttpActionResult SaveTournament(List<List<List<int>>> bracket)
		{
			try
			{
				var utl = new Utility();
				var _bracket = utl.GetBracket();
				for (int i = 0; i < bracket.Count; i++)
				{
					var round = i;

					for (var j = 0; j < bracket[i].Count; j++)
					{
						var game = bracket[i][j];
						if (!(game[0] == 1 ^ game[1] == 1) || game.Count > 2)
						{
							return BadRequest(); //This game doesn't have only 1 winner
						}
						else
						{

						}
					}
				}



			}
			catch
			{
				return NotFound();
			}
			return Ok();
		}
	}
}
