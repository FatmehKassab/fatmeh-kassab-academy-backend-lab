using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace.Models;
using System.Globalization;

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

        [HttpGet("filter")]
        public async Task<ActionResult<List<Student>>> GetStudentsByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name query parameter is required.");
            }

            var filteredStudents = Students
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredStudents.Any())
            {
                return NotFound($"No students found matching name '{name}'.");
            }

            return await Task.FromResult(Ok(filteredStudents));
        }

        
        
        [HttpGet("date")]
        public ActionResult<string> GetFormattedDate()
        {
     
            var acceptLanguage = Request.Headers["Accept-Language"].ToString();

      
            var cultureCode = "en-US";

            if (!string.IsNullOrWhiteSpace(acceptLanguage))
            {
                var languages = acceptLanguage.Split(',');
                if (languages.Length > 0)
                {
                    var requestedCulture = languages[0].Trim();
                    
                    var supportedCultures = new[] { "en-US", "es-ES", "fr-FR" };
                    if (supportedCultures.Contains(requestedCulture))
                    {
                        cultureCode = requestedCulture;
                    }
                }
            }
            
            var culture = new CultureInfo(cultureCode);
            
            var formattedDate = DateTime.Now.ToString("D", culture);

            return Ok($"Current date in {cultureCode}: {formattedDate}");
        }
        
        [HttpPost("update")]
        public IActionResult UpdateStudent([FromBody] UpdateStudent request)
        {
            var student = Students.FirstOrDefault(s => s.Id == request.Id);

            if (student == null)
                return NotFound($"Student with ID {request.Id} not found.");

            student.Name = request.Name;
            student.Email = request.Email;

            return Ok($"Student with ID {request.Id} updated successfully.");
        }
        
        [HttpPost("upload-image")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file provided.");
            }
            
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }
            
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }
            
            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsDirectory, uniqueFileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }


            return Ok(new { 
                message = "Image uploaded successfully.",
                filePath = $"/uploads/{uniqueFileName}"
            });
        }
    }
    
    
}