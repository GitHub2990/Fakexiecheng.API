using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Models
{
    public class TouristRoutePicture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   //设置自增
        public int Id{ get; set; }
        //图片路径
        [MaxLength(100)]
        public string Url { get; set; }

        //外键
        //关联的导航属性或一个或多个关联的外键的名称ps:外键名称等于你要关联的类名+类里的主键名称
       [ForeignKey("TouristRouteId")]
        public Guid TouristRouteId { get; set; }

        //建立外键联系
        public TouristRoute TouristRoute { get; set; }
    }
}
