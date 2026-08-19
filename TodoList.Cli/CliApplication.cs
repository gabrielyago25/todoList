using TodoList.Core.Services;

namespace TodoList.Cli;

public sealed class CliApplication
{
    private readonly TodoService _todoService;

    public CliApplication(TodoService todoService)
    {
        _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
    }

    public async Task<int> RunAsync(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return 0;
        }

        var comando = args[0].ToLowerInvariant();

        try
        {
            return comando switch
            {
                "add" => await AddAsync(args),
                "list" => await ListAsync(args),
                "complete" => await CompleteAsync(args),
                "edit" => await EditAsync(args),
                "delete" => await DeleteAsync(args),
                "help" or "--help" or "-h" => ShowHelp(),
                _ => UnknownCommand(comando)
            };
        }
        catch (ArgumentException exception)
        {
            return ShowError(exception.Message, 2);
        }
        catch (InvalidOperationException exception)
        {
            return ShowError(exception.Message, 3);
        }
        catch (InvalidDataException exception)
        {
            return ShowError(exception.Message, 4);
        }
        catch (IOException exception)
        {
            return ShowError($"Não foi possível acessar os dados: {exception.Message}", 4);
        }
        catch (UnauthorizedAccessException exception)
        {
            return ShowError($"Acesso negado aos dados: {exception.Message}", 4);
        }
    }

    private async Task<int> AddAsync(string[] args)
    {
        if (args.Length < 2)
        {
            return ShowError("Uso: add <título>", 2);
        }

        var title = string.Join(' ', args.Skip(1));
        var task = await _todoService.CriarTarefaAsync(title);

        Console.WriteLine($"Tarefa #{task.Id} criada: {task.Titulo}");

        return 0;
    }

    private async Task<int> ListAsync(string[] args)
    {
        if (args.Length != 1)
        {
            return ShowError("Uso: list", 2);
        }

        var tasks = await _todoService.ListarAsync();

        if (tasks.Count == 0)
        {
            Console.WriteLine("Nenhuma tarefa cadastrada.");
            return 0;
        }

        foreach (var task in tasks)
        {
            var status = task.IsCompleted ? "[x]" : "[ ]";
            Console.WriteLine($"{task.Id} {status} {task.Titulo}");
        }

        return 0;
    }

    private async Task<int> CompleteAsync(string[] args)
    {
        if (!TryReadId(args, "complete <id>", out var id))
        {
            return 2;
        }

        var task = await _todoService.CompletarAsync(id);

        Console.WriteLine($"Tarefa #{task.Id} concluída: {task.Titulo}");

        return 0;
    }

    private async Task<int> EditAsync(string[] args)
    {
        if (args.Length < 3 || !int.TryParse(args[1], out var id))
        {
            return ShowError("Uso: edit <id> <novo título>", 2);
        }

        var newTitle = string.Join(' ', args.Skip(2));
        var task = await _todoService.EditarAsync(id, newTitle);

        Console.WriteLine($"Tarefa #{task.Id} editada: {task.Titulo}");

        return 0;
    }

    private async Task<int> DeleteAsync(string[] args)
    {
        if (!TryReadId(args, "delete <id>", out var id))
        {
            return 2;
        }

        await _todoService.DeletarAsync(id);

        Console.WriteLine($"Tarefa #{id} excluída.");
        return 0;
    }

    private static bool TryReadId(
        string[] args,
        string usage,
        out int id)
    {
        id = 0;

        if (args.Length != 2 || !int.TryParse(args[1], out id))
        {
            ShowError($"Uso: {usage}", 2);
            return false;
        }

        return true;
    }

    private static int UnknownCommand(string command)
    {
        ShowError($"Comando desconhecido: {command}", 2);
        ShowHelp();

        return 2;
    }

    private static int ShowError(string message, int exitCode)
    {
        Console.Error.WriteLine($"Erro: {message}");
        return exitCode;
    }

    private static int ShowHelp()
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

        return 0;
    }
}