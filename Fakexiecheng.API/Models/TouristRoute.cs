using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakexiecheng.API.Models
{
    public class TouristRoute
    {
        //路线ID
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }
        //原价
        [Column(TypeName = "decimal(18,2)")]
        public decimal Originalprice { get; set; }
        //折扣
        [Range(0.0,1.0)]
        public double? DiscountPresent { get; set; }
        //创建时间
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        //发团时间
        public DateTime? DepartuerTime { get; set; }

        //卖点介绍
        [MaxLength]
        public string Features { get; set; }

        //线路说明
        [MaxLength]
        public string Fees { get; set; }

        //记录
        [MaxLength]
        public string Notes { get; set; }

        //建立外键联系
        public ICollection<TouristRoutePicture> TouristRoutePictures { get; set; }
        = new List<TouristRoutePicture>();
        //评分
        public double? Rating { get; set; }

        //旅游天数
        public TravelDays? travelDays { get; set; }

        //旅游类型
        public TripType? TripType { get; set; }

        //出发地
        public DepartureCity? DepartureCity { get; set; }
    }
}
