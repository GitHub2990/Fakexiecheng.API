using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fakexiecheng.API.services;
using Fakexiecheng.API.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Fakexiecheng.API.Models;

namespace Fakexiecheng.API
{
    public class Startup
    {

        //获取配置文件信息 appsettings.json
        public IConfiguration Configuration { get; }

        //调用构造函数后会自动注入配置文件信息
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        //注入服务组件的依赖
        public void ConfigureServices(IServiceCollection services)
        {
            ///注册身份认证的服务依赖  
            ///泛型俩个参数分别是用户数据模型和角色模型
            ///AddEntityFrameworkStores 通过这个函数连接数据库上下文对象。泛型为对应的上下文对象。
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();


            //注入JWT验证依赖服务  AddAuthentication注册身份认证服务 参数为认证类型
            //AddJwtBearer  配置jwt认证  参数为委托
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                //获取密钥
                var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //验证token发布者  只有后端Issuer发出的Token才被接受
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],
                    //token持有者
                    ValidateAudience = true ,
                    ValidAudience=Configuration["Authentication:Audience"],
                    //验证token是否过期
                    ValidateLifetime=true ,
                    //传如私钥并加密
                    IssuerSigningKey=new SymmetricSecurityKey(secretByte)

                };
            });
         
            


            //注册MVC控制器组件
            //AddXmlDataContractSerializerFormatters  让服务器可以接收请求头里 Accept以xml的格式并把数据以xml格式返回
            //
            services.AddControllers(
                setupAction => {
                    //ReturnHttpNotAcceptable如果为flase http请求头里的Accept不管是什么值 服务器都默认返回Json格式数据，如果是true则只返回服务器开启的数据类型  目前默认Json  开启了一个xml  其他格式  则返回406
                    setupAction.ReturnHttpNotAcceptable = true;
                }//注入json序列化服务。
                ).AddNewtonsoftJson(setupAction => {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction => {
                    //InvalidModelStateResponseFactory无效模型状态转移工厂
                    setupAction.InvalidModelStateResponseFactory = context => {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "无所谓",
                            Title = "数据验证失败",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "请看详细说明",
                            Instance = context.HttpContext.Request.Path   //请求路径
                        };
                        //加上追踪ID
                        problemDetail.Extensions.Add("traeId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            //问题类型    用于返回前端
                            ContentTypes = { "application/problem+json" }
                        };
                     };
                });//ConfigureApiBehaviorOptions  验证数据是否非法的一个过程
            //注册数据仓库的服务依赖   ITouristRouteRepository要添加的服务的类型。   MockTouristRouteRepository要使用的实现的类型
            //依赖注入的其他2个方法
            //services.AddSingleton  //会共享数据通道
            //services.AddScoped //比较复杂些

            //AddTransient  AddScoped AddSingleton三种不同的生命周期     后面2个泛型参数的意识就是 每当第一个参数接口被调用，我们的IOC反转机制就会返回实现这个接口的类  就是第二个参数的实例
            //方便解耦 没有强依赖
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();//每次发起请求就会创建一个新的仓库   请求结束后会自动注销

            //注入上下文   需要nuget  EntityFrameworkCore.SqlServer

            //services.AddDbContext<AppDbContext>(option => {
            //    option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=FakexiechengDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //});

            //通过配置文件注入上下文链接字符串
            services.AddDbContext<AppDbContext>(option => {

            option.UseSqlServer(Configuration["DbContext:ConnectionString"]);

            });

            //注入自动映射服务   //AppDomain.CurrentDomain.GetAssemblies()获取项目程序集
            //扫秒profile文件  
            //需要nuget  AutoMapper 具体名称看依赖包
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //创建中间件 请求通道   配置http请求通道  通过IApplicationBuilder  创建中间件    IWebHostEnvironment 环境变量
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {  
            //如果是开发环境   浏览器报错我们可以看到报错原因 如果是生产环境 则看不见  
            //环境变量在项目属性  Debug下面改   目前是生产模式    开发使用 开发模式
            if (env.IsDevelopment())
            {
                //如果是开发环境下 的错误信息没有被接收  可以通过此中间件 抛出详细错误信息在页面
                app.UseDeveloperExceptionPage();
            }
            //为当前请求路径启用静态文件服务   比如wwwrote里面的图片  和JS CSS等
            // app.UseStaticFiles();
            //添加用于将 HTTP 请求重定向到 HTTPS 的中间件。
            // app.UseHttpsRedirection();
            //这将启用身份验证功能
            //app.UseAuthentication();
            //路由中间件
            app.UseRouting();  //（路由）你在哪？

            app.UseAuthentication();//（认证）你是谁？

            app.UseAuthorization();//（授权）你能干什么？  有什么权限？

            //通过UseEndpoints中间件进行路由   MapGet进行映射
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/test", async context =>
                //{
                //    throw new Exception("test");
                //    //await context.Response.WriteAsync("Hello from test!");
                //});
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});


                //启动mvc的路由映射中间件
                endpoints.MapControllers();
            });



        }
    }
}
