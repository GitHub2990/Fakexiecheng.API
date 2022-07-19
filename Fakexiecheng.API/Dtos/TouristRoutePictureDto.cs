using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Dtos
{
    public class TouristRoutePictureDto
    {
      
        public int Id { get; set; }
        //图片路径
    
        public string Url { get; set; }

        //外键
        //关联的导航属性或一个或多个关联的外键的名称ps:外键名称等于你要关联的类名+类里的主键名称
   
        public Guid TouristRouteId { get; set; }
    }
}
