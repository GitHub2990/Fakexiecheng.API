using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text.RegularExpressions;
using Fakexiecheng.API.ResourceParameters;
using Fakexiecheng.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Fakexiecheng.API.Helper;
using Microsoft.AspNetCore.Authorization;

namespace Fakexiecheng.API.Controllers
{
    [Route("api/[controller]")]  //路由
    [ApiController]  //API控制器属性
    //TourestRoutes   asp命名规范   控制器结尾用复数形式
    public class TouristRoutesController : ControllerBase
    {
        //数据仓库接口
        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        //依赖注入  构造函数注入
        //注入的时候已经实现了ITouristRouteRepository     因为参数类型已经注册到了ioc容器，所以他所对面的实现类会直接返回实例
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository,IMapper mapper) {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        //FromQuery对应的请求路径为  api/toristRoute?keyword=参数
        [HttpGet]
        [HttpHead("GetTouristRoutes")]//请求成功后只返回头部信息   不返回主体
        //获取全部路径   加上搜索
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourcePramaters pramaters
           // [FromQuery] string keyword,
            //string rating //小于lessThan,大于largerThan,等于equalTo lessThan3,largerThan2,equal5
            ) 
        {


            //await 将会把当前函数挂起 进行下面函数操作   直到完成操作 再回来。
            var touristeRouteFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(pramaters.keyword, pramaters.RatingOperator, pramaters.RatingValue);
        

            if (touristeRouteFromRepo == null || touristeRouteFromRepo.Count() <= 0) {
               //404
                return NotFound("没有旅游路线");
            
            }
            //对数据进行映射处理
            var touristRouteDto = _mapper.Map < IEnumerable < TouristeRouteDto >>(touristeRouteFromRepo);
            return Ok(touristRouteDto);
        }

