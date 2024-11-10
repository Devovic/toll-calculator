namespace TollCalculation.Core.Interfaces
{
    public interface ITimeConversionService
    {
        DateTime ConvertToSwedishTime(DateTime utcTime);
        DateTime ConvertToUtc(DateTimeOffset inputTime);
    }
}
