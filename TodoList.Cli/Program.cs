using TodoList.Cli.Repositories;
using TodoList.Cli.Services;

var filePath = Path.Combine("data", "tasks.json");

ITodoRepository repository = new JsonTodoRepository(filePath);
var todoService = new TodoService(repository);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLowerInvariant();

try
{
    switch (command)
    {
        case "add":
        {
            if (args.Length < 2)
            {
                ShowError("Informe o título da tarefa.");
                Console.WriteLine("Uso: add <título>");
                return;
            }

            var title = string.Join(' ', args.Skip(1));
            var createdTask =
                await todoService.CriarTarefaAsync(title);

            Console.WriteLine(
                $"Tarefa #{createdTask.Id} criada: {createdTask.Titulo}");

            break;
        }

        case "edit":
        {
            if (args.Length < 3 ||
                !int.TryParse(args[1], out var taskId))
            {
                ShowError("Uso: edit <id> <novo título>");
                return;
            }

            var newTitle = string.Join(' ', args.Skip(2));
            var editedTask =
                await todoService.EditarAsync(taskId, newTitle);

            Console.WriteLine(
                $"Tarefa #{editedTask.Id} editada: {editedTask.Titulo}");

            break;
        }

        case "complete":
        {
            if (args.Length != 2 ||
                !int.TryParse(args[1], out var taskId))
            {
                ShowError("Uso: complete <id>");
                return;
            }

            var completedTask =
                await todoService.CompletarAsync(taskId);

            Console.WriteLine(
                $"Tarefa #{completedTask.Id} concluída: {completedTask.Titulo}");

            break;
        }

        case "list":
        {
            var tasks = await todoService.ListarAsync();

            if (tasks.Count == 0)
            {
                Console.WriteLine("Nenhuma tarefa cadastrada.");
                return;
            }

            foreach (var task in tasks)
            {
                var status = task.IsCompleted ? "[x]" : "[ ]";
                Console.WriteLine($"{task.Id} {status} {task.Titulo}");
            }

            break;
        }

        case "delete":
        {
            if (args.Length != 2 ||
                !int.TryParse(args[1], out var taskId))
            {
                ShowError("Uso: delete <id>");
                return;
            }

            await todoService.DeletarAsync(taskId);
            Console.WriteLine($"Tarefa #{taskId} excluída.");

            break;
        }

        case "help":
        case "--help":
        case "-h":
            ShowHelp();
            break;

        default:
            ShowError($"Comando desconhecido: {command}");
            ShowHelp();
            break;
    }
}
catch (ArgumentException exception)
{
    ShowError(exception.Message);
}
catch (InvalidOperationException exception)
{
    ShowError(exception.Message);
}
catch (InvalidDataException exception)
{
    ShowError(exception.Message);
}
catch (IOException exception)
{
    ShowError($"Não foi possível acessar os dados: {exception.Message}");
}
catch (UnauthorizedAccessException exception)
{
    ShowError($"Acesso negado aos dados: {exception.Message}");
}

static void ShowError(string message)
{
    Console.Error.WriteLine($"Erro: {message}");
    Environment.ExitCode = 1;
}

static void ShowHelp()
{
    Console.WriteLine("TodoList CLI");
    Console.WriteLine();
    Console.WriteLine("Comandos:");
    Console.WriteLine("  add <título>             Cria uma tarefa");
    Console.WriteLine("  list                     Lista as tarefas");
    Console.WriteLine("  complete <id>            Conclui uma tarefa");
    Console.WriteLine("  edit <id> <novo título>  Edita uma tarefa");
    Console.WriteLine("  delete <id>              Exclui uma tarefa");
    Console.WriteLine("  help                     Exibe esta ajuda");
}