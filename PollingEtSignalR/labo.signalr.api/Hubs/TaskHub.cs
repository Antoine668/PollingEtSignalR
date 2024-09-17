using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class TaskHub : Hub
    {

        private readonly ApplicationDbContext _context;

        public TaskHub(ApplicationDbContext context)
        {
            _context = context;
        }

        static int nombreUser = 0;
        public override async Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
            List<UselessTask> taskList = await _context.UselessTasks.ToListAsync();
            // TODO: Ajouter votre logique
            nombreUser++;
            await Clients.Caller.SendAsync("TaskList", taskList);
            await Clients.All.SendAsync("UserCount", nombreUser);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            base.OnDisconnectedAsync(exception);
            // TODO: Ajouter votre logique
            nombreUser--;
            await Clients.All.SendAsync("UserCount", nombreUser);
        }

        public async Task AddTask(string taskText)
        {

            UselessTask uselessTask = new UselessTask()
            {
                Completed = false,
                Text = taskText
            };
            _context.UselessTasks.Add(uselessTask);
            await _context.SaveChangesAsync();
            

            List<UselessTask> tasks = await _context.UselessTasks.ToListAsync();

            await Clients.All.SendAsync("TaskList", tasks);
        }

        public async Task CompleteTask(int id)
        {
            UselessTask? task = await _context.FindAsync<UselessTask>(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
            }

            List<UselessTask> tasks = await _context.UselessTasks.ToListAsync();
            await Clients.All.SendAsync("TaskList", tasks);
        }
    }
}
