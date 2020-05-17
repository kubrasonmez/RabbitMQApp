using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
