using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Fakexiecheng.API.Helper
{
    //IModelBinder  自定义的数据绑定模型接口
    public class ArrayModelBinder : IModelBinder
    {

        /// <summary>
        /// 尝试绑定模型。
        /// </summary>
        /// <param name="bindingContext">上下文绑定的数据对象  包含了绑定对象的所有的元数据</param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //检查数据类型是否符合要绑定的类型
            if (!bindingContext.ModelMetadata.IsEnumerableType) 
            {
                //类型不符合 绑定失败 返回任务
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //如果符合类型 通过ValueProvider获取数据对象然后调用GetValue传递数据模型名称转换为字符串
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            //如果输出的数据是空或者空字符串
            if (string.IsNullOrWhiteSpace(value)) 
            {   //将结果视为绑定成功  同时以null作为最终结果 进行输出 然后  返回任务
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //如果上下文输出的字符串不为空，或者不为空字符串
            ///这时候我们可以尝试把字符串转换为相应的数据类型
            ///本案例 类型为Guid
            ///这将使用C#反射机制
            ///调用GetTypeInfo() 获得一个泛型参数的类型，这个类型就是你数据原形里对应的类型。
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            //接着进行类型转换 使用TypeDescriptor进行新的数据转换
            var converter = TypeDescriptor.GetConverter(elementType);

            //然后我们通过类型转换工具转化为Guid的对象
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => converter.ConvertFromString(x.Trim())).ToArray();
            //接着使用反射把所有的Guid对象赋值到一个新的数组中
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            //把返回结果设为成功
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;

        }
    }
}
