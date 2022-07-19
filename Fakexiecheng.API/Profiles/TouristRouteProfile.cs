using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.Models;

namespace Fakexiecheng.API.Profiles
{
    public class TouristRouteProfile:Profile
    {
        //映射配置就是在构造函数完成的
        //AutoMapper 在注入完成后 会自动扫秒Profile 然后通过构造函数完成映射
        public TouristRouteProfile() 
        {
            //CreateMap:
            //第一个参数是模型对象 第二个参数是映射的目标对象
            //俩个对象中相同的字段会自动映射
            ////ForMember:
            ///第一个参数是 目标对象dto
            ///第二个参数是原始数据模型   进行修改和 赋值给对应的字段
            ///将数据库里的数据通过Molde映射到Dto中 然后展示在界面
            CreateMap<TouristRoute, TouristeRouteDto>().ForMember(
             dest => dest.Price,
             opt => opt.MapFrom(src => src.Originalprice * (decimal)(src.DiscountPresent ?? 1))
             ).ForMember(
                dest => dest.travelDays,
                opt => opt.MapFrom(src => src.travelDays.ToString())
                ).ForMember(
                dest => dest.TripType,
                opt => opt.MapFrom(src => src.TripType.ToString())
                ).ForMember(
                dest => dest.DepartureCity,
                opt => opt.MapFrom(src => src.DepartureCity.ToString())
                );



            ///上面修改的数据  等同于
            /// Price=touristeRouteFromRepo.Originalprice*(decimal)(touristeRouteFromRepo.DiscountPresent ?? 1),//变量定义中含有两个问号，意思是取所赋值??左边的，如果左边为null，取所赋值??右边的。   DiscountPresent是折扣
            ///  travelDays = touristeRouteFromRepo.travelDays.ToString(),
            /// TripType = touristeRouteFromRepo.TripType.ToString(),
            ///   DepartureCity = touristeRouteFromRepo.DepartureCity.ToString()
            ///   省去了相同字段手动赋值的麻烦
            ///   相同字段和不想修改的数据  可以自动映射

            //将前端接收到的创建数据通过Dto映射到Molde中
            CreateMap<TouristRouteForCreationDto, TouristRoute>().ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => Guid.NewGuid())
                );

            CreateMap<TouristRouteForUpdateDto, TouristRoute>();

            CreateMap<TouristRoute, TouristRouteForUpdateDto>();

        }
    }
}
