using ApiStudent.Models.Dtos.CourseDto;
using ApiStudent.Models.Dtos.EnrollmentDto;
using ApiStudent.Models.Dtos.InstructorDto;
using ApiStudent.Models.Dtos.StudentDto;
using ApiStudent.Models.Dtos.UserDto;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiStudent.Swagger;

public class DtoExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateUserDto))
        {
            schema.Example = Json(
                ("userName", "maria.aluno"),
                ("email", "maria.aluno@email.com"),
                ("password", "Senha@123"),
                ("rePassword", "Senha@123"));
            Note(schema, "userName", "Obrigatório. Não pode se repetir.");
            Note(schema, "email", "Obrigatório e único. Exemplo de aluno: maria.aluno@email.com. Exemplo de instrutor: joao.instrutor@email.com.");
            Note(schema, "password", "Obrigatória. Mínimo de 6 caracteres, com maiúscula, minúscula, número e caractere especial. Exemplo: Senha@123.");
            Note(schema, "rePassword", "Obrigatória e igual a password.");
        }
        else if (context.Type == typeof(LoginUserDto))
        {
            schema.Example = Json(
                ("email", "superAdmin@email.com"),
                ("password", "SenhaDoAdmin@123"));
            Note(schema, "email", "superAdmin@email.com, ou o e-mail cadastrado em POST /User/Cadastra.");
            Note(schema, "password", "SenhaDoAdmin@123 para o SuperAdmin. Para os usuários de teste, Senha@123.");
        }
        else if (context.Type == typeof(AddUserRoleDto))
        {
            schema.Example = new OpenApiObject
            {
                ["roles"] = new OpenApiArray { new OpenApiString("Admin") },
                ["userId"] = new OpenApiString("COLE_O_GUID_DO_GET_USER")
            };
            Note(schema, "roles", "Lista com Admin ou SuperAdmin. Outros nomes são recusados.");
            Note(schema, "userId", "GUID copiado de GET /User.");
        }
        else if (context.Type == typeof(CreateStudentDto))
        {
            schema.Example = Json(
                ("email", "maria.aluno@email.com"),
                ("nome", "Maria Aluno"));
            Note(schema, "email", "E-mail de um usuário já cadastrado.");
            Note(schema, "nome", "Nome do aluno.");
        }
        else if (context.Type == typeof(UpdateStudentDto))
        {
            schema.Example = Json(("nome", "Maria Aluno Silva"));
            Note(schema, "nome", "Novo nome. O e-mail fica na URL.");
        }
        else if (context.Type == typeof(CreateInstructorDto))
        {
            schema.Example = Json(
                ("email", "joao.instrutor@email.com"),
                ("nome", "Joao Instrutor"));
            Note(schema, "email", "E-mail de um usuário já cadastrado e que ainda não é instrutor.");
            Note(schema, "nome", "Nome do instrutor.");
        }
        else if (context.Type == typeof(CreateCourseDto) || context.Type == typeof(UpdateCourseDto))
        {
            schema.Example = new OpenApiObject
            {
                ["descricao"] = new OpenApiString(context.Type == typeof(CreateCourseDto)
                    ? "Curso de C# e ASP.NET Core"
                    : "Curso de C# e ASP.NET Core - atualizado"),
                ["categoria"] = new OpenApiInteger(context.Type == typeof(CreateCourseDto) ? 0 : 9),
                ["cargaHoraria"] = new OpenApiInteger(context.Type == typeof(CreateCourseDto) ? 40 : 60)
            };
            Note(schema, "descricao", "Texto do curso.");
            Note(schema, "categoria", "Número de 0 a 9: 0 Programacao, 1 Design, 2 Marketing, 3 Idiomas, 4 DataScience, 5 BancoDeDados, 6 SegurancaDaInformacao, 7 NegociosEEmpreendedorismo, 8 InteligenciaArtificial, 9 CloudComputing.");
            Note(schema, "cargaHoraria", "Inteiro maior ou igual a zero. Exemplo de criação: 40. Exemplo de atualização: 60.");
        }
        else if (context.Type == typeof(CreateEnrollmentDto))
        {
            schema.Example = new OpenApiObject
            {
                ["studentEmail"] = new OpenApiString("maria.aluno@email.com"),
                ["courseId"] = new OpenApiInteger(1)
            };
            Note(schema, "studentEmail", "Obrigatório para Admin e SuperAdmin. O aluno autenticado deve omitir este campo.");
            Note(schema, "courseId", "Id do curso existente. Exemplo: 1.");
        }
    }

    private static OpenApiObject Json(params (string Name, string Value)[] fields)
    {
        var example = new OpenApiObject();
        foreach (var (name, value) in fields)
            example[name] = new OpenApiString(value);
        return example;
    }

    private static void Note(OpenApiSchema schema, string propertyName, string description)
    {
        if (schema.Properties.TryGetValue(propertyName, out var property))
        {
            property.Description = description;
            return;
        }

        var pascal = char.ToUpperInvariant(propertyName[0]) + propertyName[1..];
        if (schema.Properties.TryGetValue(pascal, out property))
            property.Description = description;
    }
}
