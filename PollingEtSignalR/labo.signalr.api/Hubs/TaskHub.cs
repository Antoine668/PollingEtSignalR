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
        public override async Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
            List<UselessTask> taskList = await _context.UselessTasks.ToListAsync();
            // TODO: Ajouter votre logique
            await Clients.Caller.SendAsync("TaskList", taskList);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            base.OnDisconnectedAsync(exception);
            // TODO: Ajouter votre logique
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

            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }
    }
}
