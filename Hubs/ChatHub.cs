using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUpload.Models;
using FileUpload.Mongo;
using Microsoft.AspNetCore.SignalR;

namespace FileUpload.Hubs
{
    public class ChatHub : Hub
    {
        MongoHelper _mongoHelper; // TODO: Get from Services
        public ChatHub(MongoHelper mongoHelper)
        {
            _mongoHelper = mongoHelper;
        }
        
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            MyChat chat = new MyChat()
            {
                Message = message,
                Name = user,
                Date = DateTime.Now
            };

            _mongoHelper.CreateChat(chat);
        }
    }
}
