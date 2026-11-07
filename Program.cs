using ApiStudent.Data;
using ApiStudent.Models;
using ApiStudent.Models.Dtos.UserDto;
using ApiStudent.Service;
using ApiStudent.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.NetworkInformation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiStudent",
        Version = "v1",
        Description = """
            Gestão de usuários, alunos, instrutores, cursos e matrículas.

            ## Como autorizar
            1. Chame POST /User/Login. O SuperAdmin da aplicação é superAdmin@email.com / SenhaDoAdmin@123.
            2. Copie só o valor de token (começa com eyJ). Não copie a palavra Bearer.
            3. Clique em Authorize, cole o token e confirme.
            O token expira em 10 minutos. POST /User/Cadastra, POST /User/Login, GET /Course e GET /Course/{id} não exigem token.

            ## Ordem sugerida
            1. Login do SuperAdmin e Authorize.
            2. POST /User/Cadastra para o aluno (maria.aluno@email.com) e para o instrutor (joao.instrutor@email.com). Senha dos dois: Senha@123.
            3. GET /User e copie os ids.
            4. POST /Student e POST /Instructor com esses e-mails.
            5. POST /Course e anote o id.
            6. POST /Enrollment.
            7. Consultas, alterações e exclusões.

            Cada operação abaixo traz o JSON, os parâmetros e quem pode chamar.
            """
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole apenas o JWT retornado por POST /User/Login, sem o prefixo Bearer."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    options.OperationFilter<SwaggerEndpointDocumentationFilter>();
    options.SchemaFilter<DtoExampleSchemaFilter>();
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); ;

var connString = builder.Configuration["ConnectionStrings:Connection"];
builder.Services.AddDbContext<Context>(opts => opts.UseLazyLoadingProxies().UseMySql(connString, ServerVersion.AutoDetect(connString)));
builder.Services.AddIdentity<User, IdentityRole>(opts=> opts.User.RequireUniqueEmail = true).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"]!)),
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


