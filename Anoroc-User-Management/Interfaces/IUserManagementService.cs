﻿using Anoroc_User_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anoroc_User_Management.Interfaces
{
    public interface IUserManagementService
    {
        string addNewUser(User user);
        public bool ValidateUserToken(string user_access_token);
    }
}
