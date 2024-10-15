using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string con = builder.Configuration["ConnectionStrings:Default"]!;

builder.Services.AddDbContext<StoreDbContext>(opts => 
 {
     opts.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
 });

// this maps the instantiation of the IStoreRepository interface to the EFStoreRepository class
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// add handling of Razor Pages
builder.Services.AddRazorPages();

// add handling of memory only session data -
// sets up a memory store
builder.Services.AddDistributedMemoryCache();

// so we can use Session objects (httpContext.ISession)
builder.Services.AddSession();

// following is defining a Service Proviser (sp) for the Cart class which will be
// injected into the CartModel class (constructor) to simplyfy access to session data.
// So, we define what method to call when we want to 'get' a Cart object
// the method is the GetCart() in SessionCart
// When we inject the 'Cart' type into a class we no longer need use 
// the code to get and set session data as it is now automatically called
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// apply requests to the session data
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// original
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

// user riendly - for pagination (name/pattern/defaults)

// put "Products/Page{productPage} BEFORE {category}/Page{productPage:int}
// so that the Controller Action will 'see' Products/Page and NOT
// 'Products' as the value for {category} ! Otherwise, if {category}/Page{productPage:int} was first the action method would ALWAYS see whatever comes after http://localhost:5000/<IN HERE> as the value for category

app.MapControllerRoute("order",
    "Order",
    new { Controller = "Order", action = "Checkout" });

app.MapControllerRoute("pagination",
    "Products",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("pagination",
    "Products/Page{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("catpage",
    "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });

app.MapControllerRoute("page", "Page{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });

app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });


// enusre we can handle razor pages endpoints
app.MapRazorPages();

// ensure we handle blazor
app.MapBlazorHub();
app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

SeedData.EnsurePopulated(app);

app.Run();
