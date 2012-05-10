using System.Collections.Generic;
using SignalR.Hubs;

namespace TaskR.Hubs {
  [HubName("TaskHub")]
  public class TaskHub : Hub {
    [HubMethodName("Authenticate")]
    public void Login(string username) {
      Groups.Add(Context.ConnectionId, username);
    }

    public void AddedTask(TaskR.Models.Task task) {
      Clients[task.AssignedTo].TaskAdded(task);
    }

    //public void UpdatedTask(TaskR.Models.Task originalTask, TaskR.Models.Task updatedTask) {
    //  string originallyAssignedTo = originalTask.AssignedTo;
    //  string nowAssignedTo = updatedTask.AssignedTo;

    //  if (originallyAssignedTo != nowAssignedTo) {
    //    Clients[originallyAssignedTo].TaskUpdated(updatedTask);
    //  }
    //  Clients[nowAssignedTo].TaskUpdated(updatedTask);
    //}
  }
}