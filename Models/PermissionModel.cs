using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeManager.Models {
  public class PermissionModel {
    public Guid ID { get; set; }
    public Int16 Level { get; set; }
    public String Description { get; set; }
    public String DescriptionLong { get; set; }
  }
}
