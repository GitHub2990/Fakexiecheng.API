//using Fakexiecheng.API.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Fakexiecheng.API.services
//{
//    //Mock 假数据
//    public class MockTouristRouteRepository : ITouristRouteRepository
//    {
//        //私有列表
//        private List<TouristRoute> _routes;

//        public MockTouristRouteRepository() {
//            //如果列表为空
//            if (_routes == null) 
//            {
//                //初始化数据
//                InitializeTouristRoutes();
//            }
//        }

//        private void InitializeTouristRoutes() 
//        {
//            _routes = new List<TouristRoute> { new TouristRoute { Id=Guid.NewGuid(),Title="黄山",Description="黄山真好玩",Originalprice=1299,Features="<p>吃住行游购物娱乐</p>",Fees="<p>交通费用自理</p>",Notes="<p>小心危险</p>"},new TouristRoute { Id = Guid.NewGuid(), Title = "华山", Description = "华山真好玩", Originalprice = 1299, Features = "<p>吃住行游购物娱乐</p>", Fees = "<p>交通费用自理</p>", Notes = "<p>小心危险</p>" } };
//        }

//        public TouristRoute GetTouristRoute(Guid touristRouteId)
//        {
//            //Linq  返回指点匹配的数据
//            return _routes.FirstOrDefault(n => n.Id == touristRouteId);
//        }

//        public IEnumerable<TouristRoute> GetTouristRoutes()
//        {
//            //返回所有的列表
//            return _routes;
//        }
//    }
//}
