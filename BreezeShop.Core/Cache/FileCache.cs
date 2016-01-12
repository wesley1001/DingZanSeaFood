using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.Caching.Interfaces;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;

namespace BreezeShop.Core.Cache
{
    public class FileCache<TKeyType> : ICache<TKeyType>
    {
        public FileCache(string cacheName, bool isbinary = false)
        {
            _isBinary = isbinary;
            CacheName = cacheName;
        }

        private static readonly ExceptionLog _log = new ExceptionLog(typeof(FileCache<TKeyType>));
        private bool _isBinary;
        protected readonly string CacheName;

        protected static string Path = 
            Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).FullName + @"\cache\";

        private static object operateLock = new object();

        protected string FilePath
        {
            get { return Path + CacheName + @"\"; }
        }

        public virtual IEnumerator<object> GetEnumerator()
        {
            var r = new List<object>();
            Directory.GetFiles(FilePath).ForEach(s => r.Add(File.ReadAllText(s)));

            return r.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>  
        /// 用递归方法删除文件夹目录及文件  
        /// </summary>  
        /// <param name="dir">带文件夹名的路径</param>   
        private static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之   
            {
                foreach (var d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        File.Delete(d);
                    } //直接删除其中的文件                          
                    else
                    {
                        DeleteFolder(d); //递归删除子文件夹   
                    }
                }
                Directory.Delete(dir, true); //删除已空文件夹                   
            }
        }

        public void Clear()
        {
            lock (operateLock)
            {
                try
                {
                    DeleteFolder(FilePath);
                }
                catch (Exception ex)
                {
                   new ExceptionLog().Error(ex);
                }
            }
        }

        protected virtual string GetCacheName(TKeyType key)
        {
            return FilePath + key + ".txt";
        }

        public void Remove(TKeyType key)
        {
            try
            {
                if (File.Exists(GetCacheName(key)))
                {
                    File.Delete(GetCacheName(key));
                }
            }
            catch (Exception ex)
            {
                new ExceptionLog().Error(ex);
            }
        }

        public bool Exists(TKeyType key)
        {
            return File.Exists(GetCacheName(key));
        }

        protected virtual void CreateCachePath(TKeyType Key)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
        }

        public void Add<T>(TKeyType Key, T Value)
        {
            lock (operateLock)
            {
                try
                {
                    CreateCachePath(Key);

                    //当前类型是基础类型或者是string
                    //var ct = typeof (T);
                    //if (ct.IsPrimitive || ct == typeof (string))
                    //{
                    //    File.WriteAllText(GetCacheName(Key), Value.ToString());
                    //}
                    //else
                    //{
                    File.WriteAllText(GetCacheName(Key), _isBinary ? Convert.ToBase64String(Value.SerializeBinary()) : Value.Serialize());
                    //}
                }
                catch (Exception ex)
                {
                    _log.Trace("异常：新增文件缓存：" + CacheName + "，key:" + Key);
                    _log.Error(ex);
                }
            }
        }

        public ValueType1 Get<ValueType1>(TKeyType Key, Func<ValueType1> getItemCallback)
        {
            //当前类型是基础类型或者是string
            //var ct = typeof(ValueType1);

            if (File.Exists(GetCacheName(Key)))
            {
                var data = File.ReadAllText(GetCacheName(Key));
                if (!string.IsNullOrEmpty(data))
                {
                    //if (ct.IsPrimitive || ct == typeof (string))
                    //{
                    //    return data.TryTo<string, ValueType1>();
                    //}

                    return _isBinary
                        ? Convert.FromBase64String(data).Deserialize<ValueType1>()
                        : data.Deserialize<ValueType1>();
                }
            }

            if (getItemCallback != null)
            {
                lock (operateLock)
                {
                    if (!Exists(Key))
                    {
                        var item = getItemCallback.Invoke();
                        if (item!=null)
                        {
                            Add(Key, item);
                            return item;
                        }

                        return default(ValueType1);
                    }

                    var data = File.ReadAllText(GetCacheName(Key));
                    if (!string.IsNullOrEmpty(data))
                    {
                        //if (ct.IsPrimitive || ct == typeof(string))
                        //{
                        //    return data.TryTo<string, ValueType1>();
                        //}

                        return _isBinary
                            ? Convert.FromBase64String(data).Deserialize<ValueType1>()
                            : data.Deserialize<ValueType1>();
                    }
                }
            }

            return default(ValueType1);
        }

        public ICollection<TKeyType> Keys
        {
            get { return Directory.GetDirectories(FilePath).Select(e => e.TryTo<string, TKeyType>()).ToList(); }
        }

        public virtual int Count {
            get {return Directory.GetFiles(FilePath).Length; }
        }

        public object this[TKeyType Key]
        {
            get { return Get<object>(Key, null); }
            set { Add(Key, value); }
        }

    }
}
