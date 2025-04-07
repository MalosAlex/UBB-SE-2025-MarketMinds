using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserType { get; set; }
        public float Balance { get; set; }
        public float Rating { get; set; }
        public float Password { get; set; }

        private const float MAX_BALANCE = 999999;

        public User(int id, string username, string email)
        {
            this.Id = id;
            this.Username = username;
            this.Email = email;
            this.Balance = MAX_BALANCE;
        }
    }
}
