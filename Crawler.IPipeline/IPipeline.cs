using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    /// <summary>
    /// 下载结果处理模块(管道)
    /// </summary>
    /// <typeparam name="TPreviousResult">待处理对象，来自上一个管道</typeparam>
    /// <typeparam name="UResult">处理过的结果对象:T->U，经过本管道产生的结果</typeparam>
    public interface IPipeline<TPreviousResult, UResult> : IDisposable {
        /// <summary>
        /// 管道结束时，释放资源事件
        /// </summary>
        event Action OnDispose;

        /// <summary>
        /// 本管道输出结果事件
        /// </summary>
        event Action<UResult> OnResult;

        /// <summary>
        /// 模块关闭时，保存状态
        /// </summary>
        void SavingState();

        /// <summary>
        /// 对上一个管道的结果进行处理
        /// </summary>
        /// <param name="previousResult">上一个管道的结果</param>
        void Extract(TPreviousResult previousResult);

        /// <summary>
        /// 挂载下一个管道
        /// </summary>
        /// <typeparam name="VNextResult">下一个管道预输出的结果</typeparam>
        /// <param name="nextPipeline">下一个管道对象</param>
        /// <returns>本管道</returns>
        IPipeline<TPreviousResult, UResult> NextPipeline<VNextResult>(IPipeline<UResult, VNextResult> nextPipeline);
    }
}
