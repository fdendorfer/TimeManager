using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TimeManager.Model
{
  public class UserModel
  {
    public Guid ID { get; set; }
    public Guid IdPermission { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Username { get; set; }
    public byte[] Password { get; set; } // Use PasswortValue = to set password from string
    [NotMapped]
    public string PasswordValue {
      set => Password = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
    }
    public string Department { get; set; }
    public decimal Holidays { get; set; }
    public bool Deactivated { get; set; }

    public bool MatchesPassword(string pw)
    {
      var hashedPW = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(pw));
      return this.Password.SequenceEqual(hashedPW);
    }
  }
}
