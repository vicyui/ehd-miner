using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace EHDMiner
{
    public class DataTableConverterHelper<T> where T : new()

    {
        public static IList<T> ConvertToModelList(DataTable dt)
        {
            //定义集合
            List<T> list = new List<T>();
            //获得此模型的类型
            Type type = typeof(T);
            //定义一个临时变量
            string tempName = string.Empty;
            //遍历Datatable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                //获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    //将属性名称赋值给临时变量
                    tempName = pi.Name;
                    //检查DataTable是否包含此列（列名==对象的属性名）
                    if (dt.Columns.Contains(tempName))
                    {
                        //判断此属性是否有Setter
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出
                        //取值
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性
                        if (value != DBNull.Value)
                        {
                            //加一重if判断，如果属性值是int32类型的，就进行一次强制转换
                            if (pi.GetMethod.ReturnParameter.ParameterType.Name == "Int32")
                            {
                                value = Convert.ToInt32(value);
                            }
                            pi.SetValue(t, value, null);
                        }

                    }
                }
                //对象添加到泛型集合中
                list.Add(t);
            }
            return list;

        }
    }
}
