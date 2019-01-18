using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeManager.Model
{
  public class UserValidation
  {
    // Set, when a user is opened for editing
    public string ID { get; set; }

    public string IdPermission { get; set; }

    public string Username { get; set; }

    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public string Password { get; set; }

    public string Department { get; set; }

    public decimal Holidays { get; set; }

    public bool Deactivated { get; set; }
  }
}
