using System.ComponentModel.DataAnnotations;
namespace CourseLibrary.API.Dtos
{

    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required(ErrorMessage = "You Should Fill Out a Description")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
