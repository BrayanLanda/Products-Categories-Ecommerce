using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Data;
using ApiEcommerce.Mapping;
using ApiEcommerce.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddAutoMapper(cfg =>
    cfg.AddProfile<CategoryProfile>()
);
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Secret key is not configured.");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = true,
    };
})
;
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(cfg =>
    cfg.AddProfile<ProductProfile>()
);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseAuthorization();

app.MapControllers();

app.Run();
