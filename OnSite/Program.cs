var buffer = new MeasurementsBuffer();
buffer.Load("OnSiteBuffer.json");

// simulating getting data from measurement device
new Thread( (buffer) => 
{
    var rand = new Random();
    var supportedUnits = new string []{"kV", "A", "VA"};
    while(true)
    {
        (buffer as MeasurementsBuffer)?.Insert(0, new Measurement
        {
            Timestamp = DateTime.Now,
            Unit = supportedUnits[rand.Next(supportedUnits.Length)],
            Value = Math.Round((decimal)(100.0 + 30.0 * rand.NextDouble()), 2)
        });
        (buffer as MeasurementsBuffer)?.Save();

        Thread.Sleep(5 * 1000);
    }
}).Start(buffer);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.Urls.Add("http://192.168.70.136:1081");

app.MapGet("/api/all", () => buffer.Measurements);
app.MapDelete("/api/get_saved_values", () => buffer.GetContentAndEmptyIt());

app.Run();