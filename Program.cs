using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using ApiStudent.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define a descrição do esquema de segurança (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo.\n\nExemplo: eyJhbGciOiJIUzI1Ni..."
    });

    // Aplica a exigência do token globalmente no Swagger
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();

var connString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<Context>(opts => opts.UseLazyLoadingProxies().UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddIdentity<User, IdentityRole>(opts=> opts.User.RequireUniqueEmail = true).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("IsStudent", policy =>
         policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "Student") ||
            context.User.IsInRole("Admin") ||
            context.User.IsInRole("SuperAdmin")
         ));
    opts.AddPolicy("IsInstructor", policy =>
         policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "Instructor") ||
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("SuperAdmin")
         ));
    opts.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.IsInRole("SuperAdmin")

    ));
}
);
  


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiStudent v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
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


