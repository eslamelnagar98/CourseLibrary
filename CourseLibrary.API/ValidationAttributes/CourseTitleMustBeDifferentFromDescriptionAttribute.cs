using CourseLibrary.API.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = validationContext.ObjectInstance as CourseForManipulationDto;
            if (course is null) return null;
            if (course.Title == course.Description)
                return new ValidationResult
                    (
                        ErrorMessage,
                         new List<string> { nameof(CourseForManipulationDto) }
                    );
            return ValidationResult.Success;
        }
    }
}
