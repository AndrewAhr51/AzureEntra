using EntraIdBL.Interfaces;
using EntraIdBL.Services;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var kvUri = new Uri("https://benchscrumkeyvault.vault.azure.net/");
//var client = new SecretClient(vaultUri: kvUri, credential: new DefaultAzureCredential());// Add services to the container.
//KeyVaultSecret tenantIdSecret = client.GetSecret("TenantId");
//KeyVaultSecret clientIdSecret = client.GetSecret("ClientId");
//KeyVaultSecret clientSecretSecret = client.GetSecret("ClientSecret");

//string tenantId = tenantIdSecret.Value;
//string clientId = clientIdSecret.Value;
//string clientSecret = clientSecretSecret.Value;

//Console.WriteLine($"Tenant ID: {tenantId}");
//Console.WriteLine($"Client ID: {clientId}");
//Console.WriteLine($"Client Secret: {clientSecret}");

//Get the connection string from appsettings.json
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
builder.Services.AddSingleton<IConfiguration>(configuration);
var azureAd = configuration.GetSection("AzureAd");
var clientId = azureAd["ClientId"];
var tenantId = azureAd["TenantId"];
var clientSecret = azureAd["ClientSecret"];

//Get the SharePoint Sites names from appsettings.json 
var sharePointSites = configuration.GetSection("SharePoint");
var sharePointBaseUrl = sharePointSites["BaseUrl"];
var sharePointProd = sharePointSites["Production"];
var sharePointDev = sharePointSites["Development"];
var sharePointListName = sharePointSites["List"];
builder.Services.AddTransient<IConnectionServices>(s => new ConnectionServices(sharePointBaseUrl, sharePointDev,  sharePointListName));

// Add services to the container.
builder.Services.AddScoped<IGraphClient>(s => new GraphClient(clientId, tenantId, clientSecret, sharePointBaseUrl));
var confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .WithClientSecret(clientSecret)
            .Build();

builder.Services.AddScoped<IRegionalMembers>(s => new RegionalMembers(clientId, tenantId, clientSecret, sharePointBaseUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();