using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    /// <summary>
    /// Urls管理模块
    /// </summary>
    /// <typeparam name="TUrl">Url类型</typeparam>
    public interface IScheduler<TUrl> : IDisposable {
        /// <summary>
        /// Url出队触发事件
        /// </summary>
        event Action<List<TUrl>> OnUrlDequeue;

        /// <summary>
        /// 模块关闭时，保存状态
        /// </summary>
        void SavingState();

        /// <summary>
        /// Url过滤规则
        /// </summary>
        event Func<TUrl, bool> Filter;

        /// <summary>
        /// 管理模块添加Urls
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        IScheduler<TUrl> AddUrls(List<TUrl> urls);

        /// <summary>
        /// 添加具体处理事件到Url出队事件上
        /// </summary>
        /// <param name="onUrlDequeue"></param>
        /// <returns></returns>
        IScheduler<TUrl> AddUrlDequeueEventListens(Action<List<TUrl>> onUrlDequeue);

        /// <summary>
        /// 添加Urls过滤规则
        /// </summary>
        /// <param name="filter">过滤规则</param>
        /// <returns>返回该模块</returns>
        IScheduler<TUrl> AddUrlsFilter(Func<TUrl, bool> filter);

        /// <summary>
        /// 运行该模块
        /// </summary>
        void Run();

    }
}
