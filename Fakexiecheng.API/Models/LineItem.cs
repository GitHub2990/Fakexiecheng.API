using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Models
{
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TouristRouteId")]
        public Guid TouristRouteId { get; set; }
        public TouristRoute TouristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
       // public Guid? OrderId { get; set; }
        //原价
        [Column(TypeName = "decimal(18,2)")]
        public decimal Originalprice { get; set; }
        //折扣
        [Range(0.0, 1.0)]
        public double? DiscountPresent { get; set; }

    }
}
