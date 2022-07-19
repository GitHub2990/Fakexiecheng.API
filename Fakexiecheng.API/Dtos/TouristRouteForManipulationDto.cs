using Fakexiecheng.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    [TourisRouteTitleMustBeDiffentFromDescriptionAttribute]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "Title 不可以为空")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string  Description { get; set; }
        //原价    
        public decimal Price { get; set; }
        //public decimal Originalprice { get; set; }
        //折扣

        //public double? DiscountPresent { get; set; }
        //创建时间
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        //发团时间
        public DateTime? DepartuerTime { get; set; }

        //卖点介绍

        public string Features { get; set; }

        //线路说明

        public string Fees { get; set; }

        //记录

        public string Notes { get; set; }


        //评分
        public double? Rating { get; set; }

        //旅游天数
        public string travelDays { get; set; }

        //旅游类型
        public string TripType { get; set; }

        //出发地
        public string DepartureCity { get; set; }

        //对照片类进行赋值  初始化
        public ICollection<TourisRoutePictureForCreationDto> TouristRoutePictures { get; set; }
        = new List<TourisRoutePictureForCreationDto>();

    }
}
