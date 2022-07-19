using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Fakexiecheng.API.Models
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser user { get; set; }


        //返回的商品列表
        public ICollection<LineItem> ShoppingCartItems { get; set; }
    

    }
}
