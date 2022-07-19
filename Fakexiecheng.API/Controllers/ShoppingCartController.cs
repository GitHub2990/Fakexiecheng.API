using AutoMapper;
using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.Models;
using Fakexiecheng.API.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]   
    public class ShoppingCartController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
            ) 
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart() 
        {
            // 1.获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // 2. 通过Useid获取用户购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);


            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));

        }

        //添加商品进入购物车
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShopingCartItemDto addShopingCartItemDto
            )
        {
            // 1.获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // 2. 通过Useid获取用户购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            // 3.创建LienItem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShopingCartItemDto.TouristRouteId);

            if (touristRoute == null) 
            {
                return NotFound("旅游路线不存在！");
            }
            var lineItem = new LineItem
            {
                TouristRouteId = addShopingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                Originalprice = touristRoute.Originalprice,
                DiscountPresent = touristRoute.DiscountPresent

            };

            //添加item,并保存数据库
            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));


        }




    }
}
