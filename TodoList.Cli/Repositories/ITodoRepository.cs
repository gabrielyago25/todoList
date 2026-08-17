using TodoList.Cli.Models;

namespace TodoList.Cli.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoTask>> GetAllAsync();
    Task<TodoTask?> GetByIdAsync(int id);
    Task AddAsync(TodoTask task);
    Task UpdateAsync(TodoTask task);
    Task<bool> DeleteAsync(int id);
}
