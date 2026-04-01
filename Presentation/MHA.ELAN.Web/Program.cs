using MHA.ELAN.Business;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using MHA.ELAN.Web.Components;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Temporary for development error dislay in console log
builder.Services.Configure<CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

//// Register Entity Framework Core with SQL Server
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("Default") ??
//        throw new InvalidOperationException(
//            "Connection string 'BlazorDemoDB' not found.")));

// Register Microsoft Identity
IEnumerable<string>? initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
.EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
.AddInMemoryTokenCaches();

// Register PnP Core services and authentication
builder.Services.AddPnPCore();
builder.Services.AddPnPCoreAuthentication();

// Register the default HttpClient
builder.Services.AddHttpContextAccessor();

// Register ElmahCore service
builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("elmah-express");
});

// Register the JSONAppSettings configuration
builder.Services.Configure<JSONAppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<JSONAppSettings>(builder.Configuration.GetSection("ConnectionStrings"));

// Register our Custom services
ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Add interactive server components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Add ELMAH middleware 
//app.UseElmah();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    //Radzen Dialog
    services.AddScoped<DialogService>();
    services.AddScoped<TempMessageService>();
    services.AddScoped<WorkflowStateService>();

    // Constant 
    services.AddScoped<ConstantHelper>();

    // Helpers
    services.AddScoped<ConnectionStringHelper>();
    services.AddScoped<DateTimeHelper>();
    services.AddScoped<EmailHelper>();
    services.AddScoped<LogHelper>();
    services.AddScoped<NavigationHelper>();
    services.AddScoped<PeoplePickerHelper>();
    services.AddScoped<ProjectHelper>();
    services.AddScoped<SharePointHelper>();
    services.AddScoped<SQLSortingHelper>();
    services.AddScoped<TokenHelper>();
    services.AddScoped<WorkflowHelper>();

    // Business Layer
    services.AddScoped<AdministrationBL>();
    services.AddScoped<ApprovalFormBL>();
    services.AddScoped<EmailTemplateBL>();
    services.AddScoped<EmployeeBL>();
    services.AddScoped<HomeBL>();
    services.AddScoped<ReportBL>();
    services.AddScoped<RequestBL>();
    services.AddScoped<SearchBL>();
    services.AddScoped<WorkflowBL>();

    // Data Layer
    services.AddScoped<EmployeeDA>();
    services.AddScoped<HomeDA>();
    services.AddScoped<RequestDA>();
    services.AddScoped<WorkflowDA>();

}