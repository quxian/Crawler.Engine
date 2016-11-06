using Crawler;
using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCrawlerEngine {
    class Program {
        static void Main(string[] args) {
            var engine = new Engine(
                new Downloader(),
                new DownloadResultProcesser(),
                new Scheduler())
                .AddUrls(new List<string> { "https://cn.tripadvisor.com/Restaurants-g293915-Thailand.html" })
                .AddPipeline(
                    new FindAllUrlsPipeline()
                    .NextPipeline(new WriteUrlsToConsolePipeline())
                    .NextPipeline(new WriteUrlsToFilePileline("urls.txt"))
                ).Run();

            while ('y' != Console.ReadKey().KeyChar) ;

            engine.Dispose();


            Thread.Sleep(1000);
            Console.WriteLine("end!");
            Console.ReadLine();
        }
    }
}

