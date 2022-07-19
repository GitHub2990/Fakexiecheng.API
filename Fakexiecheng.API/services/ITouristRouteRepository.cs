using Fakexiecheng.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fakexiecheng.API.services
{
    //数据仓库  仓库模式
    //添加public方便外部引用
    public interface ITouristRouteRepository
    {
        //返回所有旅行路线的数据    参数为搜索的title
        Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword,string operatorType, int? raringVlaue);
        //特定旅游路线
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);

        //查看路线是否存在
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);

        //获取照片
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);

        //获取单张照片

        Task<TouristRoutePicture> GetPictureAsync(int pictureId);

        //添加路线
        void AddTouristRoute(TouristRoute touristRoute);
        //添加照片
        void AddTouristRoutePicture(Guid TourisRouteId ,TouristRoutePicture touristRoutePicture);

        //通过ID删除旅游路线
        void DeleteTouristRoute(TouristRoute touristRoute);

        //删除路线下的单张照片
        void DeleteTouristRoutePictuer(TouristRoutePicture Picture);

        //批量删除
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);

        //获取旅游数据列表
        Task<IEnumerable<TouristRoute>> GetTouristRouteByIdListAsync(IEnumerable<Guid> ids);

        //获取购物车
        Task<ShoppingCart> GetShoppingCartByUserId(string userId);

        //添加购物车
        Task CreateShoppingCart(ShoppingCart shoppingCart);

        //把LienItme(历史数据)添加到购物车
        Task AddShoppingCartItem(LineItem lineItem);
        ////将在此上下文中所做的所有更改保存到数据库中。
        Task<bool> SaveAsync();
    }
}
