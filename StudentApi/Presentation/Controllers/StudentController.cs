using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace.Models;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using DefaultNamespace.Services;

namespace DefaultNamespace
{
  [ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Student>>> GetAllStudents() =>
        Ok(await _studentService.GetAllStudentsAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Student>> GetStudent(long id)
    {
        try
        {
            return Ok(await _studentService.GetStudentByIdAsync(id));
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    [HttpGet("filter")]
    public async Task<ActionResult<List<Student>>> GetStudentsByName(string name)
    {
        try
        {
            return Ok(await _studentService.GetStudentsByNameAsync(name));
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    [HttpGet("date")]
    public ActionResult<string> GetFormattedDate()
    {
        try
        {
            var lang = Request.Headers["Accept-Language"].ToString();
            return Ok(_studentService.GetFormattedDate(lang));
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    [HttpPost("update")]
    public IActionResult UpdateStudent(UpdateStudent request)
    {
        try
        {
            return Ok(_studentService.UpdateStudent(request));
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        try
        {
            var path = await _studentService.UploadImageAsync(image);
            return Ok(new { message = "Image uploaded successfully.", filePath = path });
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    [HttpDelete("{id:long}")]
    public IActionResult DeleteStudent(long id)
    {
        try
        {
            return Ok(_studentService.DeleteStudent(id));
        }
        catch (Exception ex) { return HandleException(ex); }
    }

    private ObjectResult HandleException(Exception ex) =>
        ex switch
        {
            ValidationException or ArgumentNullException => BadRequest(ex.Message),
            KeyNotFoundException => NotFound(ex.Message),
            CultureNotFoundException => BadRequest(ex.Message),
            NotSupportedException => BadRequest(ex.Message),
            _ => StatusCode(500, $"Unexpected error: {ex.Message}")
        };
}

}