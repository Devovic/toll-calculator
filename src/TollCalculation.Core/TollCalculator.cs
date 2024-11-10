using TollCalculation.Core.Interfaces;

namespace TollCalculation.Core
{
    public class TollCalculator
    {
        private const int MaxFee = 60;
        private const int FeeIntervalMinutes = 60;
        private readonly ITollRepository _tollRepository;

        public TollCalculator(ITollRepository tollRepository)
        {
            _tollRepository = tollRepository;
        }

        public int CalculateDailyToll(Vehicle vehicle, DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return 0;

            var sortedTimes = dates.OrderBy(t => t).ToArray();

            DateTime intervalStart = dates[0];
            int tempFee = GetTollFee(intervalStart, vehicle);

            int totalFee = 0;
            foreach (DateTime date in dates.Skip(1))
            {
                int nextFee = GetTollFee(date, vehicle);
                int minutes = GetMinutesBetween(intervalStart, date);

                if (minutes < FeeIntervalMinutes)
                {
                    tempFee = GetHighestFee(tempFee, nextFee);
                }
                else
                {
                    totalFee += tempFee;
                    intervalStart = date;
                    tempFee = nextFee;

                    if (totalFee >= MaxFee)
                        return MaxFee;
                }
            }

            totalFee += tempFee;
            return EnsureMaxFeeLimit(totalFee);
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return false;
            String vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        public int GetTollFee(DateTime date, Vehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;;

            return _tollRepository.GetTollFee(date);
        }

        private Boolean IsTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            //if (year == 2013)
            //{
            //    if (month == 1 && day == 1 ||
            //        month == 3 && (day == 28 || day == 29) ||
            //        month == 4 && (day == 1 || day == 30) ||
            //        month == 5 && (day == 1 || day == 8 || day == 9) ||
            //        month == 6 && (day == 5 || day == 6 || day == 21) ||
            //        month == 7 ||
            //        month == 11 && day == 1 ||
            //        month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        private static int GetMinutesBetween(DateTime start, DateTime end)
        {
            return (int)(end - start).TotalMinutes;
        }

        private static int GetHighestFee(int fee1, int fee2)
        {
            return fee1 > fee2 ? fee1 : fee2;
        }

        private static int EnsureMaxFeeLimit(int totalFee)
        {
            return totalFee < MaxFee ? totalFee : MaxFee;
        }

        private enum TollFreeVehicles
        {
            Motorbike = 0,
            Tractor = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}