using MediatR;
using DefaultNamespace.Models;

public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, List<Student>>
{
    private static readonly List<Student> _students = new();

    public Task<List<Student>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_students);
    }
}