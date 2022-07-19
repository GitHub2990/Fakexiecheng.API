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

        //��ȡ�����ļ���Ϣ appsettings.json
        public IConfiguration Configuration { get; }

        //���ù��캯������Զ�ע�������ļ���Ϣ
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        //ע��������������
        public void ConfigureServices(IServiceCollection services)
        {
            ///ע�������֤�ķ�������  
            ///�������������ֱ����û�����ģ�ͺͽ�ɫģ��
            ///AddEntityFrameworkStores ͨ����������������ݿ������Ķ��󡣷���Ϊ��Ӧ�������Ķ���
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();


            //ע��JWT��֤��������  AddAuthenticationע�������֤���� ����Ϊ��֤����
            //AddJwtBearer  ����jwt��֤  ����Ϊί��
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                //��ȡ��Կ
                var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //��֤token������  ֻ�к��Issuer������Token�ű�����
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],
                    //token������
                    ValidateAudience = true ,
                    ValidAudience=Configuration["Authentication:Audience"],
                    //��֤token�Ƿ����
                    ValidateLifetime=true ,
                    //����˽Կ������
                    IssuerSigningKey=new SymmetricSecurityKey(secretByte)

                };
            });
         
            


            //ע��MVC���������
            //AddXmlDataContractSerializerFormatters  �÷��������Խ�������ͷ�� Accept��xml�ĸ�ʽ����������xml��ʽ����
            //
            services.AddControllers(
                setupAction => {
                    //ReturnHttpNotAcceptable���Ϊflase http����ͷ���Accept������ʲôֵ ��������Ĭ�Ϸ���Json��ʽ���ݣ������true��ֻ���ط�������������������  ĿǰĬ��Json  ������һ��xml  ������ʽ  �򷵻�406
                    setupAction.ReturnHttpNotAcceptable = true;
                }//ע��json���л�����
                ).AddNewtonsoftJson(setupAction => {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction => {
                    //InvalidModelStateResponseFactory��Чģ��״̬ת�ƹ���
                    setupAction.InvalidModelStateResponseFactory = context => {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "����ν",
                            Title = "������֤ʧ��",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "�뿴��ϸ˵��",
                            Instance = context.HttpContext.Request.Path   //����·��
                        };
                        //����׷��ID
                        problemDetail.Extensions.Add("traeId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            //��������    ���ڷ���ǰ��
                            ContentTypes = { "application/problem+json" }
                        };
                     };
                });//ConfigureApiBehaviorOptions  ��֤�����Ƿ�Ƿ���һ������
            //ע�����ݲֿ�ķ�������   ITouristRouteRepositoryҪ��ӵķ�������͡�   MockTouristRouteRepositoryҪʹ�õ�ʵ�ֵ�����
            //����ע�������2������
            //services.AddSingleton  //�Ṳ������ͨ��
            //services.AddScoped //�Ƚϸ���Щ

            //AddTransient  AddScoped AddSingleton���ֲ�ͬ����������     ����2�����Ͳ�������ʶ���� ÿ����һ�������ӿڱ����ã����ǵ�IOC��ת���ƾͻ᷵��ʵ������ӿڵ���  ���ǵڶ���������ʵ��
            //������� û��ǿ����
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();//ÿ�η�������ͻᴴ��һ���µĲֿ�   �����������Զ�ע��

            //ע��������   ��Ҫnuget  EntityFrameworkCore.SqlServer

            //services.AddDbContext<AppDbContext>(option => {
            //    option.UseSqlServer(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=FakexiechengDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //});

            //ͨ�������ļ�ע�������������ַ���
            services.AddDbContext<AppDbContext>(option => {

            option.UseSqlServer(Configuration["DbContext:ConnectionString"]);

            });

            //ע���Զ�ӳ�����   //AppDomain.CurrentDomain.GetAssemblies()��ȡ��Ŀ����
            //ɨ��profile�ļ�  
            //��Ҫnuget  AutoMapper �������ƿ�������
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //�����м�� ����ͨ��   ����http����ͨ��  ͨ��IApplicationBuilder  �����м��    IWebHostEnvironment ��������
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {  
            //����ǿ�������   ������������ǿ��Կ�������ԭ�� ������������� �򿴲���  
            //������������Ŀ����  Debug�����   Ŀǰ������ģʽ    ����ʹ�� ����ģʽ
            if (env.IsDevelopment())
            {
                //����ǿ��������� �Ĵ�����Ϣû�б�����  ����ͨ�����м�� �׳���ϸ������Ϣ��ҳ��
                app.UseDeveloperExceptionPage();
            }
            //Ϊ��ǰ����·�����þ�̬�ļ�����   ����wwwrote�����ͼƬ  ��JS CSS��
            // app.UseStaticFiles();
            //������ڽ� HTTP �����ض��� HTTPS ���м����
            // app.UseHttpsRedirection();
            //�⽫���������֤����
            //app.UseAuthentication();
            //·���м��
            app.UseRouting();  //��·�ɣ������ģ�

            app.UseAuthentication();//����֤������˭��

            app.UseAuthorization();//����Ȩ�����ܸ�ʲô��  ��ʲôȨ�ޣ�

            //ͨ��UseEndpoints�м������·��   MapGet����ӳ��
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


                //����mvc��·��ӳ���м��
                endpoints.MapControllers();
            });



        }
    }
}
