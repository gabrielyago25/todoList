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

switch (command)
{
    case "add":
        if (args.Length < 2)
        {
            Console.WriteLine("Informe o título da tarefa.");
            Console.WriteLine("Uso: add <título>");
            return;
        }

        var title = string.Join(' ', args.Skip(1));
        var createdTask = await todoService.CriarTarefaAsync(title);

        Console.WriteLine(
            $"Tarefa #{createdTask.Id} criada: {createdTask.Titulo}");
        break;

    case "edit":
        if (args.Length < 3 || !int.TryParse(args[1], out var editTarefaId))
        {
            Console.WriteLine("Uso: edit <id> <novo título>");
            return;
        }

        var novoTitulo = string.Join(' ', args.Skip(2));

        try
        {
            var tarefaEditada = await todoService.EditarAsync(editTarefaId, novoTitulo);
            Console.WriteLine($"Tarefa #{tarefaEditada.Id} editada: {tarefaEditada.Titulo}");
        } catch (ArgumentException exception)
        {
            Console.WriteLine($"Erro: {exception.Message}");
        } catch (InvalidOperationException exception)
        {
            Console.WriteLine($"Erro: {exception.Message}");
        }

        break;

    case "complete":
        if (args.Length != 2 ||
            !int.TryParse(args[1], out var taskId))
        {
            Console.WriteLine("Uso: complete <id>");
            return;
        }

        try
        {
            var completedTask =
                await todoService.CompletarAsync(taskId);

            Console.WriteLine(
                $"Tarefa #{completedTask.Id} concluída: {completedTask.Titulo}");
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"Erro: {exception.Message}");
        }
        break;

    case "list":
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

    default:
        Console.WriteLine($"Comando desconhecido: {command}");
        ShowHelp();
        break;
}

static void ShowHelp()
{
    Console.WriteLine("TodoList CLI");
    Console.WriteLine();
    Console.WriteLine("Comandos:");
    Console.WriteLine("  add <título>  Cria uma tarefa");
    Console.WriteLine("  list          Lista as tarefas");
    Console.WriteLine("  complete <id> Marca uma tarefa como concluída");
    Console.WriteLine("  edit <id> <novo título>  Edita o título de uma tarefa existente");
}