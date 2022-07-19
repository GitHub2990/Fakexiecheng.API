using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    public class ShoppingCartDto
    {
     
        public Guid Id { get; set; }

        public string UserId { get; set; }

        //返回的商品列表
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}
