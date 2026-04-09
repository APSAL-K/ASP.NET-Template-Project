using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModuleDrivenFramwork.Common.Auth;
using ModuleDrivenFramwork.Common.Modules;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is required.");
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://127.0.0.1:5173"];

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplicationModules(builder.Configuration);

var app = builder.Build();

await app.InitializeApplicationModulesAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();         
    app.UseSwaggerUI();      
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();