using System;
using Microsoft.AspNetCore.Mvc;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("/Product")]
 
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult Products()
		{
			return View();
		}
	}
}

