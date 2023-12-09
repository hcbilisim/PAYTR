using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using PayTR.Business;
using PayTR.Entities;
using PayTR.Models;
using PayTR.TestUI.Models;
using System.Diagnostics;
using System.IO;

namespace PayTR.TestUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
           _logger = logger;
        }

        public IActionResult Index()
        {
            //*********************** PAYTR ÖRNEK ******************
            Random random = new Random();
            int minDeger = 1000000; // Minimum 7 haneli sayı
            int maxDeger = 9999999; // Maksimum 7 haneli sayı
            string SiparisNumarasi = random.Next(minDeger, maxDeger).ToString();

            Setting payTrSetting = new Setting()
            {
                merchant_id = "123456789",
                merchant_key = "XXXXXXXXXXXXXXX",
                merchant_salt = "XXXXXXXXXXXXXXX",
                merchant_ok_url =  $"{this.Request.Scheme}://{this.Request.Host}" + "/Payment/Success/?SiparisNo=" + SiparisNumarasi,
                merchant_fail_url =  $"{this.Request.Scheme}://{this.Request.Host}" + "/Payment/Fail/?SiparisNo=" + SiparisNumarasi,
                timeout_limit = "30",
                debug_on = "1",
                test_mode = "1",
                no_installment = "0",
                max_installment = "0",
                currency = "TL",
                lang = "tr"
            };

            Order order = new Order()
            {
                emailstr = "i@hcbilisim.com.tr",
                payment_amountstr = 1000, //10 Tl için *100 yapılacak.
                merchant_oid = SiparisNumarasi,
                user_namestr = "Ferit",
                user_addressstr = "Gezgil",
                user_phonestr = "55555555555",
                user_ip = HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString(),
            };

            List<BasketItem> products = new List<BasketItem>();
            products.Add(new BasketItem()
            {
                UrunAdi = "Deneme Ürün Adı",
                Fiyat = "10.00",
                Adet = 1
            });

            Manager paytrManager = new Manager();
            ResultIframe resultIframe = paytrManager.GetIframe(payTrSetting, order, products);
            //********************************************************

            return View(resultIframe);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
