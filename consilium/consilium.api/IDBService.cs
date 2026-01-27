using Consilium.Shared.Models;

namespace Consilium.API;

public interface IDBService {
    #region Todos
    public int AddToDo(TodoItem Todo, string email);
    IEnumerable<TodoItem> GetTodoList(string email);
    public void UpdateToDo(TodoItem Todo, string email);
    public void RemoveToDo(int id, string email);
    #endregion
    #region Assignments
    public IEnumerable<Assignment> GetIncompleteAssignments(string email);
    public IEnumerable<Assignment> GetAllAssignments(string email);
    public int AddAssignment(Assignment assignment, string email);
    public void UpdateAssignment(Assignment assignment, string email);
    public void DeleteAssignment(int id, string email);
    IEnumerable<Course> GetAllCourses(string username);
    void DeleteCourse(int id, string email);
    int AddCourse(Course course, string email);
    #endregion
    #region Messages
    IEnumerable<string> GetConversations(string username);
    IEnumerable<Message> GetMessages(string username, string otherUser);
    Task<string> AddMessage(Message message);
    bool CheckUser(string otherUser);
    #endregion
}