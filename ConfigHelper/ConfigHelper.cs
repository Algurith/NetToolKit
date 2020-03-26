using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;

namespace ConfigHelper
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public class ConfigHelper
    {
        private readonly string _currentConfigFullPath;

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ValueDic =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        /// <summary>
        /// 初始化配置帮助类实例，默认路径为AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        public ConfigHelper(string fileName)
        {
            _currentConfigFullPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\\{fileName}";
        }

        /// <summary>
        /// 初始化配置帮助类实例
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <param name="fileName">配置文件名</param>
        public ConfigHelper(string path, string fileName)
        {
            _currentConfigFullPath = path + fileName;
        }

        /// <summary>
        /// 获取指定的配置文件中的属性
        /// </summary>
        /// <param name="key">属性Key</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            var file = ValueDic.GetOrAdd(_currentConfigFullPath,
                filePath => new ConcurrentDictionary<string, string>());
            return file.GetOrAdd(key, k => GetAttributeValue(_currentConfigFullPath, key));
        }

        /// <summary>  
        /// 获取配置文件的属性  
        /// </summary>  
        /// <param name="file"></param>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        private static string GetAttributeValue(string file, string key)
        {
            var value = string.Empty;

            if (!File.Exists(file)) return value;

            var xml = new XmlDocument();

            xml.Load(file);

            var xNode = xml.SelectSingleNode("//appSettings");

            var element = (XmlElement)xNode?.SelectSingleNode("//add[@key='" + key + "']");

            if (element == null) return null;

            value = element.GetAttribute("value");

            return value;
        }
    }
}
