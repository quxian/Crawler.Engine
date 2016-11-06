using Crawler;
using Crawler.Model;
using Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public class FindAllUrlsPipeline : IPipeline<DownloadResult, List<string>> {
        public event Action OnDispose;
        public event Action<List<string>> OnResult;

        public void Dispose() {
            OnDispose?.Invoke();

            SavingState();
        }

        public void Extract(DownloadResult previousResult) {
            var urls = previousResult.FindAllUrls();

            OnResult?.Invoke(urls);
        }

        public IPipeline<DownloadResult, List<string>> NextPipeline<VNextResult>(IPipeline<List<string>, VNextResult> nextPipeline) {
            OnResult += nextPipeline.Extract;
            OnDispose += nextPipeline.Dispose;

            return this;
        }

        public void SavingState() {
            //do

        }
    }
}
