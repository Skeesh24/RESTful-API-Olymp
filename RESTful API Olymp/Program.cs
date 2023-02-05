using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp;
using RESTful_API_Olymp.Domain;

var builder = WebApplication.CreateBuilder(args);


// �������� ����������� � �������������
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();


// ������� ���������� ��� ������ ����������
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
//    pattern: "{controller=Animals}/{action=Animals}/{animalid?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=registration}/");

app.Run();
