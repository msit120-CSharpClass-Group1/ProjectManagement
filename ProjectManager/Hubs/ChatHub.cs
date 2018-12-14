using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ProjectManager.Models.ViewModel;

namespace ProjectManager.Hubs
{
    public class ChatHub : Hub
    {
        public void SendMessage(string nickName, string message)  //給前端呼叫使用
        {
            Clients.All.gotMessage(nickName, message);
        }
    }
}