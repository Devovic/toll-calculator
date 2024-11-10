using System.ComponentModel.DataAnnotations;

namespace TollCalculation.Core.Entities
{
    public class TollPrice
    {
        [Key]
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Amount { get; set; }
    }
}