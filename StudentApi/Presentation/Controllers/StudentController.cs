namespace DefaultNamespace;

[ApiController]
[Route(“api/students”)]
public class StudentController : ControllerBase
{
    private static readonly List<Student> Students = new List<Student>
    {
        new Student { Id = 1, Name = "Fatmeh Kassab", Email = "fatmeh@gmail.com" },
        new Student { Id = 2, Name = "Andrea Jabbour", Email = "andrea@gmail.com" },
        new Student { Id = 3, Name = "Christy Antoun", Email = "christy@gmail.com" },
        new Student { Id = 4, Name = "Jane Moujaess", Email = "jane@gmail.com" },
        new Student { Id = 5, Name = "Ali Yaakoub", Email = "ali@gmail.com" }
        new Student { Id = 6, Name = "Mariane Sleiman", Email = "mariane@gmail.com" }
    };
}