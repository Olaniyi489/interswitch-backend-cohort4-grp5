using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Principal;
using blacklist.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.PushService(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
// services .AddSessionStateTempDataProvider();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(24);//We set Time here
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

ConfigureLogs();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    //Add global filters
    //options.Filters.Add(new ApiExceptionFilter());

    options.Filters.Add<LanguageFilter>();

}).AddMvcOptions(options =>
{
    options.Filters.Add(
    new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None

    });
    options.Filters.Add<LanguageFilter>();
    //options.Filters.Add<SessionFilter>();
}
        ).AddNewtonsoftJson()
           .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
           });

builder.Services.AddConfigurations(builder.Configuration);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["AzureAd:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
        microsoftOptions.CallbackPath = "/Home/Landing";
        microsoftOptions.SaveTokens = true;
        microsoftOptions.AccessDeniedPath = "/AccessDenied/Index";

        //microsoftOptions.ToJToken();
        microsoftOptions.Events.OnCreatingTicket = ctx =>
        {
            List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

            tokens.Add(new AuthenticationToken()
            {
                Name = "TicketCreated",
                Value = DateTime.UtcNow.ToString()
            });

            ctx.Properties.StoreTokens(tokens);

            return Task.CompletedTask;
        };



    })//for test purpose
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
     .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwagger();
//builder.Services.AddSwaggerConfig();

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var logFactory = app.Services.GetRequiredService<ILoggerFactory>();
await app.SeedSuperAdmin();



var config = app.Services.GetService<IConfiguration>() ?? throw new ArgumentNullException("config");
var env = app.Services.GetService<IHostEnvironment>() ?? throw new ArgumentNullException("env");
SwaggerOptionsHelper.SwaggerOptionsHelperConfifure(config);
//FileHelper.ReFillMessageBundle(env, config)?.GetAwaiter().GetResult();


app.UseMiddleware<ExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{}
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BLACK-LIST V1");
    });


//app.UseHsts();
//app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseCors("AllowedPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Use(async (context, next) =>
{
    //context.Response.Headers.Add("language", new[] { "en" });
    // context.Request.Headers.Add("language", new[] { "en" });

    if (context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
    {
        // Read headers from Azure
        var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
        var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];

        #region extract claims via call /.auth/me
        //invoke /.auth/me
        var cookieContainer = new CookieContainer();
        HttpClientHandler handler = new HttpClientHandler()
        {
            CookieContainer = cookieContainer
        };
        string uriString = $"{context.Request.Scheme}://{context.Request.Host}";
        foreach (var c in context.Request.Cookies)
        {
            cookieContainer.Add(new Uri(uriString), new Cookie(c.Key, c.Value));
        }
        string jsonResult = string.Empty;
        using (HttpClient client = new HttpClient(handler))
        {
            var res = await client.GetAsync($"{uriString}/.auth/me");
            jsonResult = await res.Content.ReadAsStringAsync();
        }

        //parse json
        var obj = JArray.Parse(jsonResult);
        string user_id = obj[0]["user_id"].Value<string>(); //user_id

        // Create claims id
        List<Claim> claims = new List<Claim>();
        foreach (var claim in obj[0]["user_claims"])
        {
            claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
        }

        // Set user in current context as claims principal
        var identity = new GenericIdentity(azureAppServicePrincipalIdHeader);
        identity.AddClaims(claims);
        #endregion

        // Set current thread user to identity
        context.User = new GenericPrincipal(identity, null);
    };
    await next.Invoke();
});
app.Run();

void ConfigureLogs()
{


    //get configuration files

    IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .Build();
    //get the environment which the app is running on
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    if (env is null)
    {
        env = configuration.GetValue<string>("Env:Environment");
    }

    //Logger

    Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .Enrich.WithExceptionDetails()//add exception details
          .WriteTo.Debug()
          .WriteTo.Console()
          // .WriteTo.File($"{builder.Environment.ContentRootPath}{Path.DirectorySeparatorChar}ServiceLogs/vat-sop-", rollingInterval: RollingInterval.Day)
          .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)

          .CreateLogger();




}

async void CreateUser(IConfiguration config, UserManager<ApplicationUser> userManager)
{
    var email = config.GetValue<string>("DefaultLogin:Email");
    var password = config.GetValue<string>("DefaultLogin:Password");

    var userExists = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();

    if (userExists == null)
    {
        var person = new ApplicationUser
        {
            Email = email,
            UserName = email,
            DateCreated = DateTime.Now,

        };
    }


}