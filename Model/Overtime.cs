using System;

namespace TimeManager.Model
{
  public class OvertimeModel
  {
    public Guid ID { get; set; }
    public Guid IdOvertimeDetail { get; set; }
    public Guid IdUser { get; set; }
    public String Customer { get; set; }
    public DateTime Date { get; set; }
    public Decimal Hours { get; set; }
    public DateTime CreatedOn { get; set; }
  }
}