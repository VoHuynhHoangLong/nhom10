using SNKRS.Models;
using SNKRS.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using System;

namespace SNKRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            List<Product> upcoming = db.Products.Where(x => x.isVisible).OrderBy(p => p.Id).OrderByDescending(p => p.Id).Take(4).ToList();
            var best_selling = db.Products.SqlQuery("select P.* from Products P inner join ( select P.Id, Count(P.Id) as Count from OrderDetails OD inner join ProductSizes PS on OD.ProductSizeId = PS.Id inner join Products P on PS.ProductId = P.Id where P.isVisible = 'true' group by P.Id ) A on P.Id = A.Id order by A.Count desc").Take(4).ToList<Product>();
            HomeViewModel model = new HomeViewModel();
            model.Upcoming = upcoming;
            model.Best_selling = best_selling;
            model.OnSale = db.Products.Where(x => x.SalePrice > 0).Take(8).ToList();
            return View(model);
        }
        
        

        public ActionResult About()
        {
            ViewBag.Message = "Về chúng tôi";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Liên lạc.";

            return View();
        }
        public ActionResult SendEmail(string name, string email, string subject, string message)
        {
            string receiverEmail = "quangsus1114@gmail.com";

            // Tạo đối tượng MailMessage để xây dựng email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email);
            mail.To.Add(receiverEmail);
            mail.Subject = subject;
            mail.Body = $"Name: {name}<br>Email: {email}<br>Message: {message}";
            mail.IsBodyHtml = true;

            // Cấu hình SMTP để gửi email
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("yikesdoancoso@gmail.com", "ltwfmuyctdyxwsho");

            try
            {
                // Gửi email
                smtpClient.Send(mail);

                // Gửi email thành công, chuyển hướng người dùng đến một trang thành công hoặc hiển thị thông báo thành công
                TempData["SuccessMessage"] = "Email has been sent successfully.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu gửi email không thành công, chẳng hạn hiển thị thông báo lỗi hoặc chuyển hướng đến trang lỗi
                TempData["ErrorMessage"] = "Failed to send email. Please try again later.";
                return RedirectToAction("Contact");
            }
        }
    }
}