        //根据ID获取单一路径
        [HttpGet("{touristRoutId}",Name = "GetTouristRouteById")]
        // [HttpHead]  //请求成功后只返回头部信息   不返回主体    路由为:api/TouristRoute/{touristRoutId}
        public async  Task<IActionResult> GetTouristRouteById(Guid touristRoutId) {
            var touristeRouteFromRepo =await  _touristRouteRepository.GetTouristRouteAsync(touristRoutId);
            if (touristeRouteFromRepo == null)
            {
                //404
                return NotFound($"旅游路线{touristRoutId}不存在！");

            }
            //通过Dtos处理后的数据
            //var touristRouteDto = new TouristeRouteDto()
            //{
            //    Id = touristeRouteFromRepo.Id,
            //    Title=touristeRouteFromRepo.Title,
            //    Description=touristeRouteFromRepo.Description,
            //    Price=touristeRouteFromRepo.Originalprice*(decimal)(touristeRouteFromRepo.DiscountPresent ?? 1),//变量定义中含有两个问号，意思是取所赋值??左边的，如果左边为null，取所赋值??右边的。DiscountPresent是折扣
            //    CreateTime=touristeRouteFromRepo.CreateTime,
            //    UpdateTime=touristeRouteFromRepo.UpdateTime,
            //    Features=touristeRouteFromRepo.Features,
            //    Fees=touristeRouteFromRepo.Fees,
            //    Notes=touristeRouteFromRepo.Notes,
            //    Rating=touristeRouteFromRepo.Rating,
            //    travelDays=touristeRouteFromRepo.travelDays.ToString(),
            //    TripType=touristeRouteFromRepo.TripType.ToString(),
            //    DepartureCity=touristeRouteFromRepo.DepartureCity.ToString()


            //};
            ///Map泛型是映射的结果   参数是原始数据
            var touristRouteDto = _mapper.Map<TouristeRouteDto>(touristeRouteFromRepo);

            return Ok(touristRouteDto);
        }
        /// <summary>
        /// 创建旅游路线    .net core  将会自动解析Json中的数据   映射到Dto内对应的字段
        /// </summary>
        /// <parameter>FromBody特性 用以接收Json类型数据</parameter>>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles ="Admin")]  //只有登录的用户才可以访问该方法  加了Roles代表只有admin这个角色可以访问
        public async  Task<IActionResult> CreateTouristRoute
            (
            [FromBody] TouristRouteForCreationDto touristRouteForCreationDto
            )
        {
            //将数据映射为目标数据模型
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            //添加模型实体到上下文  内存中
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            //保存到数据库
            await _touristRouteRepository.SaveAsync();
            //将添加好的数据返回给Api进行数据输出
            var touristRouteToReture = _mapper.Map<TouristeRouteDto>(touristRouteModel);

            //CreatedAtRoute:
            //            参数
            //routeName
            //String
            //用于生成 URL 的路由的名称。

            //routeValues
            //Object
            //用于生成 URL 的路由数据。

            //value
            //Object
            //要在实体正文中设置格式的内容值。
            //创建成功返回201 ///并且返回Api名称 ps就是创建数据后的API路由  可以通过这个路由查看到你刚刚创建的数据
            //返回的API在  headers中的Location中  
            return CreatedAtRoute(
                routeName:"GetTouristRouteById",
                routeValues: new { touristRoutId = touristRouteToReture.Id }, 
                value:touristRouteToReture
                );

        }

        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateTourisRoute(
            Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
            ) 
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)) 
            {
                return NotFound("旅游路线找不到");
            }
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //更新
            //1.映射dto
            //2.更新dto
            //3.映射model
            //将Dto映射给Model  完成自动更新
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);   //记得所有的映射都需要先在profiles文件进行相应的声明
            //更新到数据库
            await _touristRouteRepository.SaveAsync();

            return NoContent();//返回204  前端只能得到204结果，什么都没有。

        }



        /// <summary>
        /// HttpPatch   使用JsonPacth实现局部更新
        /// </summary>
        /// <param name="touristRouteId">局部路线id</param>
        /// <param name="patchDoucument">对JsonPatc进行数据解析</param>
        /// <returns></returns>
        [HttpPatch("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async  Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute]Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDoucument
            )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }

            //获取原始数据
          var touristRouteFormRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            //进行数据映射
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFormRepo);
            //进行数据更新  使用JsonPatchDocument内置函数进行更新
            patchDoucument.ApplyTo(touristRouteToPatch, ModelState);  //ModelState是系统的全局变量  对dto进行绑定
            //对更改的数据做数据验证   TryValidateModel验证指定的 model 实例。  如果有效，则为true ModelState ;否则为 false 。
            if (!TryValidateModel(touristRouteToPatch)) 
            {
                //验证失败
                ///创建一个 ActionResult ，它将生成 Status400BadRequest 具有验证错误的响应 modelStateDictionary 。
                return ValidationProblem(ModelState);
            }



            //数据保存
            _mapper.Map(touristRouteToPatch, touristRouteFormRepo);
            await _touristRouteRepository.SaveAsync();

            //204
            return NoContent();

        }

        /// <summary>
        /// 删除旅游路线
        /// </summary>
        /// <param name="touristRouteId">通过指定的路线ID删除</param>
        /// <returns></returns>
        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeliteTouristRoute([FromRoute] Guid touristRouteId)
        {

            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }

            //获取数据模型
            var touristRoute =await  _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();
            return NoContent();

        }


        [HttpDelete("({touristRoutesIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async  Task<IActionResult> DeleteByIds(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))][FromRoute]IEnumerable<Guid> touristRoutesIds) 
        {
            if (touristRoutesIds == null) 
            {  //400
                return BadRequest();
            }

            //获取对象模型列表
          var touristRoutesFromRepo = await  _touristRouteRepository.GetTouristRouteByIdListAsync(touristRoutesIds);
            //批量删除
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            //保存
            await _touristRouteRepository.SaveAsync();
            return NoContent();

        }




    }
}
