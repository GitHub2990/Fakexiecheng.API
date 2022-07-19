using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fakexiecheng.API.Dtos;
using Fakexiecheng.API.Models;

namespace Fakexiecheng.API.Profiles
{
    public class TouristRoutePictureProfile:Profile
    {
        public TouristRoutePictureProfile() 
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
            CreateMap<TourisRoutePictureForCreationDto, TouristRoutePicture>();
            CreateMap<TouristRoutePicture, TourisRoutePictureForCreationDto>();
        }
    }
}
