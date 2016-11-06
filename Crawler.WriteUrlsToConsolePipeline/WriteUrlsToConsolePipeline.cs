using Crawler;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public class WriteUrlsToConsolePipeline : IPipeline<DownloadResult, DownloadResult> {
        public event Action OnDispose;
        public event Action<DownloadResult> OnResult;

        public void Dispose() {
            OnDispose?.Invoke();

            SavingState();
        }

        public void Extract(DownloadResult previousResult) {
            Console.WriteLine(previousResult.CurrentUrl);

            OnResult?.Invoke(previousResult);
        }

        public IPipeline<DownloadResult, DownloadResult> NextPipeline<VNextResult>(IPipeline<DownloadResult, VNextResult> nextPipeline) {
            OnResult += nextPipeline.Extract;
            OnDispose += nextPipeline.Dispose;

            return this;
        }

        public void SavingState() {
            //do
        }
    }
}
