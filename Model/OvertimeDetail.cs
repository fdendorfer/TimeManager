using System;

namespace TimeManager.Model
{
  public class OvertimeDetailModel
  {
    public Guid ID { get; set; }
    public String Description { get; set; }
    public Decimal Rate { get; set; }
  }
}