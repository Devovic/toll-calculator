namespace TollCalculation.Core.Interfaces
{
    public interface ITollFeeCalculatorService
    {
        Task<Dictionary<DateTime, int>> CalculateTool(string vehicleType, DateTime[] utcDates);
    }
}
