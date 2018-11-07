using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManager.Models {
  public class UserModel {
    public Guid ID { get; set; }
    public Guid IdPermission { get; set; }
    public String Firstname { get; set; }
    public String Lastname { get; set; }
    public String Username { get; set; }
    public String Password { get; set; }
    public String Department { get; set; }

    //From Table Permission on IdPermission
    public Int16 Level { get; set; }
  }
}
