using Microsoft.AspNetCore.Mvc;

namespace PayTR.TestUI.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Success(string SiparisNo)
        {
            ViewBag.SiparisNo = SiparisNo;
            return View();
        }
        
        public IActionResult Fail(string SiparisNo)
        {
            ViewBag.SiparisNo = SiparisNo;
            return View();
        }
    }
}
