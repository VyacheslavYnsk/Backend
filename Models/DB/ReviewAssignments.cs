using Domain.Entities;
public class ReviewAssignment
{
    public int Id { get; set; }
    public int SolutionId { get; set; }
    public SolutionModel Solution { get; set; }
    public int ReviewerId { get; set; }
    public UserModel Reviewer { get; set; }
    public DateTime AssignedDate { get; set; }
    // Статус проверки и другие свойства
}