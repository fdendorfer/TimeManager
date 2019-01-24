using System;

namespace TimeManager.Model
{
  public class ControllingModel
  {
    public Guid IdUser { get; set; }
    public Guid IdAbsence { get; set; }
    public string Name { get; set; }
    public DateTime AbsentFrom { get; set; }
    public DateTime AbsentTo { get; set; }
    public string Reason { get; set; }
    public bool Approved { get; set; }
    public DateTime CreatedOn { get; set; }
  }
}