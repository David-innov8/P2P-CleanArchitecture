namespace P2P.Application.UseCases.Interfaces;

public interface ISetPinCase
{
     Task Handle(string command);
}