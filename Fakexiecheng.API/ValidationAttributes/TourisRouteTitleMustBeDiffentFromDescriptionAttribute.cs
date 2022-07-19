using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Fakexiecheng.API.Dtos;

namespace Fakexiecheng.API.ValidationAttributes
{
    public class TourisRouteTitleMustBeDiffentFromDescriptionAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //通过上下文关系 获得当前的对象
            var touriRouteDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;
            if (touriRouteDto.Title == touriRouteDto.Description)
            {
                //yield return 保存当前函数的状态，下次调用时继续从当前位置处理。具体看云盘.net  有实验验证
                     return new ValidationResult(
                    "路线名称必须和路线描述不同",
                    new[] { "TouristRouteForCreationDto" }
                    );
            }

            return ValidationResult.Success;
        }

    }
}
