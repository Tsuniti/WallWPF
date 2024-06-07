using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
namespace WallWPF.Entities;

public class ChatUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; }

    public string Password { get; set; }

}