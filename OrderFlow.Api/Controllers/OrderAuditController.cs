using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Api.Controllers
{
    public class OrderAuditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
