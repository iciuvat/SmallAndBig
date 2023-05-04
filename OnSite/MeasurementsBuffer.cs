using System.Text.Json;

public class MeasurementsBuffer
{
    private string? path;
    public List<Measurement> Measurements { get; private set; } = new List<Measurement>();

    public void Load(string path)
    {
        lock(Measurements)
        {
            if (File.Exists("OnSiteBuffer.json"))
            {
                using (StreamReader reader = new StreamReader(path))
                {  
                    var content = reader.ReadToEnd();
                    Measurements = JsonSerializer.Deserialize<List<Measurement>>(content);
                }
            }
            this.path = path;            
        }
    }

    public void Save(string? path = null)
    {
        lock(Measurements)
        {
            if (path == null)
            {
                path = this.path ?? throw new ArgumentNullException("Unknown buffer path", nameof(this.path));
            }
            
            string jsonString = JsonSerializer.Serialize(Measurements, new JsonSerializerOptions() { WriteIndented = true});  
            using (StreamWriter outputFile = new StreamWriter(path))  
            {
                outputFile.WriteLine(jsonString);  
            }
        }
    }
    public void Insert(int position, Measurement measurement)
    {
        lock(Measurements)
        {
            Measurements.Insert(position, measurement);
        }
    }
    public List<Measurement> GetContentAndEmptyIt()
    {
        lock(Measurements)
        {
            var content = Measurements;
            Measurements = new List<Measurement>();
            Save();
            return content;
        }
    }
}