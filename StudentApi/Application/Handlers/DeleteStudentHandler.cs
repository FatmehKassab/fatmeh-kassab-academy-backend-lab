using MediatR;
using DefaultNamespace.Services; 

public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, string>
{
    private readonly IStudentService _studentService;

    public DeleteStudentHandler(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public Task<string> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var result = _studentService.DeleteStudent(request.Id);
        return Task.FromResult(result);
    }
}