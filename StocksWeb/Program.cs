var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication("cookies")
.AddCookie("cookies", options =>
{
    options.Cookie.Name = "user";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/user/Login";
    options.AccessDeniedPath = "/user/Login";
    options.LogoutPath = "/";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsLoggedIn", policy => policy.RequireClaim("Id"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
