# Crawler.Engine
爬虫框架

主要有4个模块组成：

# Crawler.IDownloader
下载模块，结果传递给Crawler.IDownloadResultProcesser模块。

# Crawler.IDownloadResultProcesser
结果处理模块。
1.找到所有的Url，传递到Crawler.IScheduler模块。
2.把结果传递到Crawler.IPipeline模块。

# Crawler.IScheduler
Urls管理模块。

# Crawler.IPipeline
结果处理管道。实现了树形管道。

# 使用

using (new Engine(
    new Downloader(),
    new DownloadResultProcesser(),
    new Scheduler())
    .AddUrls(new List<string> { "https://www.baidu.com/" })
    .AddPipeline(
        new FindAllUrlsPipeline()
        .NextPipeline(new WriteUrlsToConsolePipeline())
        .NextPipeline(new WriteUrlsToFilePileline("urls.txt"))
    ).Run()) {

    while ('y' != Console.ReadKey().KeyChar) ;
}

当然，你需要写自己的管道逻辑来处理你的业务，你只需要继承AbstractPipeline即可，适当扩展，就可以。
