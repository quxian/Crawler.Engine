using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    /// <summary>
    /// 下载结果处理模块
    /// </summary>
    /// <typeparam name="TDownloadResult">下载结果类型</typeparam>
    /// <typeparam name="UResult">本模块要产生的结果类型，要传递到管道</typeparam>
    /// <typeparam name="VUrls">本模块从下载结果找到的Urls类型</typeparam>
    public interface IDownloadResultProcesser<TDownloadResult, UResult, VUrls> : IDisposable {
        /// <summary>
        /// 触发处理结果传递给管道事件
        /// </summary>
        event Action<UResult> OnResultToPipeline;

        /// <summary>
        /// 模块关闭时，保存状态
        /// </summary>
        void SavingState();

        /// <summary>
        /// 找到下载结果中的所有Urls之后触发该事件
        /// </summary>
        event Action<VUrls> OnFindAllUrls;

        /// <summary>
        /// 给该模块添加下载结果
        /// </summary>
        /// <param name="downloadResult">下载模块产生的结果</param>
        /// <returns>该模块</returns>
        IDownloadResultProcesser<TDownloadResult, UResult, VUrls> AddDownloadResult(TDownloadResult downloadResult);

        /// <summary>
        /// 添加具体行为到找到所有Urls事件
        /// </summary>
        /// <param name="onFildAllUrls">具体处理行为</param>
        /// <returns>该模块</returns>
        IDownloadResultProcesser<TDownloadResult, UResult, VUrls> AddFindAllUrlsEventListens(Action<VUrls> onFildAllUrls);

        /// <summary>
        /// 添加具体的管道处理行为到管道事件
        /// </summary>
        /// <param name="onResultToPipeline">具体的处理行为</param>
        /// <returns>该模块</returns>
        IDownloadResultProcesser<TDownloadResult, UResult, VUrls> AddResultToPipelineEventListens(Action<UResult> onResultToPipeline);

        /// <summary>
        /// 运行该模块
        /// </summary>
        void Run();
    }
}
