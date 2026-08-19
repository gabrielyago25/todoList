using TodoList.Core.Models;

namespace TodoList.Core.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoTask>> GetAllAsync();
    Task<TodoTask?> GetByIdAsync(int id);
    Task AddAsync(TodoTask task);
    Task UpdateAsync(TodoTask task);
    Task<bool> DeleteAsync(int id);
}
