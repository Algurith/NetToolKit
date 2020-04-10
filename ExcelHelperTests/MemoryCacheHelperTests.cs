using MemoryCacheProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExcelHelperTests
{
    [TestClass()]
    public class MemoryCacheHelperTests
    {

        [TestMethod()]
        public void GetOrSetCacheItemTest() 
        {
            var key1 = "key1";
            var key2 = "key2";
            var slidingExpiration = new TimeSpan(0, 3, 0);
            var data1 = MemoryCacheHelper.GetOrSetCacheItem(key1, CreateData);
            var data2 = MemoryCacheHelper.GetOrSetCacheItem(key2, CreateData, slidingExpiration);

            //GetOrRemoveCacheItemTest();
            //Console.WriteLine($"count1：{data1.Count}， count2：{data2.Count}");
            //for (var i = 0; i <= 10; i++) 
            //{
            //    System.Threading.Thread.Sleep(5000);
            //}
            //data1 = MemoryCacheHelper.MemoryCacheHelper.GetOrSetCacheItem(key1, CreateData);
            //data2 = MemoryCacheHelper.MemoryCacheHelper.GetOrSetCacheItem(key2, CreateData, slidingExpiration);
            //Console.WriteLine($"count1：{data1.Count}， count2：{data2.Count}");
        }

        private List<string> CreateData() 
        {
            return new List<string> { "key1", "key2" };
        }

        [TestMethod()]
        public void GetOrRemoveCacheItemTest()
        {
            var key = "key2";

            var data = MemoryCacheHelper.GetOrRemoveCacheItem<List<string>>(key);
            Console.WriteLine($"count1：{data.Count}");
        }
    }
}
