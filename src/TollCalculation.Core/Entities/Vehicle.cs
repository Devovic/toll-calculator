using System.ComponentModel.DataAnnotations;

namespace TollCalculation.Core.Entities
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsTollFree { get; set; }
    }
}
