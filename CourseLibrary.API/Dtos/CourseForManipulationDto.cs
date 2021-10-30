using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
namespace CourseLibrary.API.Dtos
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute
        (ErrorMessage = "The Provided Description Should Be Diffrent From The Title")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "You Should Fill Out a Title")]
        [MaxLength(100, ErrorMessage = "The Title Should not Have More Than 100 Charcters")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "The Deccription Should not Have More Than 1500 Charcters")]
        public virtual string Description { get; set; }
    }
}
