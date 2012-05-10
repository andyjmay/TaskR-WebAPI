using SignalR.Hubs;

namespace TaskR.Hubs {
  [HubName("TaskHub")]
  public class TaskHub : Hub {
    [HubMethodName("Authenticate")]
    public void Login(string username) {
      Groups.Add(Context.ConnectionId, username);
    }
  }
}