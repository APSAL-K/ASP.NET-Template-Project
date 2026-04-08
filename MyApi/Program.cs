using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApi.Common.Auth;
using MyApi.Common.Modules;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is required.");

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<DisabledModuleActionFilter>();
    })
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new ModuleControllerFeatureProvider(builder.Configuration));
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
builder.Services.AddSingleton<DisabledModuleActionFilter>();
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<DisabledModuleSwaggerDocumentFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddApplicationModules(builder.Configuration);

var app = builder.Build();

await app.InitializeApplicationModulesAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();         
    app.UseSwaggerUI();      
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();