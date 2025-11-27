using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToughService.Models;
using ToughService.Data;
using ToughService.Repository;
using ToughService.Services;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// Serviços base
// ------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// ------------------------------------
// Swagger 
// ------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------
// Sessão
// ------------------------------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ------------------------------------
// Repositórios
// ------------------------------------
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>(); // Repositório antigo (compatibilidade)
builder.Services.AddScoped<IProdutoRepositoryGeneric, ProdutoRepositoryGeneric>(); // Novo repositório genérico
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddScoped<ICheckoutViewModelBuilder, CheckoutViewModelBuilder>();
builder.Services.AddHttpClient<ICaptchaService, RecaptchaService>();

// ------------------------------------
// Serviços
// ------------------------------------
builder.Services.AddScoped<ISkuService, SkuService>();

// ------------------------------------
// Banco de dados
// ------------------------------------
string mySqlConnection = Environment.GetEnvironmentVariable("MYSQL_CONNECTION");

builder.Services.AddDbContext<BancoContext>(opt =>
    opt.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

// ------------------------------------
// Identity (Configura o esquema de Cookie e define o LoginPath)
// ------------------------------------
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BancoContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Registro/Login";
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("ClientId do Google não configurado.");
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
            ?? throw new InvalidOperationException("ClientSecret do Google não configurado.");

        options.CallbackPath = "/signin-google";
        options.SignInScheme = IdentityConstants.ExternalScheme;

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SaveTokens = true;
    });


var app = builder.Build();

// ------------------------------------
// Criar usuário admin (seed)
// ------------------------------------
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
        logger.LogError(ex, "Erro ao popular o banco de dados!");
    }
}

// ------------------------------------
// Middlewares
// ------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Essencial
app.UseAuthorization();  // Essencial (e depois de UseAuthentication)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ------------------------------------
// Seed do Admin
// ------------------------------------
async Task SeedRolesAndAdminUser(UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IConfiguration configuration)
{
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine(">>> Papel Admin criado.");
    }

    string adminEmail = configuration["AdminUser:Email"];
    string adminPassword = configuration["AdminUser:Password"];

    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
    {
        Console.WriteLine(">>> Email ou senha do admin não configurados.");
        return;
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        Console.WriteLine(">>> Criando usuário admin...");
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (!result.Succeeded)
        {
            Console.WriteLine("Erro ao criar admin:");
            foreach (var err in result.Errors)
                Console.WriteLine($"- {err.Code}: {err.Description}");
            return;
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
        Console.WriteLine(">>> Admin adicionado ao papel Admin.");
    }
}