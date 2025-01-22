using ImageUploadingRetreiving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageUploadingRetreiving.Controllers
{
    public class ProductController : Controller
    {
        private readonly imageDbContext context;
        private readonly IWebHostEnvironment env;

        public ProductController(imageDbContext context,IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }
        public IActionResult Index()
        {
            var data = context.Products.ToList();
            return View(data);
            
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel prod)
        {
            string filename = "";
            if (prod.Photo != null)
            {
                var ext = Path.GetExtension(prod.Photo.FileName);
                var size = prod.Photo.Length;

                if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".jpeg"))
                {
                    if (size <= 1000000)
                    {
                        string folder = Path.Combine(env.WebRootPath, "images");
                        filename = Guid.NewGuid().ToString() + "_" + prod.Photo.FileName;
                        string filePath = Path.Combine(folder, filename);
                        prod.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                        Product p = new Product()
                        {
                            Name = prod.Name,
                            Price = prod.Price,
                            ImagePath = filename,
                        };
                        context.Products.Add(p);
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Size error"] = "only 1mb image allowed";
                    }
                }
                else
                {
                    TempData["Ext error"] = "Only JPG,JPEG and PNG allowed";
                }
            }
            return View();
        }
    }
}
