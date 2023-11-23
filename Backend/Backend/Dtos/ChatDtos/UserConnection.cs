namespace Backend.Dtos.ChatDtos;

public class UserConnection
{
    public int Id { get; set; }
    public HashSet<string> Sockets { get; set; }
}