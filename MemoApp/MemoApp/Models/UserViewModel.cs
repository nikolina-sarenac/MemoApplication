using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public RoleViewModel Role { get; set; }
        public List<RoleViewModel> AllRoles { get; set; }

    }
}
