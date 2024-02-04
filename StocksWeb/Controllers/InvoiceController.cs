using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using Newtonsoft.Json;
using StocksWeb.Enums;
using StocksWeb.Infrastructure;
using StocksWeb.Models;
using StocksWeb.ViewModels;
using System;

namespace StocksWeb.Controllers
{
    [Authorize(Policy = "IsLoggedIn")]
    public class InvoiceController : Controller
    {
        private IConfiguration _config;
        public InvoiceController(IConfiguration configuration)
        {
            _config = configuration;
        }
        // GET: InvoiceController
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> Index()
        {
            List<InvoiceDataViewModel> invoices = new();
            Response<List<InvoiceDataViewModel>> invoicesDataViewModel = new Response<List<InvoiceDataViewModel>>();
            invoicesDataViewModel = await GetInvoicesData();
            if (invoicesDataViewModel != null)
            {
                if (invoicesDataViewModel.IsSucceded)
                {
                    if (invoicesDataViewModel.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        invoices = invoicesDataViewModel.Data;
                    else
                        ViewData["RetvalMsg"] = invoicesDataViewModel.Errors!.First();
                }
                else
                    ViewData["RetvalMsg"] = invoicesDataViewModel.Errors!.First();
            }
            else
                ViewData["RetvalMsg"] = "Error while retreiving data.";
            return View(invoices);
        }

        public async Task<Response<List<InvoiceDataViewModel>>> GetInvoicesData()
        {
            string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Invoice/GetAllInvoices";
            string token = HttpContext.User.FindFirst("token")?.Value.ToString();
            Response<List<InvoiceDataViewModel>>? response = await Helper.GetAPIAsync<List<InvoiceDataViewModel>>(queryString, token);
            return response;
        }

        [Authorize(Policy ="Seller")]
        // GET: InvoiceController/Create
        public async Task<ActionResult> Create()
        {
            InvoiceCreateViewModel invoiceCreateViewModel = new();
            invoiceCreateViewModel.InvoiceNo=ObjectId.GenerateNewId().ToString();
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
            ViewBag.products = productsList;
            
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
            ViewBag.units = unitsList;
            //invoiceCreateViewModel.items=new List<InvoiceItemsCreateDTO>();

            return View(invoiceCreateViewModel);
        }

        // POST: InvoiceController/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InvoiceCreateViewModel invoiceCreateViewModel)
        {
            try
            {
                // Invoice Object
                InvoiceObjectViewModel invoiceobjectViewModel=new InvoiceObjectViewModel();
                invoiceobjectViewModel.Net=invoiceCreateViewModel.totalNet;
                invoiceobjectViewModel.UserId=Convert.ToInt32(HttpContext.User.FindFirst("Id")?.Value.ToString());
                invoiceobjectViewModel.InvoiceDate=invoiceCreateViewModel.InvoiceDate;
                invoiceobjectViewModel.TotalDiscount=invoiceCreateViewModel.TotalDiscount;
                invoiceobjectViewModel.Total=invoiceCreateViewModel.totalTotal;
                invoiceobjectViewModel.Taxes=invoiceCreateViewModel.Taxes;
                invoiceobjectViewModel.InvoiceNo=invoiceCreateViewModel.InvoiceNo;
                invoiceobjectViewModel.TotalItems=invoiceCreateViewModel.productList.Count;
                
                List<InvoiceItemsCreateViewModel> invoiceItems=new List<InvoiceItemsCreateViewModel>();
                for (int i = 0; i < invoiceCreateViewModel.productList.Count; i++)
                {
                    InvoiceItemsCreateViewModel item = new InvoiceItemsCreateViewModel();
                    item.Total=invoiceCreateViewModel.total[i];
                    item.Net=invoiceCreateViewModel.net[i];
                    item.Price=invoiceCreateViewModel.price[i];
                    item.Quantity=invoiceCreateViewModel.quantity[i];
                    item.Discount=invoiceCreateViewModel.discount[i];
                    item.StoreProductId=await GetStoreProduct(Convert.ToInt32(invoiceCreateViewModel.StoreId), invoiceCreateViewModel.productList[i], invoiceCreateViewModel.unitList[i]);
                    invoiceItems.Add(item);
                }
                invoiceobjectViewModel.items=invoiceItems;
                string invoiceJson = JsonConvert.SerializeObject(invoiceobjectViewModel);
                string encInvoiceJson = EncryptionHelper.EncryptString(invoiceJson, _config.GetValue<string>("Pass"));
                string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Invoice/CreateInvoice";
                string token = HttpContext.User.FindFirst("token")?.Value.ToString();
                Response<bool>? response = await Helper.PostAPIAsync<string>(queryString, encInvoiceJson, token);
                
                return RedirectToAction(nameof(Create));
            }
            catch
            {
                return View();
            }
        }

        public async Task<int> GetStoreProduct(int storeId, int productId, int unitId)
        {
            int storeProductId =0;
            string encStoreId = EncryptionHelper.EncryptString(storeId.ToString(), _config.GetValue<string>("Pass"));
            string encProductId = EncryptionHelper.EncryptString(productId.ToString(), _config.GetValue<string>("Pass"));
            string encUnitId = EncryptionHelper.EncryptString(unitId.ToString(), _config.GetValue<string>("Pass"));
            StoreProductSearchViewModel unitSearchObj = new StoreProductSearchViewModel { StoreId = encStoreId, ProductId=encProductId,UnitId=encUnitId };
            string unitSearchJson = JsonConvert.SerializeObject(unitSearchObj);
            string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Store/GetStoreProducts";
            string token = HttpContext.User.FindFirst("token")?.Value.ToString();
            Response<string>? response = await Helper.GetAPIAsync<string>(queryString, token, unitSearchJson);
            if (response != null)
            {
                if (response.IsSucceded)
                {
                    if (response.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        storeProductId = Convert.ToInt32(response.Data);
                    else
                        ViewData["RetvalMsg"] = response.Errors!.First().ErrorMessage;
                }
                else
                    ViewData["RetvalMsg"] = response.Errors!.First().ErrorMessage;
            }
            else
                ViewData["RetvalMsg"] = "General error, Please try again later";
            return storeProductId;
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
        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> CheckQuantity([FromBody] CheckQuantityViewModel request)
        {
            int errorsCount=0;
            bool result=true;
            string returnMsg="";
            string encStoreId = EncryptionHelper.EncryptString(request.StoreId.ToString(), _config.GetValue<string>("Pass"));
            string encProductId = "";
            string encUnitId = "";
            for (int i = 0; i < request.Data.Count; i++)
            {
                int quantity = request.Data[i].Quantity;
                encProductId=EncryptionHelper.EncryptString(request.Data[i].ProductId.ToString(), _config.GetValue<string>("Pass"));
                encUnitId=EncryptionHelper.EncryptString(request.Data[i].UnitId.ToString(), _config.GetValue<string>("Pass"));
                ProductQuantitySearchViewModel productSearchObj = new ProductQuantitySearchViewModel
                {
                    StoreId = encStoreId,
                    ProductId=encProductId,
                    UnitId=encUnitId
                };
                string productSearchJson = JsonConvert.SerializeObject(productSearchObj);
                string queryString = _config.GetValue<string>("ServiceAPIBaseUrl") + $"Product/GetProductQuantity";
                string token = HttpContext.User.FindFirst("token")?.Value.ToString();
                Response<ProductQuantityListViewModel> response = await Helper.GetAPIAsync<ProductQuantityListViewModel>(queryString, token, productSearchJson);
                if (response!=null)
                {
                    if (response.IsSucceded)
                    {
                        if (response.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        {
                            int productQuantity = response.Data.Quantity;
                            if (productQuantity<quantity)
                            {
                                errorsCount++;
                                returnMsg+=$"  {response.Data.ProductName} available quantity is less than the required";
                            }
                        }
                        else
                        {
                            errorsCount++;
                            returnMsg+=" Error during checking the data";
                        }
                    }
                    else
                    {
                        errorsCount++;
                        returnMsg+=" Error during checking the data";
                    }
                }
                else
                {
                    errorsCount++;
                    returnMsg+=" Error during checking the data";
                }
            }
            if (errorsCount>0)
                result=false;
            var responseObject = new
            {
                Success = result,
                Message = returnMsg
            };
            return Json(responseObject);
        }

    }
}
