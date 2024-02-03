using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StocksWeb.Enums;
using StocksWeb.Infrastructure;
using StocksWeb.ViewModels;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace StocksWeb.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration _config;
        public UserController(IConfiguration configuration)
        {
            _config = configuration;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                Response<LoginResponseViewModel> userDataViewModel = await GetUserData(user);
                if (userDataViewModel != null)
                {
                    if (userDataViewModel.IsSucceded)
                    {
                        if (userDataViewModel.ResponseCode == (int)ResponseCodesEnum.SuccessWithData)
                        {
                            LoginResponseViewModel userData = userDataViewModel.Data;
                            var claims = new List<Claim>
                            {
                                new ("Id",userData.UserId.ToString() ),
                                new ("Name",userData.DisplayName ),
                                new (ClaimTypes.Role, userData.RoleName),
                                new ("token",userData.Token ),
                            };
                            var cI = new ClaimsIdentity(claims, "pwd", "Name", "Admin");
                            var cP = new ClaimsPrincipal(cI);
                            await HttpContext.SignInAsync(cP);
                            return RedirectToAction("Index", "Home"); ;
                        }
                        else
                            ViewData["Error"]="Invalid username or password";
                    }
                    else
                        ViewData["Error"]="Invalid username or password";
                }
                ViewData["Error"] = "Invalid username or password";
            }
            return View(user);
        }

        [Authorize(Policy = "IsLoggedIn")]
        public ActionResult LogOut()
        {
            HttpContext.Session.Remove("loggedinuser");
            return RedirectToAction("Login");
        }


        
        public async Task<Response<LoginResponseViewModel>> GetUserData(LoginViewModel loginViewModel)
        {
            string encUserName = EncryptionHelper.EncryptString(loginViewModel.UserName.ToString(), _config.GetValue<string>("Pass"));
            string encPassword = EncryptionHelper.EncryptString(loginViewModel.Password.ToString(), _config.GetValue<string>("Pass"));
            LoginViewModel EncObject = new LoginViewModel { UserName = encUserName, Password=encPassword };
            string EncObjectJson = JsonConvert.SerializeObject(EncObject);
            var client = new HttpClient();
            client.BaseAddress = new Uri(_config.GetValue<string>("ServiceAPIBaseUrl") + "User/UserLogin");
            string apiResponse = "";
            Response<LoginResponseViewModel> responseS = new Response<LoginResponseViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                httpClient.DefaultRequestHeaders.Add("sendData", EncObjectJson);
                using (HttpResponseMessage result = await httpClient.GetAsync(client.BaseAddress.AbsoluteUri))
                {
                    apiResponse = await result.Content.ReadAsStringAsync();
                    responseS = JsonConvert.DeserializeObject<Response<LoginResponseViewModel>>(apiResponse);
                }
            }

            return responseS;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
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

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
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

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
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
