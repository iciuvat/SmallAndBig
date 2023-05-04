public class Location  
{  
    public int Id { get; set; }
    public string? Address { get; set; }
    public string? Web { get; set; }
    public string? Name { get; set; }
    public List<Measurement>? measurements { get; set; }
}