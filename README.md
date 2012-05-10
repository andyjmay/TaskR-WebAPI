#TaskR-WebAPI


Demo Application of TaskR "Pre-SignalR"

##Walkthrough

###Initial Setup
Clone the repo

```
git clone https://github.com/andyjmay/TaskR-WebAPI.git
```

Build and run the project.

Open the site in two browser windows.

Log into the site using the **same username** in both windows.

Create a new Task in one window, it should NOT appear in the second window.

###Adding SignalR
Install using NuGet: Install-Package SignalR

Create a new folder called Hubs in the solution

Add a new class called TaskHub with the following code

```csharp
using SignalR.Hubs;    

namespace TaskR.Hubs {
  [HubName("TaskHub")]
  public class TaskHub : Hub {

  }
}
```

Open the _Layout.cshtml file and add the following below @RenderBody()

```
    <script src="@Url.Content("~/Scripts/jquery.signalR-0.5.0.js")"></script>    
    <script src="@Url.Content("~/signalr/hubs")"></script>
```

Open [Fiddler](http://www.fiddler2.com/fiddler2/) (or any tool to watch browser traffic).

Build and run the project.

In Fiddler you should see the following HTTP GET requests

```
/signalr/hubs
```

Inspect the response in TextView. You should see that the response was actually some javascript. Toward the end of the script locate the following:

```javascript
// Create hub signalR instance
$.extend(signalR, {
    taskHub: {
        _: {
            hubName: 'TaskHub',
            ignoreMembers: ['namespace', 'ignoreMembers', 'callbacks'],
            connection: function () { return signalR.hub; }
        }

    }
});
```

Notice that SignalR is using the [jQuery extend method](http://api.jquery.com/jQuery.extend/) to create a taskHub object. It has the name that we gave it (using the [HubName] attribute) but no functionality. Let's add some.

Open TaskHub.cs and add the following method:

```csharp
public void Login(string username) {
  Groups.Add(Context.ConnectionId, username);
}
```

Build and run the project.

Inspect the /signalr/hubs response. You should see the following:

```javascript
// Create hub signalR instance
$.extend(signalR, {
    taskHub: {
        _: {
            hubName: 'TaskHub',
            ignoreMembers: ['login', 'namespace', 'ignoreMembers', 'callbacks'],
            connection: function () { return signalR.hub; }
        },

        login: function (username, callback) {
            return serverCall(this, "Login", $.makeArray(arguments));
        }
    }
});
```

Our taskHub now has a "login" function! Notice that it's "login" and not "Login". SignalR will camelCase the function names. Also note that the function makes a "serverCall" to "Login".

This is how the SignalR javascript client works. It creates this proxy object for every Hub it finds in the application. It exposes all of the public methods of the Hub as javascript functions.

Just for fun, let's see how we can change the function names SignalR uses for our Hub methods.

In TaskHub.cs add the following attribute to the Login method

```csharp
[HubMethodName("Authenticate")]
public void Login(string username) {
  Groups.Add(Context.ConnectionId, username);
}
```

Build and run the project.

Inspect the /signalr/hubs response. You should see the following:

```javascript
// Create hub signalR instance
$.extend(signalR, {
    taskHub: {
        _: {
            hubName: 'TaskHub',
            ignoreMembers: ['authenticate', 'namespace', 'ignoreMembers', 'callbacks'],
            connection: function () { return signalR.hub; }
        },

        authenticate: function (username, callback) {
            return serverCall(this, "Authenticate", $.makeArray(arguments));
        }
    }
});
```

Our "login" function is gone, replaced by an "authenticate" function. Cool.

So now we know we have a "taskHub" available somewhere on the client-side javascript. But where?

SignalR makes our hubs accessible through the $.connection 

In taskr.js add the following under "use strict":

```javascript
var taskHub = $.connection.taskHub;
```

This will give us a shortcut to our taskHub. But we need to start the connection before we can use it. Add the following after the ko.applyBindings section:

```javascript
$.connection.hub.start();
```

Run the project.

There should be two new GET requests:

```
/signalr/negotiate
/signalr/connect?transport={negotiatedTransport}
```

The value of {negotiatedTransport} will vary depending on your browser. Try using a few browsers and see how they use different transports.

Notice that the /signalr/connect request does not immediately respond. It leaves the connection open, so that the server can decide when to respond. It will time out after 110 seconds and immediately reconnect.

So now we have a connect to our hub. Let's call that Authenticate method we added.

In taskr.js locate the viewModel.Login method and add the following:

```javascript
taskHub.authenticate(this.username());
```

Refresh your browser and log in using any username.

You should see a POST when you click the Login button:

```
/signalr/send
```

Inspecting the WebForms tab in Fiddler for this request, you should see a data object sent with the following:

```javascript
{
  "hub":"TaskHub",
  "method":"Authenticate",
  "args":["yourusername"],
  "state":{},
  "id":0
}
```

So it sent our username to the Login (authenticate) method, which added the connection ID to a new Group with our username. Great. Now what?

Let's notify when a task has been added. Open the TasksController.cs and add the following to the Post method after taskEntities.SaveChanges()

```csharp
var context = SignalR.GlobalHost.ConnectionManager.GetHubContext<TaskR.Hubs.TaskHub>();
context.Clients[task.AssignedTo].AddedTask(task);
```

Notice that we never defined a method on our Hub called AddedTask. That's because public methods on the hub are for the Client to call. AddedTask is going to be a method on the client that the server will call.

So let's add the AddedTask method on the client. Open taskr.js and add the following after var taskHub = $.connection.taskHub;

```javascript
taskHub.AddedTask = function(task) {
  tasks.addedTask(task);
};
```

Build and run. Open two different browser windows (or tabs) and log in with the same username to both. Add a new task in one of the windows. Verify that the task is automatically added to the second window.