using Fakexiecheng.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    
    //用于数据写入的Dto          IValidatableObject做数据校验的接口
    public class TouristRouteForCreationDto:TouristRouteForManipulationDto //:IValidatableObject
    {
       

        //通过Validate 方法可以实现上下文中多个属性的校验，
        //public IEnumerable<ValidationResult> Validate(
        //    ValidationContext validationContext)
        //{
        //    if (Title == Description) 
        //    {
        //        //yield return 保存当前函数的状态，下次调用时继续从当前位置处理。具体看云盘.net  有实验验证
        //        yield return new ValidationResult(
        //            "路线名称必须和路线描述不同",
        //            new[] { "TouristRouteForCreationDto" }
        //            );
        //    }
        //}
    }
}
