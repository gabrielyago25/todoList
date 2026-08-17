using System.Text.Json;
using TodoList.Cli.Models;

namespace TodoList.Cli.Repositories;

public sealed class JsonTodoRepository : ITodoRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly string _filePath;
    public JsonTodoRepository (string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }
    public async Task<IReadOnlyList<TodoTask>> GetAllAsync()
    {
        return await LoadAsync();
    }
    public async Task<TodoTask?> GetByIdAsync(int id)
    {
        var tasks = await LoadAsync();
        return tasks.SingleOrDefault(task => task.Id == id);
    }
    public async Task AddAsync(TodoTask task)
    {
        var tasks = await LoadAsync();

        if (tasks.Any(existingTask => existingTask.Id == task.Id))
        {
            throw new InvalidOperationException(
                $"ERRO: Já existe uma tarefa com o ID {task.Id}.");
        }

        tasks.Add(task);
        await SaveAsync(tasks);
    }
    public async Task UpdateAsync(TodoTask task)
    {
        var tasks = await LoadAsync();
        var index = tasks.FindIndex(
            existingTask => existingTask.Id == task.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"ERRO: A tarefa de ID {task.Id} não existe.");
        }

        tasks[index] = task;
        await SaveAsync(tasks);
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var tasks = await LoadAsync();
        var removed = tasks.RemoveAll(task => task.Id == id) > 0;

        if (removed)
        {
            await SaveAsync(tasks);
        }

        return removed;
    }
    private async Task<List<TodoTask>> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            await using var stream = File.OpenRead(_filePath);

            return await JsonSerializer.DeserializeAsync<List<TodoTask>>(
                stream,
                JsonOptions) ?? [];
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"ERRO: O arquivo contém um JSON inválido: {_filePath}",
                exception);
        }
    }
    private async Task SaveAsync(IReadOnlyCollection<TodoTask> tasks)
    {
        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, tasks, JsonOptions);
    }
}