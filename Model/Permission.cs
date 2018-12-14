using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeManager.Model {
  public class Permission {
    public Guid ID { get; set; }
    public byte Level { get; set; }
    public String Description { get; set; }
    public String DescriptionLong { get; set; }
  }
}
