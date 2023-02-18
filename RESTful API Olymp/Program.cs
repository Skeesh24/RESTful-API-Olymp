using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Middlewares;

var builder = WebApplication.CreateBuilder(args);


// включаем контроллеры и представления
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();


// хэндлер исключений для режима разработки
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/"
//);

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Animal}/{action=Animal}/{animalId:long?}/types/{typeId:long?}"
);

app.Run();
