namespace TollCalculation.Core.Interfaces
{
    public interface ITollRepository
    {
        Task<int> GetTollFee(DateTime time);
    }
}
