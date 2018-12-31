using System;
using System.ComponentModel.DataAnnotations;

namespace TimeManager.Model
{
  public class ControllingModel
  {
    public Guid IdUser {get; set; }
    public Guid IdAbsence { get; set; }
    public String Name { get; set; }
    public DateTime AbsentFrom { get; set; }
    public DateTime AbsentTo { get; set; }
    public String Reason { get; set; }
    public Boolean Approved { get; set; }
    public DateTime CreatedOn { get; set; }
  }
}