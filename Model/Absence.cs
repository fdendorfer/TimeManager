using System;

namespace TimeManager.Model
{
  public class AbsenceModel
  {
    public Guid ID { get; set; }
    public Guid IdUser { get; set; }
    public Guid IdAbsenceDetail { get; set; }
    public DateTime AbsentFrom { get; set; }
    public DateTime AbsentTo { get; set; }
    public bool Negative { get; set; }
    public string Reason { get; set; }
    public bool Approved { get; set; }
    public DateTime CreatedOn { get; set; }
  }
}