using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagementAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AreYouAliveController : ControllerBase
	{
		// GET
		[HttpGet]
		public string Get()
		{
			return "Hello world";
		}
	}
}
