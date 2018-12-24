using System.ComponentModel.DataAnnotations;
using TimeManager.Model;

namespace TimeManager.Extensions
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        public bool AllowEmptyStrings { get; set; }
        public bool IsDisabled { get; set; } = false;
        public AbsenceValidation AbsenceValidation { get; set; }

        public RequiredIfAttribute() {
        }

        public override bool IsValid(object value)
        {
            if(IsDisabled == true)
            {
                return true;
            }
            if(value == null)
            {
                return false;
            }
            var stringValue = value as string;
            if (stringValue != null && !AllowEmptyStrings) {
                return stringValue.Trim().Length != 0;
            }
            return true;
        }
    }
}