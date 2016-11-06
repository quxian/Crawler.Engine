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
