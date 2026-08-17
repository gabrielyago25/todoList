namespace TodoList.Cli.Models;

public sealed class TodoTask {
    public int Id {get; set;}
    public required string Titulo {get; set;}
    public bool IsCompleted {get; set;}
    public DateTimeOffset CriadoEm {get; set;}
    public DateTimeOffset? ConcluidoEm {get; set;}

}