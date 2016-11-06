using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    /// <summary>
    /// 下载模块
    /// </summary>
    /// <typeparam name="TDownloadResult">下载结果</typeparam>
    /// <typeparam name="VUrls">待下载的Urls类型</typeparam>
    public interface IDownloader<TDownloadResult, VUrls> : IDisposable {
        /// <summary>
        /// 下载结果触发事件
        /// </summary>
        event Action<TDownloadResult> OnDownloadResult;

        /// <summary>
        /// 模块关闭时，保存状态
        /// </summary>
        void SavingState();

        /// <summary>
        /// 运行模块
        /// </summary>
        void Run();

        /// <summary>
        /// 添加urls到下载队列
        /// </summary>
        /// <param name="urls">待添加到队列里的urls</param>
        /// <returns>返回模块本身</returns>
        IDownloader<TDownloadResult, VUrls> AddUrls(VUrls urls);

        /// <summary>
        /// 添加具体事件到下载结果事件上
        /// </summary>
        /// <param name="onDownloadResult">待添加的事件</param>
        /// <returns>返回模块本身</returns>
        IDownloader<TDownloadResult, VUrls> AddDownloadResultEventListens(Action<TDownloadResult> onDownloadResult);
    }
}
