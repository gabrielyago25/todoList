using TodoList.Cli.Models;
using TodoList.Cli.Repositories;

namespace TodoList.Cli.Services;

public sealed class TodoService
{
    private readonly ITodoRepository _repository;
    public TodoService (ITodoRepository repository)
    {
        _repository = repository;
    }

// CRIAÇÃO: Método para criar uma nova tarefa.
    public async Task<TodoTask> CriarTarefaAsync(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
         throw new ArgumentException("O título da tarefa não pode estar vazio.", nameof(titulo));   
        }
        var tarefas = await _repository.GetAllAsync();
        var proximoId = tarefas.Count == 0 ? 1 : tarefas.Max(task => task.Id) + 1;
        var tarefa = new TodoTask
        {
            Id = proximoId,
            Titulo = titulo.Trim()
        };
        await _repository.AddAsync(tarefa);
        return tarefa; 
    }

// EDIÇÃO: Método para editar o título de uma tarefa existente.
    public async Task<TodoTask> EditarAsync(int id, string novoTitulo)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O ID deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(novoTitulo))
        {
            throw new ArgumentException("O novo título não pode estar vazio.", nameof(novoTitulo));
        }

        var tarefa = await _repository.GetByIdAsync(id);

        if (tarefa is null)
        {
            throw new InvalidOperationException($"A tarefa de ID {id} não foi encontrada.");
        }

        tarefa.Titulo = novoTitulo.Trim();
        await _repository.UpdateAsync(tarefa);

        return tarefa;
    }

// CONCLUSÃO: Método para marcar uma tarefa como concluída.
    public async Task<TodoTask> CompletarAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id),"O ID deve ser maior que zero.");
        }

        var tarefa = await _repository.GetByIdAsync(id);

        if (tarefa is null)
        {
            throw new InvalidOperationException($"A tarefa de ID {id} não foi encontrada.");
        }

        if (tarefa.IsCompleted)
        {
            return tarefa;
        }

        tarefa.IsCompleted = true;
        tarefa.ConcluidoEm = DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(tarefa);

        return tarefa;
    }

    public async Task DeleteAsync (int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O ID deve ser maior que zero.");
        }

        var foiRemovida = await _repository.DeleteAsync(id);
        if (!foiRemovida)
        {
            throw new InvalidOperationException($"A tarefa de ID {id} não foi encontrada.");
        }
    }
    public Task<IReadOnlyList<TodoTask>> ListarAsync()
    {
        return _repository.GetAllAsync();
    }
}
