using EPM.Mouser.Interview.Data;
using Microsoft.AspNetCore.Mvc;

namespace EPM.Mouser.Interview.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly WarehouseApi _warehouseApi;
        public HomeController (IWarehouseRepository warehouseRepository )
        {
            _warehouseApi = new WarehouseApi(warehouseRepository);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var products = await _warehouseApi.GetPublicInStockProducts();
            return View(products);
        }
       
    }
}
