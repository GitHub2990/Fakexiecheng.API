using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.Models;
using Fakexiecheng.API.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Fakexiecheng.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthencateController : ControllerBase
    {
        //注入配置文件
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;//可以通过这个工具对密码进行加密  泛型为定义的用户模型
        private readonly SignInManager<ApplicationUser> _signInManager;  //登录验证
        private readonly ITouristRouteRepository _touristRouteRepository;
        public AuthencateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository
            )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }



        [AllowAnonymous]//允许所有人访问
        [HttpPost("login")]
        public async  Task<IActionResult> login([FromBody] LoginDto loginDto) 
        {
            //1.进行信息认证(省略)
            var loginResult = await _signInManager.PasswordSignInAsync(loginDto.Email,loginDto.password
                , false  //指示在关闭浏览器后登录 Cookie 是否应该保留的标志。
                , false//多次登录失败后 是否锁定账号
                );
            //判断是否验证成功
            if (!loginResult.Succeeded) 
            {
                // 400
                return BadRequest();
            }
            //获取用户信息
            var user = await _userManager.FindByNameAsync(loginDto.Email);

           

            //2.创建JWT  Token
            //header   singningAlgorithm储存编码算法
            var singningAlgorithm = SecurityAlgorithms.HmacSha256;
            //payload   需要用到的数据
            var claims = new List<Claim> { 
           // sub ==jwt的ID    
           //等同于  Sub:fake_user_id
           new Claim(JwtRegisteredClaimNames.Sub,user.Id),
          // new Claim(ClaimTypes.Role,"Admin")//角色信息

           };
            //获取用户角色
            var roleNames = await _userManager.GetRolesAsync(user);
            //遍历角色  一个用户可能有多个角色
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }

            //signature   数字签名   需要用到私钥
            //私钥一般放在配置文件中  私钥是自定义的  想写什么写什么

            //使用utf进行编码
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            //使用非对称算法 对私钥加密
            var signingkey = new SymmetricSecurityKey(secretByte);
            //通过256验证非对称加密的私钥
            var signingCredentials = new SigningCredentials(signingkey, singningAlgorithm);

            //创建token
            var token = new JwtSecurityToken(
                issuer:_configuration  ["Authentication:Issuer"],//谁发布的TOken
                audience:_configuration["Authentication:Audience"],//token发布给谁
           claims,//payload数据
           notBefore:DateTime.UtcNow,//发布时间
           expires:DateTime.UtcNow.AddDays(1),//有效时间
           signingCredentials//数字签名
                );

            //以字符串形式 输出Token
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            //3.返回jwt字符串

            return Ok(tokenStr);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="registerDto">注册信息</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //1 使用用户名创建用户对象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            //2 hash密码，保存用户
            var result = await _userManager.CreateAsync(user, registerDto.Password);//hash密码并连同用户模型对象 一起保存打数据库中
            //如果成功表示  用户创建成功并且保存起来了

            //不成功
            if (!result.Succeeded) {
                //返回400
                return BadRequest();
            }
            //3 初始化购物车
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id

            };
            //添加数据库
           await  _touristRouteRepository.CreateShoppingCart(shoppingCart);
            //保存
            await _touristRouteRepository.SaveAsync();

            //4 return
            //成功  200
            return Ok();
        
        }

    }
}
