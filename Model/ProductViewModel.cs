using System;
using Agullto_IMS.Models;

namespace Agullto_IMS.API.Models
{
    public class ProductViewModel
    {
        
        public required string Name { get; set; }
        public int Stock { get; set; }
        public ProductDepartment Department { get; set; }
        public double WeightValue { get; set; }
        public MeasurementUnit Unit { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}