using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApolloStage.Data;
using ApolloStage.Models;
using ApolloStage.Services;
using ApolloStage.Factories.IFactories;

using ApolloStage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

builder.Services.AddControllersWithViews();

// Add the line below to configure the Razor Pages service
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews(options =>
{
}).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
    }).AddJsonOptions(options =>
    {

        options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IFactories, ApolloStage.Factories.Factories>();
builder.Services.AddTransient<ISingleton, Singleton>();
builder.Services.AddTransient<IMusicService, SpotifyService>();

builder.Services.AddTransient<IHttpClientHelper, HttpCLientHelper>();

builder.Services.AddScoped<HttpCLientHelper>();




builder.Services.AddAuthentication().AddGoogle("google", options =>
{
    options.ClientId = "138536774146-o1tjo5q46jl3n183v6jlnji31a7m5qqv.apps.googleusercontent.com";
    options.ClientSecret = "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";
  //  options.CallbackPath = "/Account/ExternalLoginCallback";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapRazorPages();

app.Run();

