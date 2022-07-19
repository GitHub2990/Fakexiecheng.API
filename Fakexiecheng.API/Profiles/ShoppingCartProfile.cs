using AutoMapper;
using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Profiles
{
    public class ShoppingCartProfile : Profile
    {
      
         public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
