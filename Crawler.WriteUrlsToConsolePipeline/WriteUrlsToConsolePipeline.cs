using System;
using System.Collections.Generic;

namespace Crawler {
    public class WriteUrlsToConsolePipeline : AbstractPipeline<List<string>, List<string>> {
        public override void Extract(List<string> previousResult) {
            previousResult?.ForEach(url => Console.WriteLine(url));

            _onResult(previousResult);
        }
    }
}
