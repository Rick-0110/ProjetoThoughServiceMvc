using Microsoft.EntityFrameworkCore;
using ToughService.Data;

var builder = WebApplication.CreateBuilder(args);

// Registra o IHttpContextAccessor para usar na view
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

// Habilitar cache em memória para sessão
builder.Services.AddDistributedMemoryCache();

// Adicionar serviço de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Sua conexão e DbContext
string mySqlConnection = builder.Configuration.GetConnectionString("DefaultDatabase");
builder.Services.AddDbContext<BancoContext>(opt => {
    opt.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
