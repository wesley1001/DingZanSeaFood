namespace BreezeShop.Core.UserTrace
{
    public abstract class BaseBrowseTrace
    {
        protected BaseBrowseTrace()
        {
            LogUserGuid();

            if (LoadedHander != null)
            {
                LoadedHander();
            }
        }

        protected delegate void Loaded();
        protected event Loaded LoadedHander;

        /// <summary>
        /// 获取或设置用户的唯一标识
        /// </summary>
        public string UserGuid { get; set; }

        /// <summary>
        /// 记录用户唯一标识
        /// </summary>
        public void LogUserGuid()
        {
            if (!string.IsNullOrEmpty(UserGuid)) return;

            UserGuid = CreateGuid();
        }

        /// <summary>
        /// 创建用户Id
        /// </summary>
        /// <returns></returns>
        protected abstract string CreateGuid();


    }
}
