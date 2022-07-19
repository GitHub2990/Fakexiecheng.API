using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fakexiecheng.API.ResourceParameters
{
    public class TouristRouteResourcePramaters
    {
        //title搜索参数
        public string keyword { get; set; }

        //RatingOperator和RatingValue是对参数Rating的分割
        public string RatingOperator { get; set; }
        public int? RatingValue { get; set; }
        private string _rating;
        //       当读取属性时，执行 get 访问器的代码块；当向属性分配一个新值时，执行 set 访问器的代码块。
        //评分参数
        //http发送请求后  对象会把对应的参数映射过来，然后进行写的操作
        public string Rating {
            get { return _rating; }
            set {
                //如果Rating不是空   就正则
               
                if (!string.IsNullOrWhiteSpace(value)) 
                {

                    //正则表达式   对rating进行字符分割()()第一个括号表达式提取字母。第二个括号提取数字
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = int.Parse(match.Groups[2].Value);
                    }
                }
                
                //value是Rating的一个变量 负责接收外界数据   value ==Rating
                _rating = value;
            } }

    }
}
