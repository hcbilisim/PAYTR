

using Microsoft.AspNetCore.Mvc;
using PayTR.Entities;
using System.Security.Cryptography;
using System.Text;

//TODO: CallBack Url'yi hazırladıktan sonra Paytr Paneli üzerinden callback url'nizi bildirin.
namespace PayTR.TestUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PayTrController : ControllerBase
    {
        [HttpPost]
        public ContentResult callback(object sender)
        {

            Setting setting = new Setting(); //PAYTR AYAR BİLGİLERİNİ DOLDUR.

            // POST değerleri ile hash oluştur.
            string merchant_oid = Request.Form["merchant_oid"]; //Order ID
            string status = Request.Form["status"];
            string total_amount = Request.Form["total_amount"];
            string hash = Request.Form["hash"];

            string Birlestir = string.Concat(merchant_oid, setting.merchant_salt, status, total_amount);
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(setting.merchant_key));
            byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
            string token = Convert.ToBase64String(b);

            if (hash.ToString() != token)
            {
                return Content("PAYTR notification failed: bad hash");
            }

            //###########################################################################
            // BURADA YAPILMASI GEREKENLER
            // 1) Siparişin durumunu $post['merchant_oid'] değerini kullanarak veri tabanınızdan sorgulayın.
            // 2) Eğer sipariş zaten daha önceden onaylandıysa veya iptal edildiyse  echo "OK"; exit; yaparak sonlandırın.
            if (status == "success")
            { 
                //Ödeme Onaylandı
                // Bildirimin alındığını PayTR sistemine bildir.  
                return Content("OK");
                // BURADA YAPILMASI GEREKENLER ONAY İŞLEMLERİDİR.
                // 1) Siparişi onaylayın.
                // 2) iframe çağırma adımında merchant_oid ve diğer bilgileri veri tabanınıza kayıp edip bu aşamada karşılaştırarak eğer var ise bilgieri çekebilir ve otomatik sipariş tamamlama işlemleri yaptırabilirsiniz.
                // 2) Eğer müşterinize mesaj / SMS / e-posta gibi bilgilendirme yapacaksanız bu aşamada yapabilirsiniz. Bu işlemide yine iframe çağırma adımında merchant_oid bilgisini kayıt edip bu aşamada sorgulayarak verilere ulaşabilirsiniz.
                // 3) 1. ADIM'da gönderilen payment_amount sipariş tutarı taksitli alışveriş yapılması durumunda
                // değişebilir. Güncel tutarı Request.Form['total_amount'] değerinden alarak muhasebe işlemlerinizde kullanabilirsiniz
            }
            else
            { 
                //Ödemeye Onay Verilmedi
                // Bildirimin alındığını PayTR sistemine bildir.  
                return Content("Ups!");
                // BURADA YAPILMASI GEREKENLER
                // 1) Siparişi iptal edin.
                // 2) Eğer ödemenin onaylanmama sebebini kayıt edecekseniz aşağıdaki değerleri kullanabilirsiniz.
                // $post['failed_reason_code'] - başarısız hata kodu
                // $post['failed_reason_msg']  - başarısız hata mesajı
            }
        }
    }
}
