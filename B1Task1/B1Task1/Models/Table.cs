namespace B1Task1.Models;

/// <summary>
/// Model for Database
/// </summary>
public class Table
{
    public long Id { get; set; }
    public DateOnly Date { get; set; }
    public string EngString { get; set; }
    public string RusString { get; set; }
    public int IntValue { get; set; }
    public double DoubleValue { get; set; }
}