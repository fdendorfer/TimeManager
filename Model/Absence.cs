using System;
using System.ComponentModel.DataAnnotations;

namespace TimeManager.Model
{
  public class AbsenceModel
  {
    public Guid ID { get; set; }
    public Guid IdUser { get; set; }
    public Guid IdAbsenceDetail { get; set; }
    public DateTime AbsentFrom { get; set; }
    public DateTime AbsentTo { get; set; }
    public String Reason { get; set; }
    public Boolean Approved { get; set; }
  }
}