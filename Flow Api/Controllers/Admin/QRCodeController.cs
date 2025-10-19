using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Admin
{
    public class QRCodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
