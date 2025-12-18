using System.Runtime.Serialization;
using Services;
using Services.Interfaces;
using DbRepos;
using Configuration.Extensions;
using DbContext.Extensions;
using Encryption.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//adding support for several secret sources and database sources
builder.Configuration.AddSecrets(builder.Environment);

//use encryption and multiple Database connections and their respective DbContexts
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddEnvironmentInfo();
builder.Services.AddUserBasedDbContext();

builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<FriendsDbRepos>();
builder.Services.AddScoped<IFriendsService, FriendsServiceDb>();
builder.Services.AddScoped<AddressesDbRepos>();
builder.Services.AddScoped<IAddressesService, AddressesServiceDb>();
builder.Services.AddScoped<PetsDbRepos>();
builder.Services.AddScoped<IPetsService, PetsServiceDb>();
builder.Services.AddScoped<QuotesDbRepos>();
builder.Services.AddScoped<IQuotesService, QuotesServiceDb>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();