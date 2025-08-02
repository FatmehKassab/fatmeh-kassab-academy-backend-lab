using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace.Models;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

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
            try
            {
                if (id <= 0)
                {
                    throw new ValidationException("Please enter a valid Id greater than 0.");
                }

                var student = Students.FirstOrDefault(s => s.Id == id);

                if (student == null)
                {
                    throw new KeyNotFoundException($"Student with ID {id} not found.");
                }

                return await Task.FromResult(Ok(student));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<Student>>> GetStudentsByName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ValidationException("Name query parameter is required.");
                }

                if (name.Length < 2)
                {
                    throw new ValidationException("Name must be at least 2 characters long.");
                }

                var filteredStudents = Students
                    .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!filteredStudents.Any())
                {
                    throw new KeyNotFoundException($"No students found matching name '{name}'.");
                }

                return await Task.FromResult(Ok(filteredStudents));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("date")]
        public ActionResult<string> GetFormattedDate()
        {
            try
            {
                var acceptLanguage = Request.Headers["Accept-Language"].ToString();
                var cultureCode = "en-US"; 

                if (!string.IsNullOrWhiteSpace(acceptLanguage))
                {
                    var languages = acceptLanguage.Split(',');
                    if (languages.Length > 0)
                    {
                        var requestedCulture = languages[0].Trim();
                        
       
                        var supportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                            .Select(c => c.Name)
                            .Where(n => !string.IsNullOrEmpty(n))
                            .ToList();

                        if (supportedCultures.Contains(requestedCulture))
                        {
                            cultureCode = requestedCulture;
                        }
                        else
                        {
                            throw new CultureNotFoundException($"The requested culture '{requestedCulture}' is not supported.");
                        }
                    }
                }
                
                var culture = new CultureInfo(cultureCode);
                var formattedDate = DateTime.Now.ToString("D", culture);

                return Ok($"Current date in {culture.DisplayName}: {formattedDate}");
            }
            catch (CultureNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
            finally
            {
           
            }
        }
        
        [HttpPost("update")]
        public IActionResult UpdateStudent([FromBody] UpdateStudent request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request body cannot be null.");
                }

                if (request.Id <= 0)
                {
                    throw new ValidationException("Please enter a valid Id greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new ValidationException("Name is required.");
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new ValidationException("Email is required.");
                }

                if (!new EmailAddressAttribute().IsValid(request.Email))
                {
                    throw new ValidationException("Please provide a valid email address.");
                }

                var student = Students.FirstOrDefault(s => s.Id == request.Id);

                if (student == null)
                {
                    throw new KeyNotFoundException($"Student with ID {request.Id} not found.");
                }

                student.Name = request.Name;
                student.Email = request.Email;

                return Ok($"Student with ID {request.Id} updated successfully.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("upload-image")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    throw new ValidationException("No image file provided.");
                }
                
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    throw new NotSupportedException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }
                
                if (image.Length > 5 * 1024 * 1024) 
                {
                    throw new ValidationException("File size exceeds the 5MB limit.");
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
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"File system error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpDelete("{id:long}")]
        public IActionResult DeleteStudent([FromRoute] long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ValidationException("Please enter a valid Id greater than 0.");
                }

                var student = Students.FirstOrDefault(s => s.Id == id);
                if (student == null)
                {
                    throw new KeyNotFoundException($"Student with ID {id} not found.");
                }

                Students.Remove(student);
                return Ok($"Student with ID {id} deleted successfully.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}