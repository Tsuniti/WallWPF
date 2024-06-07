namespace WallWPF.Entities;
using System.ComponentModel.DataAnnotations;

public class Publication
{
    public Guid Id { get; set; }
    [StringLength(512)]
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public ChatUser User { get; set; }
}