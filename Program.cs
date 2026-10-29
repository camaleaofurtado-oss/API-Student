using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using ApiStudent.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<Context>(opts => opts.UseLazyLoadingProxies().UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(("sua-chave-gigante-com-mais-de-trinta-e-dois-caracteres"))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
}
);
  


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("SuperAdmin"))
    {
        await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
    }

    var adminEmail = "superAdmin@email.com";
    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new User
        {
            UserName = "SuperAdminSeeder",
            Email = adminEmail,
        };
        string senha = "SenhaDoAdmin@123";
        await userManager.CreateAsync(user, senha);

    }

    if (!await userManager.IsInRoleAsync(user, "SuperAdmin"))
    {
        await userManager.AddToRoleAsync(user, "SuperAdmin");
    }

}
app.Run();


