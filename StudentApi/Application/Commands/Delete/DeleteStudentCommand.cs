using MediatR;

public record DeleteStudentCommand(long Id) : IRequest<string>;