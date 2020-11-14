using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncDemo
{
    internal static class Program
    {
        private static void Main()
        {
            var urls = new[]
            {
                "1 www.google.com",
                "2 www.facebook.com",
                "3 www.amazon.com",
                "4 www.stackoverflow.com"
            };
            Download(urls);
            //DownloadRec(urls);
            //DownloadAsync(urls);
            
            
            Task.Delay(500).Wait();
        }

        private static void Download(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                DownloadAsync(url);
            }
        }

        private static async void DownloadAsync(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                await DownloadAsync(url);
            }
        }

        private static void DownloadRec(IReadOnlyList<string> urls)
        {
            var n = urls.Count;

            void TryNextUrl(int i)
            {
                if (i >= n)
                {
                    return;
                }

                DownloadAsync(urls[i], () => { TryNextUrl(i + 1); });
            }

            TryNextUrl(0);
        }

        private static Task DownloadAsync(string url, Action onSuccess = null)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            Task.Delay(rnd.Next(100, 500));

            return Task
                .Run(() =>
                {
                    Console.WriteLine(url);
                    return url.ToUpper();
                })
                .ContinueWith((t)=>
                {
                    Console.WriteLine(t.Result);
                    onSuccess?.Invoke();
                });
        }
    }
}
