namespace TollCalculation.Core.Interfaces
{
    public interface ITollCalculator
    {
        public Task<int> CalculateDailyToll(DateTime[] dates);
    }
}
