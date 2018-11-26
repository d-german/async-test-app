using System;
using System.Threading.Tasks;

namespace AsyncTestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DownloadRec(new[] {"www.google.com", "www.facebook.com"});
            Task.Delay(500).Wait();
        }

        private static void DownloadRec(string[] urls)
        {
            var n = urls.Length;

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

        private static Task DownloadAsync(string url, Action onSuccess)
        {
            return Task
                .Run(() =>
                {
                    Console.WriteLine(url);
                    return url.ToUpper();
                })
                .ContinueWith((t)=>
                {
                    Console.WriteLine(t.Result);
                    onSuccess();
                });
        }
    }
}