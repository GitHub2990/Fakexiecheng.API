using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    public class LineItemDto
    {
     
        public int Id { get; set; }
    
        public Guid TouristRouteId { get; set; }
        public TouristeRouteDto TouristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
        // public Guid? OrderId { get; set; }
  
        public decimal Originalprice { get; set; }
        //折扣
 
        public double? DiscountPresent { get; set; }
    }
}
