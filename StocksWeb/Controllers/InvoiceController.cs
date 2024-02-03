using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using StocksWeb.Enums;
using StocksWeb.Infrastructure;
using StocksWeb.Models;
using StocksWeb.ViewModels;

namespace StocksWeb.Controllers
{
    //[Authorize(Policy = "IsLoggedIn")]
    public class InvoiceController : Controller
    {
        private IConfiguration _config;
        public InvoiceController(IConfiguration configuration)
        {
            _config = configuration;
        }
        // GET: InvoiceController
        public ActionResult Index()
        {
            return View();
        }

        // GET: InvoiceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InvoiceController/Create
        public async Task<ActionResult> Create()
        {
            InvoiceCreateViewModel invoiceCreateViewModel = new();

            // Stores List
            List<StoreListViewModel> storesList = new();
            Response<List<StoreListViewModel>> storesListModel = new Response<List<StoreListViewModel>>();
            storesListModel = await GetStoresData();
            if (storesListModel != null)
            {
                if (storesListModel.IsSucceded)
                {
                    if (storesListModel.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        storesList = storesListModel.Data;
                    else
                        ViewData["RetvalMsg"] = storesListModel.Errors!.First().ErrorMessage;
                }
                else
                    ViewData["RetvalMsg"] = storesListModel.Errors!.First().ErrorMessage;
            }
            else
                ViewData["RetvalMsg"] = "General error, Please try again later";

            ViewData["storeList"] = new SelectList(storesList, "StoreId", "StoreName");

            // Products List
            List<ProductsListViewModel> productsList = new();
            Response<List<ProductsListViewModel>> productsListModel = new Response<List<ProductsListViewModel>>();
            productsListModel = await GetProductsData(storesList.FirstOrDefault().StoreId);
            if (productsListModel != null)
            {
                if (productsListModel.IsSucceded)
                {
                    if (productsListModel.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        productsList = productsListModel.Data;
                    else
                        ViewData["RetvalMsg"] = productsListModel.Errors!.First().ErrorMessage;
                }
                else
                    ViewData["RetvalMsg"] = productsListModel.Errors!.First().ErrorMessage;
            }
            else
                ViewData["RetvalMsg"] = "General error, Please try again later";

            ViewData["productList"] = new SelectList(productsList, "ProductId", "ProductName");

            // Units List
            List<UnitsListViewModel> unitsList = new();
            Response<List<UnitsListViewModel>> unitsListModel = new Response<List<UnitsListViewModel>>();
            unitsListModel = await GetUnitsData(storesList.FirstOrDefault().StoreId, productsList.FirstOrDefault().ProductId);
            if (unitsListModel != null)
            {
                if (unitsListModel.IsSucceded)
                {
                    if (unitsListModel.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        unitsList = unitsListModel.Data;
                    else
                        ViewData["RetvalMsg"] = unitsListModel.Errors!.First().ErrorMessage;
                }
                else
                    ViewData["RetvalMsg"] = unitsListModel.Errors!.First().ErrorMessage;
            }
            else
                ViewData["RetvalMsg"] = "General error, Please try again later";

            ViewData["unitList"] = new SelectList(unitsList, "UnitId", "UnitName");
            
            invoiceCreateViewModel.items=new List<InvoiceItemsCreateDTO>();

            return View(invoiceCreateViewModel);
        }

        // POST: InvoiceController/Create
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


        // AJAX Request
        [HttpPost]
        public async Task<JsonResult> LoadProducts(int storeId)
        {
            Response<List<ProductsListViewModel>> productsData = new Response<List<ProductsListViewModel>>();
            productsData = await GetProductsData(storeId);
            if (productsData != null)
            {
                if (productsData.IsSucceded)
                {
                    if (productsData.ResponseCode != (int)ResponseCodesEnum.SuccessWithData)
                        ViewData["RetvalMsg"] = productsData.Errors!.First();
                }
                else
                {
                    ViewData["RetvalMsg"] = productsData.Errors!.First();
                }
            }
            else
            {
                ViewData["RetvalMsg"] = "General Error.";
            }
            List<SelectListItem> products = new();
            if (productsData.Data != null)
            {
                products = (from item in productsData.Data
                           select new SelectListItem
                           {
                               Value = item.ProductId.ToString(),
                               Text = item.ProductName
                           }).ToList();
                
            }
            return new JsonResult(products);
        }
        
        // AJAX Request
        [HttpPost]
        public async Task<JsonResult> LoadUnits(int storeId,int productId)
        {
            Response<List<UnitsListViewModel>> unitsData = new Response<List<UnitsListViewModel>>();
            unitsData = await GetUnitsData(storeId,productId);
            if (unitsData != null)
            {
                if (unitsData.IsSucceded)
                {
                    if (unitsData.ResponseCode != (int)ResponseCodesEnum.SuccessWithData)
                        ViewData["RetvalMsg"] = unitsData.Errors!.First();
                }
                else
                {
                    ViewData["RetvalMsg"] = unitsData.Errors!.First();
                }
            }
            else
            {
                ViewData["RetvalMsg"] = "General Error.";
            }
            List<SelectListItem> units = new();
            if (unitsData.Data != null)
            {
                units = (from item in unitsData.Data
                           select new SelectListItem
                           {
                               Value = item.UnitId.ToString(),
                               Text = item.UnitName
                           }).ToList();
                
            }
            return new JsonResult(units);
        }

        public async Task<Response<List<StoreListViewModel>>> GetStoresData()
        {
            string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Store/GetAllStores";
            string token = HttpContext.User.FindFirst("token")?.Value.ToString();
            Response<List<StoreListViewModel>>? response = await Helper.GetAPIAsync<List<StoreListViewModel>>(queryString, token);
            return response;
        }

        public async Task<Response<List<ProductsListViewModel>>> GetProductsData(int storeId)
        {
            string encStoreId = EncryptionHelper.EncryptString(storeId.ToString(), _config.GetValue<string>("Pass"));
            ProductSearchViewModel productSearchObj = new ProductSearchViewModel { StoreId = encStoreId };
            string productSearchJson = JsonConvert.SerializeObject(productSearchObj);
            string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Product/GetProducts";
            string token = HttpContext.User.FindFirst("token")?.Value.ToString();
            Response<List<ProductsListViewModel>> response = await Helper.GetAPIAsync<List<ProductsListViewModel>>(queryString, token, productSearchJson);
            return response;
        }

        public async Task<Response<List<UnitsListViewModel>>> GetUnitsData(int storeId, int productId)
        {
            string encStoreId = EncryptionHelper.EncryptString(storeId.ToString(), _config.GetValue<string>("Pass"));
            string encProductId = EncryptionHelper.EncryptString(productId.ToString(), _config.GetValue<string>("Pass"));
            UnitSearchViewModel unitSearchObj = new UnitSearchViewModel { StoreId = encStoreId,ProductId=encProductId };
            string unitSearchJson = JsonConvert.SerializeObject(unitSearchObj);
            string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Unit/GetUnits";
            string token = HttpContext.User.FindFirst("token")?.Value.ToString();
            Response<List<UnitsListViewModel>>? response = await Helper.GetAPIAsync<List<UnitsListViewModel>>(queryString, token, unitSearchJson);
            return response;
        }

        // GET: InvoiceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InvoiceController/Edit/5
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

        // GET: InvoiceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InvoiceController/Delete/5
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
