using System;

namespace TimeManager.Model
{
  public class OvertimeDetailModel
  {
    public Guid ID { get; set; }
    public string Description { get; set; }
    public decimal Rate { get; set; }
  }
}