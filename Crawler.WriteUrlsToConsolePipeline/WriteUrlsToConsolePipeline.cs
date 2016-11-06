using System;
using System.Collections.Generic;

namespace Crawler {
    public class WriteUrlsToConsolePipeline : IPipeline<List<string>, List<string>> {
        public event Action OnDispose;
        public event Action<List<string>> OnResult;

        public void Dispose() {
            OnDispose?.Invoke();

            SavingState();
        }

        public void Extract(List<string> previousResult) {
            previousResult?.ForEach(url => Console.WriteLine(url));

            OnResult?.Invoke(previousResult);
        }

        public IPipeline<List<string>, List<string>> NextPipeline<VNextResult>(IPipeline<List<string>, VNextResult> nextPipeline) {
            OnResult += nextPipeline.Extract;
            OnDispose += nextPipeline.Dispose;

            return this;
        }

        public void SavingState() {
            //do
        }
    }
}
