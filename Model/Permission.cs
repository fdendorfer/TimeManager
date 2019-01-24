using System;

namespace TimeManager.Model
{
  public class PermissionModel
  {
    public Guid ID { get; set; }
    public byte Level { get; set; }
    public string Description { get; set; }
    public string DescriptionLong { get; set; }
  }
}
