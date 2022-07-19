using Fakexiecheng.API.Database;
using Fakexiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 除了添加和删除  其他操作都可以用异步操作。   来提高效率
/// </summary>

namespace Fakexiecheng.API.services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        //获取上下文类
        private readonly AppDbContext _context;

        //在依赖注入的时候  会自动把参数传过来
        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }
        //获取图片
        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId).ToListAsync();
        }

        //获取单个路线
        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {   //  EF 中Include 进行表链接  //FirstOrDefault也是立即执行数据查询  
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n =>  n.Id==touristRouteId);
        }

        //返回全部旅游路线
        public async Task < IEnumerable<TouristRoute>> GetTouristRoutesAsync(
            string keyword, 
            string operatorType,
            int? raringVlaue
         )
        {
            //Include  和Jion 都可以进行表连接   IQueryable将会对数据进行延时处理  只会先生成sql语句  并不会执行操作
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);
            
            //判断搜索内容是否为空
            if (!string.IsNullOrEmpty(keyword)) 
            {
                //对字符串进行空字符去除
                keyword = keyword.Trim();
                result = result.Where(t=>t.Title.Contains(keyword));
            }

            //对评分进行过滤
            if (raringVlaue >= 0) 
            {
                //进行筛选
                result = operatorType switch
                {
                    "largerThan" => result.Where(t => t.Rating >= raringVlaue),
                    "lessThan" => result.Where(t => t.Rating <= raringVlaue),
                    _ => result.Where(t => t.Rating == raringVlaue),
                };
            }



            return await result.ToListAsync();//ToList将会立即执行数据库操作
        }


        /// <summary>
        /// 同步操作
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        //public bool TouristRouteExists(Guid touristRouteId)
        //{
        //    return _context.TouristRoutes.Any(t => t.Id == touristRouteId);
        //}

        //判断路线是否存在
        //异步操作
        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);
        }

        //获取单张图片
        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId) 
        {
            return await _context.TouristRoutePictures.Where(p => p.Id== pictureId).FirstOrDefaultAsync();
        }

        //创建路线
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            //对对象进行检查
            if (touristRoute == null) 
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            //将数据添加进入上下文关系对象中
            _context.TouristRoutes.Add(touristRoute);
             
        }
        //将在此上下文中所做的所有更改保存到数据库中。
        public async Task<bool> SaveAsync()
        {
            //SaveChanges 写入数据库的状态项的数目。
            return (await _context.SaveChangesAsync()>=0);
        }
        /// <summary>
        ///添加照片
        /// </summary>
        /// <param name="TourisRouteId">路线ID</param>
        /// <param name="touristRoutePicture">照片实体类</param>
        public void AddTouristRoutePicture(Guid TourisRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (TourisRouteId == Guid.Empty) 
            {
                throw new ArgumentNullException(nameof(TourisRouteId));
            }
            if (touristRoutePicture==null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = TourisRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);


        }
        //删除单条旅游路线
        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
             _context.TouristRoutes.Remove(touristRoute);
        }

        /// <summary>
        /// 删除路线下的单张照片
        /// </summary>
        /// <param name="Picture">删除的数据对象</param>
        public void DeleteTouristRoutePictuer(TouristRoutePicture Picture)
        {
            _context.TouristRoutePictures.Remove(Picture);
        }

        /// <summary>
        /// 获取旅游列表
        /// </summary>
        /// <param name="ids">旅游ID集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<TouristRoute>> GetTouristRouteByIdListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="touristRoutes">删除的对象列表</param>
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            return await _context.ShoppingCarts.Include(s => s.user).Include(s => s.ShoppingCartItems).ThenInclude(li => li.TouristRoute).Where(s => s.UserId == userId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 创建购物车
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        /// <summary>
        /// 添加Itme进入购物车
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        public async Task AddShoppingCartItem(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }
    }
}
