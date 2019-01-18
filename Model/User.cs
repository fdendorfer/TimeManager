using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model {
  public class UserModel {
    public Guid ID { get; set; }
    public Guid IdPermission { get; set; }
    public String Firstname { get; set; }
    public String Lastname { get; set; }
    public String Username { get; set; }
    public Byte[] Password { get; set; } // Use PasswortValue = to set password from string
    [NotMapped]
    public String PasswordValue {
      set {
        Password = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
      }
    }
    public String Department { get; set; }
    public Decimal Holidays { get; set; }
    public Boolean Deactivated { get; set; }

    public bool MatchesPassword(string pw) {
      var hashedPW = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(pw));
      return this.Password.SequenceEqual(hashedPW);
    }
  }
}
