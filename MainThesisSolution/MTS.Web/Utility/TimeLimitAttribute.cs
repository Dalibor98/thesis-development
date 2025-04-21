using System.ComponentModel.DataAnnotations;

namespace MTS.Web.Utility
{
    public class TimeLimitAttribute : ValidationAttribute
    {
        private readonly string _startTimeProperty;
        private readonly string _endTimeProperty;

        public TimeLimitAttribute(string startTimeProperty, string endTimeProperty)
        {
            _startTimeProperty = startTimeProperty;
            _endTimeProperty = endTimeProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var timeLimit = (int)value;

            // Get property values using reflection
            var startTimeProperty = validationContext.ObjectType.GetProperty(_startTimeProperty);
            var endTimeProperty = validationContext.ObjectType.GetProperty(_endTimeProperty);

            if (startTimeProperty == null || endTimeProperty == null)
                return new ValidationResult($"Properties {_startTimeProperty} or {_endTimeProperty} not found.");

            var startTime = (DateTime)startTimeProperty.GetValue(validationContext.ObjectInstance);
            var endTime = (DateTime)endTimeProperty.GetValue(validationContext.ObjectInstance);

            // Calculate the maximum allowed time limit based on the duration between start and end time
            var totalMinutes = (endTime - startTime).TotalMinutes;

            // If time limit exceeds the quiz duration, return validation error
            if (timeLimit > totalMinutes)
            {
                return new ValidationResult(
                    $"Time limit cannot exceed the duration between start time and end time ({Math.Floor(totalMinutes)} minutes).");
            }

            return ValidationResult.Success;
        }
    }
}