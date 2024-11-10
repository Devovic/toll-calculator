using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollCalculation.Core.Interfaces
{
    public interface IHolidayService
    {
        bool IsTollFreeDate(DateTime date);
    }
}
