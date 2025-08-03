using MediatR;
using DefaultNamespace.Models;

public record GetAllStudentsQuery() : IRequest<List<Student>>;