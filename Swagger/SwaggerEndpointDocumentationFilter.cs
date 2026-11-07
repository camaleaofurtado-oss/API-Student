using ApiStudent.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiStudent.Swagger;

public class SwaggerEndpointDocumentationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var action = context.MethodInfo.Name;
        var controller = context.MethodInfo.DeclaringType;

        if (controller == typeof(UserController))
            DocumentUser(operation, action);
        else if (controller == typeof(StudentController))
            DocumentStudent(operation, action);
        else if (controller == typeof(InstructorController))
            DocumentInstructor(operation, action);
        else if (controller == typeof(CourseController))
            DocumentCourse(operation, action);
        else if (controller == typeof(EnrollmentController))
            DocumentEnrollment(operation, action);
    }

    private static void DocumentUser(OpenApiOperation operation, string action)
    {
        switch (action)
        {
            case nameof(UserController.PostUser):
                Describe(operation,
                    "Cadastra um usuário do Identity",
                    """
                    Não exige token.

                    Campos obrigatórios: userName, email, password e rePassword. rePassword tem de ser igual a password.
                    A senha precisa de no mínimo 6 caracteres, com maiúscula, minúscula, número e um caractere especial.
                    E-mail e username não podem se repetir.

                    Exemplo de aluno:
                    { "userName": "maria.aluno", "email": "maria.aluno@email.com", "password": "Senha@123", "rePassword": "Senha@123" }

                    Exemplo de instrutor (segunda chamada):
                    { "userName": "joao.instrutor", "email": "joao.instrutor@email.com", "password": "Senha@123", "rePassword": "Senha@123" }

                    Sucesso: 201 com o usuário criado. Guarde o id para POST /User/Role.
                    """,
                    anonymous: true);
                Response(operation, "201", "Usuário criado.");
                Response(operation, "400", "Corpo inválido (e-mail, senha ou confirmação).");
                break;

            case nameof(UserController.Login):
                Describe(operation,
                    "Gera o token JWT",
                    """
                    Não exige token. Cole só o valor de token em Authorize, sem a palavra Bearer.
                    O token expira em 10 minutos.

                    SuperAdmin criado na subida da API:
                    { "email": "superAdmin@email.com", "password": "SenhaDoAdmin@123" }

                    Aluno (faça este login depois de POST /Student, para o token receber UserType = Student):
                    { "email": "maria.aluno@email.com", "password": "Senha@123" }

                    Instrutor (faça este login depois de POST /Instructor, para o token receber UserType = Instructor):
                    { "email": "joao.instrutor@email.com", "password": "Senha@123" }
                    """,
                    anonymous: true);
                Response(operation, "200", "Login aceito. A resposta traz success, message e token.");
                Response(operation, "400", "E-mail ou senha inválidos. Corpo: { \"message\": \"...\" }.");
                break;

            case nameof(UserController.ReadUsers):
                Describe(operation,
                    "Lista todos os usuários",
                    "Exige Admin ou SuperAdmin. Não envia parâmetros. Copie o id (GUID) do usuário que for receber uma role.");
                Response(operation, "200", "Lista com id, userName, email e roles.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(UserController.ReadUserPorId):
                Describe(operation,
                    "Busca um usuário pelo id",
                    "Exige Admin ou SuperAdmin. No parâmetro id cole o GUID retornado por GET /User. Exemplo: a1b2c3d4-e5f6-7890-abcd-ef1234567890.");
                DescribeParameter(operation, "id", "GUID do usuário, copiado de GET /User.");
                Response(operation, "200", "Usuário encontrado, com a lista de roles.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(UserController.PostRoleEmUser):
                Describe(operation,
                    "Adiciona roles a um usuário",
                    """
                    Exige a role SuperAdmin. Admin comum recebe 403.

                    Roles que existem: "Admin" e "SuperAdmin".
                    userId é o GUID de GET /User.

                    { "roles": ["Admin"], "userId": "COLE_O_GUID_DO_GET_USER" }

                    Sucesso: 200 com o texto "Roles adicionadas ao usuario!".
                    """);
                Response(operation, "200", "Roles vinculadas. Corpo em texto puro.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem a role SuperAdmin.");
                break;
        }
    }

    private static void DocumentStudent(OpenApiOperation operation, string action)
    {
        switch (action)
        {
            case nameof(StudentController.PostStudent):
                Describe(operation,
                    "Vincula um usuário como aluno",
                    """
                    Exige Admin ou SuperAdmin.
                    O e-mail tem de ser de um usuário já criado em POST /User/Cadastra.
                    Um usuário só pode ter um aluno.

                    { "email": "maria.aluno@email.com", "nome": "Maria Aluno" }

                    Sucesso: 201. Depois faça login de novo com esse e-mail para o token sair com UserType = Student.
                    """);
                Response(operation, "201", "Aluno criado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(StudentController.ReadStudents):
                Describe(operation,
                    "Lista os alunos",
                    "Exige Admin ou SuperAdmin. Não envia parâmetros. Alunos marcados como excluídos não aparecem.");
                Response(operation, "200", "Lista de alunos.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(StudentController.ReadStudentPorId):
                Describe(operation,
                    "Busca um aluno pelo e-mail",
                    """
                    Exige UserType = Student, ou role Admin, ou role SuperAdmin.

                    Parâmetro email: maria.aluno@email.com

                    Com o token do SuperAdmin qualquer e-mail de aluno funciona.
                    Com o token da Maria, o e-mail da URL tem de ser exatamente maria.aluno@email.com.
                    """);
                DescribeParameter(operation, "email", "E-mail do aluno. Exemplo: maria.aluno@email.com.");
                Response(operation, "200", "Aluno encontrado.");
                Response(operation, "404", "Aluno não encontrado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem permissão de aluno ou administrador.");
                break;

            case nameof(StudentController.PutStudent):
                Describe(operation,
                    "Atualiza o nome do aluno",
                    """
                    Exige Admin ou SuperAdmin.
                    O e-mail vai na URL, não no corpo. Só o nome é alterado.

                    URL: /Student/maria.aluno@email.com
                    { "nome": "Maria Aluno Silva" }

                    Sucesso: 200 com o texto "Usuario alterado".
                    """);
                DescribeParameter(operation, "email", "E-mail do aluno que será atualizado. Exemplo: maria.aluno@email.com.");
                Response(operation, "200", "Nome atualizado. Corpo em texto puro.");
                Response(operation, "404", "Aluno não encontrado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(StudentController.DeleteStudent):
                Describe(operation,
                    "Exclui o aluno de forma lógica",
                    """
                    Exige Admin ou SuperAdmin. Não envia corpo.
                    Marca IsDeleted = true. O registro deixa de aparecer nas consultas.

                    URL: /Student/maria.aluno@email.com

                    Deixe este passo por último se ainda for testar matrícula.
                    """);
                DescribeParameter(operation, "email", "E-mail do aluno. Exemplo: maria.aluno@email.com.");
                Response(operation, "204", "Aluno marcado como excluído. Resposta sem corpo.");
                Response(operation, "404", "Aluno não encontrado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;
        }
    }

    private static void DocumentInstructor(OpenApiOperation operation, string action)
    {
        switch (action)
        {
            case nameof(InstructorController.PostInstructor):
                Describe(operation,
                    "Vincula um usuário como instrutor",
                    """
                    Exige Admin ou SuperAdmin. O controller inteiro usa essa política.
                    O e-mail tem de ser de um usuário já criado, e esse usuário ainda não pode ser instrutor.

                    { "email": "joao.instrutor@email.com", "nome": "Joao Instrutor" }

                    Sucesso: 200 com o texto "Instrutor adicionado com sucesso".
                    Faça login de novo com esse e-mail para o token sair com UserType = Instructor.
                    """);
                Response(operation, "200", "Instrutor criado. Corpo em texto puro.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;

            case nameof(InstructorController.DeleteInstructor):
                Describe(operation,
                    "Remove o instrutor",
                    """
                    Exige Admin ou SuperAdmin. Não envia corpo.
                    Apaga o registro do banco.

                    URL: /Instructor/joao.instrutor@email.com

                    Faça isso só depois de testar a criação de curso com o token do João, se quiser esse fluxo.
                    """);
                DescribeParameter(operation, "email", "E-mail do instrutor. Exemplo: joao.instrutor@email.com.");
                Response(operation, "204", "Instrutor removido. Resposta sem corpo.");
                Response(operation, "404", "Instrutor não encontrado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;
        }
    }

    private static void DocumentCourse(OpenApiOperation operation, string action)
    {
        switch (action)
        {
            case nameof(CourseController.PostCourse):
                Describe(operation,
                    "Cria um curso",
                    """
                    Exige UserType = Instructor, ou role Admin, ou role SuperAdmin.
                    O SuperAdmin já pode chamar. Para testar o João, troque o token depois do login feito após POST /Instructor.

                    categoria é o número do enum:
                    0 Programacao, 1 Design, 2 Marketing, 3 Idiomas, 4 DataScience,
                    5 BancoDeDados, 6 SegurancaDaInformacao, 7 NegociosEEmpreendedorismo,
                    8 InteligenciaArtificial, 9 CloudComputing.
                    Valor fora de 0 a 9 devolve 400.

                    { "descricao": "Curso de C# e ASP.NET Core", "categoria": 0, "cargaHoraria": 40 }

                    Sucesso: 201. Anote o id. No primeiro curso ele costuma ser 1.
                    """);
                Response(operation, "201", "Curso criado.");
                Response(operation, "400", "Categoria inválida. Resposta sem corpo.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem permissão de instrutor ou administrador.");
                break;

            case nameof(CourseController.ReadCoursePorId):
                Describe(operation,
                    "Busca um curso pelo id",
                    "Não exige token. Parâmetro id: 1.",
                    anonymous: true);
                DescribeParameter(operation, "id", "Id numérico do curso. Exemplo: 1.");
                Response(operation, "200", "Curso encontrado.");
                Response(operation, "404", "Curso não encontrado.");
                break;

            case nameof(CourseController.ReadCourse):
                Describe(operation,
                    "Lista cursos com filtros opcionais",
                    """
                    Não exige token. Para listar tudo, deixe os três parâmetros vazios.

                    categoria: nome do enum, não o número. Exemplo: Programacao.
                    Nomes aceitos: Programacao, Design, Marketing, Idiomas, DataScience, BancoDeDados, SegurancaDaInformacao, NegociosEEmpreendedorismo, InteligenciaArtificial, CloudComputing.
                    Nome inexistente devolve 200 com lista vazia.

                    cargaMinima: cursos com carga maior ou igual. Exemplo: 20.
                    dataMinima: cursos criados a partir dessa data. Exemplo: 2026-01-01T00:00:00.
                    """,
                    anonymous: true);
                DescribeParameter(operation, "categoria", "Nome do enum. Exemplo: Programacao. Vazio lista todas as categorias.");
                DescribeParameter(operation, "cargaMinima", "Carga horária mínima, inclusive. Exemplo: 20.");
                DescribeParameter(operation, "dataMinima", "Data mínima de criação. Exemplo: 2026-01-01T00:00:00.");
                Response(operation, "200", "Lista de cursos, possivelmente vazia.");
                break;

            case nameof(CourseController.PutCourse):
                Describe(operation,
                    "Atualiza um curso",
                    """
                    Exige UserType = Instructor, ou role Admin, ou role SuperAdmin.
                    O id vai na URL. categoria continua sendo o número de 0 a 9.

                    URL: /Course/1
                    { "descricao": "Curso de C# e ASP.NET Core - atualizado", "categoria": 9, "cargaHoraria": 60 }

                    Sucesso: 200 com o texto "Curso com Id 1 atualizado com sucesso!".
                    """);
                DescribeParameter(operation, "id", "Id do curso. Exemplo: 1.");
                Response(operation, "200", "Curso atualizado. Corpo em texto puro.");
                Response(operation, "400", "Categoria inválida. Corpo em texto puro.");
                Response(operation, "404", "Curso não encontrado. Corpo em texto puro.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem permissão de instrutor ou administrador.");
                break;

            case nameof(CourseController.DeleteCourse):
                Describe(operation,
                    "Remove um curso",
                    """
                    Exige Admin ou SuperAdmin. Não envia corpo.
                    URL: /Course/1

                    Só apague depois da matrícula, porque a matrícula aponta para esse curso.
                    """);
                DescribeParameter(operation, "id", "Id do curso. Exemplo: 1.");
                Response(operation, "204", "Curso removido. Resposta sem corpo.");
                Response(operation, "404", "Curso não encontrado.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;
        }
    }

    private static void DocumentEnrollment(OpenApiOperation operation, string action)
    {
        switch (action)
        {
            case nameof(EnrollmentController.PostEnrollment):
                Describe(operation,
                    "Matricula um aluno em um curso",
                    """
                    Exige UserType = Student, ou role Admin, ou role SuperAdmin.
                    O aluno e o curso precisam existir. Não há atualização nem exclusão de matrícula.

                    Com o token do SuperAdmin, informe o e-mail do aluno:
                    { "studentEmail": "maria.aluno@email.com", "courseId": 1 }

                    Com o token da Maria (login feito depois de POST /Student), não envie studentEmail.
                    A API usa o claim id do token. Se o aluno mandar o e-mail, a requisição é recusada.
                    { "courseId": 1 }
                    """);
                Response(operation, "201", "Matrícula criada.");
                Response(operation, "400", "Regra de negócio recusada. Corpo em texto puro.");
                Response(operation, "404", "Aluno ou curso não encontrado. Corpo em texto puro.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem permissão de aluno ou administrador.");
                break;

            case nameof(EnrollmentController.ReadEnrollmentPorId):
                Describe(operation,
                    "Busca uma matrícula",
                    """
                    Exige Admin ou SuperAdmin.

                    studentEmail: maria.aluno@email.com
                    courseId: 1

                    URL resultante: /Enrollment/maria.aluno@email.com/1
                    """);
                DescribeParameter(operation, "studentEmail", "E-mail do aluno matriculado. Exemplo: maria.aluno@email.com.");
                DescribeParameter(operation, "courseId", "Id do curso. Exemplo: 1.");
                Response(operation, "200", "Matrícula encontrada.");
                Response(operation, "404", "Matrícula não encontrada.");
                Response(operation, "401", "Token ausente ou inválido.");
                Response(operation, "403", "Token sem role Admin ou SuperAdmin.");
                break;
        }
    }

    private static void Describe(OpenApiOperation operation, string summary, string description, bool anonymous = false)
    {
        operation.Summary = summary;
        operation.Description = description.Trim();
        if (anonymous)
            operation.Security = new List<OpenApiSecurityRequirement>();
    }

    private static void DescribeParameter(OpenApiOperation operation, string name, string description)
    {
        var parameter = operation.Parameters?.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (parameter != null)
            parameter.Description = description;
    }

    private static void Response(OpenApiOperation operation, string statusCode, string description)
    {
        if (operation.Responses.TryGetValue(statusCode, out var existing))
            existing.Description = description;
        else
            operation.Responses[statusCode] = new OpenApiResponse { Description = description };
    }
}
