﻿using Anoroc_User_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anoroc_User_Management.Interfaces
{
    public interface INotificationService
    {
        public void SaveNotificationToDatabase(Notification notification);
        public List<Notification> SendNotificationToApp(string notification);
        public void EmailNotificationToUser(string accessToken, string body);
    }
}
