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
using System.Net;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]  //规定的路由格式
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        //创建数据仓库变量
        private ITouristRouteRepository _touristRouteRepository;
        //创建映射变量
        private IMapper _mapper;

        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository ?? throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));


        }

        //获取单个路线中的旅游照片
        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            //判断路线是否存在
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在！");
            }
            //获取照片
            var touristeRoutePictureFromRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            //判断数据是否存在
            if (touristeRoutePictureFromRepo == null || touristeRoutePictureFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在!");
            }
            //Map 泛型代表要映射的类型   touristeRoutePictureFromRepo是原数据
            var GetPictureList = _mapper.Map<IEnumerable<TouristRoutePictureDto>>(touristeRoutePictureFromRepo);
            return Ok(GetPictureList);
        }

        //获取单个旅游路线中的单张照片   加{}表示get路由要传入的参数
        [HttpGet("{pictureId}",Name = "GetPicture")]
        public async  Task<IActionResult> GetPicture(Guid touristRouteId ,int pictureId)
        {

            //判断路线是否存在
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在！");
            }
            //判断照片是否存在
            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null) {
                return NotFound("照片不存在！");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TourisRoutePictureForCreationDto tourisRoutePictureForCreationDto

            )
        {
            //判断路线是否存在
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在！");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(tourisRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute(
                "GetPicture",
                new {
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn
                );
             
        }

        [HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeletePicture(
            [FromRoute]Guid touristRouteId,
            [FromRoute]int pictureId
            ) 
        {
            //判断路线是否存在
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("旅游路线不存在！");
            }

            //获取照片数据
            var pictuer =  await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictuer==null)
            {
                return NotFound("照片不存在！");
            }
            _touristRouteRepository.DeleteTouristRoutePictuer(pictuer);
            await _touristRouteRepository.SaveAsync();
            return NoContent();
        }



      


    }
}
