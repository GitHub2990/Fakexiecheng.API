using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fakexiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Fakexiecheng.API.Database
{

    //上下文类
    public class AppDbContext:IdentityDbContext<ApplicationUser>//DbContext
    {
        //base关键字等同于父类
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
           
        }

        //DbSet 对对象的映射   注意名称是复数形式
        //旅游路径
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        //旅游照片
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        //购物车表
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        //历史商品信息保存表
        public DbSet<LineItem> LineItems { get; set; }



        //控制数据库和数据映射   通过数据模型 创建数据库表
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //将此实体配置为具有种子数据。 它用于生成数据运动迁移。
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = Guid.NewGuid(),
            //    Title="cesititle",
            //    Description="shuoming",
            //    Originalprice=0,
            //    CreateTime=DateTime.UtcNow

            //});

            //批量读取文件中的数据，进行数据迁移   路径=文件夹地址(通过反射获取)+文件地址
            //Assembly.GetExecutingAssembly().Location获取当前文件程序集路径
            var TouristRouteJsonData=  File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"/Database/touristRoutesMockData.json");
            //对读取到的Json数据进行反序列化处理   需要在nuget中安装newtonsoft.json
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(TouristRouteJsonData);
            //将种子数据进行数据迁移
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);


            //添加旅游图片信息
            //Assembly.GetExecutingAssembly().Location获取当前文件程序集路径
            var TouristRoutePicturesJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            //对读取到的Json数据进行反序列化处理   需要在nuget中安装newtonsoft.json
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(TouristRoutePicturesJsonData);
            //将种子数据进行数据迁移
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);


            //初始化用户与角色的种子数据
            //1.更新用户与角色的外键
            //HasMany 表示一对多的关系  这个关系可以被映射为roles 每一个role都有一个外键关系 WithOne  而这个外键关系使用的是UserID  这个ID是必须的  所以加上IsRequired()
            modelBuilder.Entity<ApplicationUser>(u => u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());

            //2.添加管理员角色

            //给角色添加主键
            var adminRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()//大写
                }
                ); 
            //3.添加用户
            var adminUserID = Guid.NewGuid().ToString();
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserID,
                UserName = "13011@qq.com",
                NormalizedUserName = "13011@qq.com".ToUpper(),
                Email= "13011@qq.com",
                NormalizedEmail= "13011@qq.com".ToUpper(),
                TwoFactorEnabled=false,
                EmailConfirmed=true,
                PhoneNumber="1234567891",
                PhoneNumberConfirmed=false


            };
            //hash密码
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            //4.给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new  IdentityUserRole<string>(){ 

            RoleId=adminRoleId,
            UserId=adminUserID
            
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
