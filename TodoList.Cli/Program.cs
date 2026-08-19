using TodoList.Cli;
using TodoList.Core.Repositories;
using TodoList.Core.Services;
using TodoList.Infrastructure.Persistence;

var filePath = Path.Combine("data", "tasks.json");

ITodoRepository repository =
    new JsonTodoRepository(filePath);

var todoService = new TodoService(repository);
var application = new CliApplication(todoService);

return await application.RunAsync(args);