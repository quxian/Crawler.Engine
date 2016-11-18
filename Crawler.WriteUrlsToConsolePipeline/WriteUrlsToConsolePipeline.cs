using System;
using System.Collections.Generic;

namespace Crawler {
    public class WriteUrlsToConsolePipeline : AbstractPipeline<List<string>, List<string>> {
        private long _urlCount = 0;
        public override void Extract(List<string> previousResult) {
            previousResult?.ForEach(url => Console.WriteLine($"{++_urlCount}:{url}"));

            _onResult(previousResult);
        }
    }
}
