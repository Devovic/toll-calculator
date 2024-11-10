namespace TollCalculation.API.Dtos
{
    public class CalculateTollRequest
    {
        public string VehicleType { get; set; }
        public DateTimeOffset[] Dates { get; set; }
    }
}
