using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToughService.Data;
using ToughService.Repository;
using ToughService.Services;
using ToughService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddHttpClient<ICaptchaService, RecaptchaService>();
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();

string mySqlConnection = Environment.GetEnvironmentVariable("MYSQL_CONNECTION");

builder.Services.AddDbContext<BancoContext>(opt =>
    opt.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BancoContext>();

var app = builder.Build();

//Inicializar o banco de dados e criar o usuário Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();


        await SeedRolesAndAdminUser(userManager, roleManager, configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu ao popular o banco de dados!!!");
    }
}

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
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


//método auxiliar para cirar o admin roles

async Task SeedRolesAndAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
{
    // Etapa 1: Cria o papel "Admin"
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine(">>> Papel 'Admin' criado com sucesso.");
    }

    // Etapa 2: Pega os dados da configuração
    string adminEmail = configuration["AdminUser:Email"];
    string adminPassword = configuration["AdminUser:Password"];

    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
    {
        Console.WriteLine(">>> AVISO: Email ou senha do administrador não configurados.");
        return;
    }

    // Etapa 3: Verifica se o usuário já existe
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        Console.WriteLine($">>> Usuário admin '{adminEmail}' não encontrado. Tentando criar...");
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        // Tenta criar o usuário e captura o resultado
        IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);

        // Se a criação falhou, imprime os erros detalhados no console
        if (!result.Succeeded)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("!!!   ERRO AO CRIAR USUÁRIO ADMIN   !!!");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- CÓDIGO: {error.Code}, DESCRIÇÃO: {error.Description}");
            }
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }
        Console.WriteLine($">>> Usuário admin '{adminEmail}' criado com sucesso.");
    }

    // Etapa 4: Adiciona o usuário ao papel "Admin"
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
        Console.WriteLine($">>> Usuário admin '{adminEmail}' adicionado ao papel 'Admin'.");
    }
}