using COTS1.Class;
using COTS1.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TestNhiemVuContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=LAPTOP-R74JRM89\\HUY;Initial Catalog=TestNhiemVu;Integrated Security=True;Trust Server Certificate=True"));
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của session
    options.Cookie.HttpOnly = true; // Chỉ sử dụng Session qua HTTP, không qua JavaScript
    options.Cookie.IsEssential = true; // Bắt buộc cho Session
});
/*builder.Services.AddHttpClient();*/
builder.Services.AddHttpContextAccessor();
/*builder.Services.AddScoped<GetMails>();*/
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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
