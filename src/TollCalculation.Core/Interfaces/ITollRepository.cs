namespace TollCalculation.Core.Interfaces
{
    public interface ITollRepository
    {
        int GetTollFee(DateTime time);
    }
}
