using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.DataTypes.ExtensionMethods;

namespace BreezeShop.Core.Cache
{
    public class MultiFolderFileCache : FileCache<string>
    {
        public MultiFolderFileCache(string cacheName, char separtor)
            : base(cacheName)
        {
            _separtor = separtor;
        }

        private readonly char _separtor;

        public override int Count
        {
            get { return Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories).Length; }
        }

        protected override void CreateCachePath(string Key)
        {
            if (Key.IndexOf(_separtor) > 0)
            {
                var spKey = Key.Split(new[] { _separtor }, StringSplitOptions.RemoveEmptyEntries);
                if (!Directory.Exists(FilePath + spKey[0] + @"\"))
                {
                    Directory.CreateDirectory(FilePath + spKey[0] + @"\");
                }
            }

            base.CreateCachePath(Key);
        }

        public override IEnumerator<object> GetEnumerator()
        {
            var r = new List<object>();
            Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories)
                .ForEach(path => r.Add(File.ReadAllText(path)));
            return r.GetEnumerator();
        }

        public IList<object> Fetch(int page, int pageSize, string key)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);
            pageSize = Math.Min(100, pageSize);

            var r = new List<object>();
            string[] files;
            if (key.IndexOf(_separtor) > 0)
            {
                var spKey = key.Split(new[] {_separtor}, StringSplitOptions.RemoveEmptyEntries);
                files = Directory.GetFiles(FilePath + spKey[0] + @"\");
            }
            else
            {
                files = Directory.GetFiles(FilePath);
            }

            files.OrderByDescending(e => e)
                .Skip(page - 1)
                .Take(pageSize)
                .ForEach(path => r.Add(File.ReadAllText(path)));
            return r;
        }

        protected override string GetCacheName(string key)
        {
            if (key.IndexOf(_separtor) > 0)
            {
                var spKey = key.Split(new[] {_separtor}, StringSplitOptions.RemoveEmptyEntries);
                return FilePath + spKey[0] + @"\" + spKey[1] + ".txt";
            }
            
            return base.GetCacheName(key);
        }

    }
}
