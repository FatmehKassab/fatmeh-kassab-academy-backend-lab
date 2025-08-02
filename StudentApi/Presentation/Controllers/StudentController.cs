using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace.Models;

namespace DefaultNamespace
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private static readonly List<Student> Students = new List<Student>
        {
            new Student { Id = 1, Name = "Fatmeh Kassab", Email = "fatmeh@gmail.com" },
            new Student { Id = 2, Name = "Andrea Jabbour", Email = "andrea@gmail.com" },
            new Student { Id = 3, Name = "Christy Antoun", Email = "christy@gmail.com" },
            new Student { Id = 4, Name = "Jane Moujaess", Email = "jane@gmail.com" },
            new Student { Id = 5, Name = "Ali Yaakoub", Email = "ali@gmail.com" },
            new Student { Id = 6, Name = "Mariane Sleiman", Email = "mariane@gmail.com" }
        };

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            return await Task.FromResult(Ok(Students));
        }
        
        [HttpGet("{id:long}")]
        public async Task<ActionResult<Student>> GetStudent([FromRoute] long id)
        {
            var student = Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            return await Task.FromResult(Ok(student));
        }

    }
}