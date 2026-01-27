using Consilium.Shared.Models;

namespace Consilium.Shared.Services;

public interface IPersistenceService {
    IEnumerable<TodoItem>? GetToDoLists();
    void SaveList(IEnumerable<TodoItem> list);
    void SaveToken(string email, string token);
    void DeleteToken();
    void OnStartup();
    string GetUserName();
    bool CheckLocalLoginStatus();
    string GetNotes();
    void SaveNotes(string content);
    string GetTheme();
    void SaveTheme(string theme);
}