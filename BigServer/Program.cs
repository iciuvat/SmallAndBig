using System.Net.Http.Headers;
using System.Text.Json;
using Npgsql;

async Task<List<Location>> GetLocationsAsync(NpgsqlDataSource dataSource, bool addMeasurements)
{
    await using var command = dataSource.CreateCommand("SELECT * FROM locations");
    await using var reader = await command.ExecuteReaderAsync();

    var locations = new List<Location>();
    while (await reader.ReadAsync())
    {
        var location = new Location
        {
            Id = reader.GetInt32(0),
            Address = reader.GetString(1),
            Web = reader.GetString(2),
            Name = reader.GetString(3)
        };
        if (addMeasurements)
        {
            var x = await GetMeasurementByLocationAsync(dataSource, location.Id);
            location.measurements = x;
        }
        locations.Add(location);
    }

    return locations;
}

async Task<List<Measurement>?> GetMeasurementByLocationAsync(NpgsqlDataSource dataSource, int locationId)
{
    await using var command = dataSource.CreateCommand(
        "SELECT timestamp, value, unit FROM measurements WHERE location_id = " + locationId + " ORDER BY timestamp DESC LIMIT 100");
    await using var reader = await command.ExecuteReaderAsync();

    var measurements = new List<Measurement>();
    while (await reader.ReadAsync())
    {
        measurements.Add( new Measurement
        {
            Timestamp  = reader.GetDateTime(0),
            Value = (decimal) reader.GetDouble(1),
            Unit = reader.GetString(2)
        });
    }

    return measurements.Count == 0 ? null : measurements;
}

await using var dataSource = NpgsqlDataSource.Create(
    /*windows*/ "Host=localhost:5243;Username=dboperator;Password=pass1765;Database=measurements_db"
    // /*macos*/ "User ID=dboperator;Password=pass1765;Host=localhost;Port=5432;Database=measurements_db;Pooling=true;"
);

var clients = new Dictionary<string, HttpClient>();

// check sites for new data
new Thread(async (dataSourceAndClient) => 
{
    var tupleDataSourceAndClient = dataSourceAndClient as ValueTuple<NpgsqlDataSource, Dictionary<string, HttpClient>>?;
    var clients = tupleDataSourceAndClient?.Item2;
    
    while(true)
    {
        var locations = await GetLocationsAsync(dataSource: tupleDataSourceAndClient?.Item1,
                                                addMeasurements: false);

        foreach (var location in locations)
        {
            
            HttpClient client;
            if (clients.ContainsKey(location.Web))
            {
                client = clients[location.Web];
            }
            else
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(location.Web);
                clients[location.Web] = client;
            }
            
            HttpResponseMessage response;
            try
            {
                response = await client.DeleteAsync("/api/get_saved_values");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("cannot access " + client.BaseAddress + "/api/get_saved_values\n" + ex.Message);
                continue;
            }
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var measurements = JsonSerializer.Deserialize<List<Measurement>>(responseBody, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
                var insert = "INSERT INTO measurements (location_id, timestamp, value, unit) VALUES";
                foreach (var measurement in measurements)
                {
                    insert += " ('" + location.Id + "', '" + measurement.Timestamp + "', '" +measurement.Value + "', '" + measurement.Unit + "'),";
                }
                insert = insert.TrimEnd(',');
                if (measurements.Count > 0)
                {
                    await using var command = dataSource.CreateCommand(insert);
                    var rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
    }

    Thread.Sleep(5 * 1000);
}).Start((dataSource, clients));

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/locations", async () => await GetLocationsAsync(
    dataSource,
    addMeasurements: false));
app.MapGet("/api/locations_and_measurements", async () => await GetLocationsAsync(
    dataSource,
    addMeasurements: true));
app.MapGet("/api/locations/{locationId:int}", async (int locationId) => await GetMeasurementByLocationAsync(dataSource, locationId));

app.Run();