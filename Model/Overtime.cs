using System;

namespace TimeManager.Model
{
  public class OvertimeModel
  {
    public Guid ID { get; set; }
    public Guid IdOvertimeDetail { get; set; }
    public Guid IdUser { get; set; }
    public string Customer { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public bool Approved { get; set; }
    public DateTime LastChanged { get; set; }
  }
}