using System;
using System.Collections.Generic;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;

namespace BreezeShop.Core.Cache
{
    public class NumericalFileCache : FileCache<long>
    {
        public NumericalFileCache(string cacheName, int maxSepartor)
            : base(cacheName)
        {
            _maxSepartor = Math.Min(5000, maxSepartor);
        }

        public int _maxSepartor;

        public override int Count
        {
            get
            {
                var r = Directory.GetFiles(FilePath).Length;
                Directory.GetDirectories(FilePath).ForEach(path =>
                {
                    r += Directory.GetFiles(path).Length;
                });

                return r;
            }
        }

        protected override void CreateCachePath(long Key)
        {
            if (!Directory.Exists(FilePath + Key/_maxSepartor + @"\"))
            {
                Directory.CreateDirectory(FilePath + Key/_maxSepartor + @"\");
            }
        }

        public override IEnumerator<object> GetEnumerator()
        {
            var r = new List<object>();
            Directory.GetFiles(FilePath).ForEach(path => r.Add(File.ReadAllText(path)));
            Directory.GetDirectories(FilePath).ForEach(path =>
            {
                Directory.GetFiles(path).ForEach(s => r.Add(File.ReadAllText(s)));
            });

            return r.GetEnumerator();
        }

        protected override string GetCacheName(long key)
        {
            return FilePath + key / _maxSepartor + @"\" + key + ".txt";
        }

    }
}
