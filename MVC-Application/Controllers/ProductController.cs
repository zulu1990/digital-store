using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC_Application.Models;

namespace MVC_Application.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _config;


        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        // GET: ProductController
        public async Task<ActionResult> Index()
        {
            var products = await $"{_config.GetConnectionString("OnlineStoreBaseUrl")}/api/products".GetJsonAsync<List<Product>>();
            var defaultImage = _config["defaultImage"].ToString();
            products.ForEach(x => x.Url ??= defaultImage);

            return View(products);
        }

        // GET: ProductController/Details/5
        public async Task<ActionResult> Details(int productIdentifier)
        {
            var baseUrl = _config.GetConnectionString("OnlineStoreBaseUrl");

            var product = await $"{baseUrl}/api/products/get-products-by-productIdentifier/{productIdentifier}"
                .GetJsonAsync<Product>();

            return View(product);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
