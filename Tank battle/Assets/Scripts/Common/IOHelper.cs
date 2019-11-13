using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LS.Common
{
    /// <summary>
    /// IO辅助类
    /// JsonNet. 转为Json字符串
    /// JsonNet. Version  : 9.0.1
    /// Time:  2019.7.28
    /// </summary>
    public static class IOHelper 
    {
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="fileName">文件路径名</param>
        /// <param name="obj">存储对象</param>
        public static void SetData(string fileName,object obj)
        {
            string toSave = SerializeObject(obj);
            StreamWriter writer = File.CreateText(fileName);
            writer.Write(toSave);
            writer.Close();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fileName">文件路径名</param>
        /// <param name="type">数据类型</param>
        public static object GetData(string fileName,Type type)
        {
            StreamReader reader = File.OpenText(fileName);
            string data = reader.ReadToEnd();
            reader.Close();
            return DeserializeObject(data, type);
        }

        /// <summary>
        /// 将对象转化为字符串
        /// </summary>
        /// <param name="obj">目标对象</param>
        public static string SerializeObject(object obj)
        {
            string serializedString = string.Empty;
            serializedString = JsonConvert.SerializeObject(obj);
            if(serializedString==string.Empty || serializedString == null)
            {
                Debug.LogWarning("IOHelper/SerializeObject Warning : the serialized string is null or empty ,the object is " + obj);
            }
            return serializedString;
        }

        /// <summary>
        /// 将字符串转换为目标对象
        /// </summary>
        /// <param name="serializedString">序列化字符串</param>
        /// <param name="type">对象类型</param>
        public static object DeserializeObject(string serializedString,Type type)
        {
            object obj = null;
            obj = JsonConvert.DeserializeObject(serializedString, type);

            if (obj == null)
            {
                Debug.LogWarning("IOHelper/DeserializeObject Warning : the deserialized string to object is null !");
            }
            return obj;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        public static bool IsFileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        public static bool IsDirectoryExists(string fileName)
        {
            return Directory.Exists(fileName);
        }

        /// <summary>
        /// 创建一个文本文件    
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="content">文件内容</param>
        public static void CreateFile(string fileName, string content)
        {
            StreamWriter streamWriter = File.CreateText(fileName);
            streamWriter.Write(content);
            streamWriter.Close();
        }

        /// <summary>
        /// 创建一个文件夹
        /// </summary>
        public static void CreateDirectory(string fileName)
        {
            //文件夹存在则返回
            if (IsDirectoryExists(fileName))
                return;
            Directory.CreateDirectory(fileName);
        }

    }

}