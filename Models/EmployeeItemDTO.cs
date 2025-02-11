namespace TestAPICreateWithC_.Models;
public class EmployeeItemDTO
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? Position { get; set; }
    public required DateOnly DOB { get; set; }
    public string? Role { get; set; }
    public long? Salary { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public string? Zip { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
