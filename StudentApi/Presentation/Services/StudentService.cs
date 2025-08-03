using DefaultNamespace.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DefaultNamespace.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(long id);
        Task<List<Student>> GetStudentsByNameAsync(string name);
        string GetFormattedDate(string acceptLanguageHeader);
        string UpdateStudent(UpdateStudent request);
        Task<string> UploadImageAsync(IFormFile image);
        string DeleteStudent(long id);
    }

    public class StudentService : IStudentService
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

        public async Task<List<Student>> GetAllStudentsAsync() => await Task.FromResult(Students);

        public async Task<Student> GetStudentByIdAsync(long id)
        {
            if (id <= 0)
                throw new ValidationException("Please enter a valid Id greater than 0.");

            var student = Students.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Student with ID {id} not found.");

            return await Task.FromResult(student);
        }

        public async Task<List<Student>> GetStudentsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Name query parameter is required.");
            if (name.Length < 2)
                throw new ValidationException("Name must be at least 2 characters long.");

            var result = Students
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!result.Any())
                throw new KeyNotFoundException($"No students found matching name '{name}'.");

            return await Task.FromResult(result);
        }

        public string GetFormattedDate(string acceptLanguage)
        {
            var cultureCode = "en-US";

            if (!string.IsNullOrWhiteSpace(acceptLanguage))
            {
                var requestedCulture = acceptLanguage.Split(',')[0].Trim();
                var supportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Select(c => c.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();

                if (!supportedCultures.Contains(requestedCulture))
                    throw new CultureNotFoundException($"The requested culture '{requestedCulture}' is not supported.");

                cultureCode = requestedCulture;
            }

            var culture = new CultureInfo(cultureCode);
            return $"Current date in {culture.DisplayName}: {DateTime.Now.ToString("D", culture)}";
        }

        public string UpdateStudent(UpdateStudent request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Id <= 0) throw new ValidationException("Id must be greater than 0.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ValidationException("Name is required.");
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ValidationException("Email is required.");
            if (!new EmailAddressAttribute().IsValid(request.Email)) throw new ValidationException("Invalid email format.");

            var student = Students.FirstOrDefault(s => s.Id == request.Id)
                ?? throw new KeyNotFoundException($"Student with ID {request.Id} not found.");

            student.Name = request.Name;
            student.Email = request.Email;

            return $"Student with ID {request.Id} updated successfully.";
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ValidationException("No image file provided.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new NotSupportedException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");

            if (image.Length > 5 * 1024 * 1024)
                throw new ValidationException("File size exceeds 5MB limit.");

            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsDirectory);

            var fileName = Guid.NewGuid() + fileExtension;
            var filePath = Path.Combine(uploadsDirectory, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        public string DeleteStudent(long id)
        {
            if (id <= 0)
                throw new ValidationException("Id must be greater than 0.");

            var student = Students.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Student with ID {id} not found.");

            Students.Remove(student);
            return $"Student with ID {id} deleted successfully.";
        }
    }
}
