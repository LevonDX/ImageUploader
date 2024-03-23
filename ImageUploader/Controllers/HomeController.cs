using ImageUploader.Models;
using ImageUploader.Services.Abstract;
using ImageUploader.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ImageUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IImageSaver _imageSaver;

        public HomeController(ILogger<HomeController> logger,
            IImageSaver imageSaver)
        {
            _logger = logger;
            _imageSaver = imageSaver;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);

           string sas = await _imageSaver.SaveImageAsync(ms, file.FileName, "images");

            return RedirectToAction("ShowImage", sas);
        }

        [HttpGet]
        public IActionResult ShowImage(string sas)
        {
            return View(new ShowImageViewModel { Sas = sas });
        }
    }
}
