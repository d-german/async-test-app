using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncTests
{
    public class AsyncTests
    {
        private Stopwatch _stopWatch;

        [SetUp]
        public void Init()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        [TearDown]
        public void Cleanup()
        {
            _stopWatch.Stop();
            Console.WriteLine(_stopWatch.ElapsedMilliseconds);
        }

        // Recursively calculates Fibonacci numbers
        private static long Fibonacci(long n)
        {
            if (n == 0 || n == 1) return n;

            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        private static Task<long> FibonacciAsync(long n)
        {
            return Task.FromResult(Fibonacci(n));
        }

        private static string GetString(string value)
        {
            return value.ToUpper();
        }

        private static async Task<string> GetStringAsync(string value, int waitTime = 0)
        {
            await Task.Delay(waitTime);
            Console.WriteLine(value);
            return GetString(value);
        }

        [Test]
        public async Task TestMethod1()
        {
            Assert.AreEqual(832040, await Task.FromResult(Fibonacci(30)));
        }

        [Test]
        public async Task TestMethod11()
        {
            var captured = 5;

            void MultiplyByTwo()
            {
                captured *= 2;
            }

            async Task MultiplyByTwoAsync() //note: not returning anything
            {
                // ReSharper disable once AccessToModifiedClosure
                await Task.FromResult(captured *= 2);
            }

            Task MultiplyByTwo2Async() //note: have to return task
            {
                return Task.FromResult(captured *= 2);
            }

            MultiplyByTwo();
            Assert.AreEqual(10, captured);

            await MultiplyByTwoAsync();
            Assert.AreEqual(20, captured);

            await MultiplyByTwo2Async();
            Assert.AreEqual(40, captured);
        }

        [Test]
        public async Task TestMethod2()
        {
            Assert.AreEqual(await FibonacciAsync(18), 2584);
        }

        [Test]
        public async Task TestMethod21()
        {
            long total = 0;

            for (var i = 10; i < 20; i++)
            {
                total += await FibonacciAsync(i);
            }

            Assert.AreEqual(10857, total);
        }

        [Test]
        public void TestMethod22()
        {
            long total = 0;

            for (var i = 10; i < 20; i++)
            {
                total += Fibonacci(i); //Not this could lockup the current thread (UI)
            }

            Assert.AreEqual(10857, total);
        }

        [Test]
        public void TestMethod3()
        {
            Assert.AreEqual(Fibonacci(20), 6765);
        }

        [Test]
        public void TestMethod4() // broken without await
        {
            var task1 = GetStringAsync("time out.", 1000);
            var task2 = GetStringAsync("Hello", 2000);
            var task3 = GetStringAsync("World", 3000);
            _ = Task.WhenAny(task3, task2, task1);
            Assert.IsFalse(task3.IsCompleted);
            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
        }

        [Test]
        public async Task TestMethod5() //use WhenAny for timeouts
        {
            var task1 = GetStringAsync("Hello", 2000);
            var task2 = GetStringAsync("World", 3000);
            var task3 = GetStringAsync("time out.");
            var res = await Task.WhenAny(task3, task2, task1);
            Assert.AreEqual(res.Result, "TIME OUT.");
            Assert.IsTrue(task3.IsCompleted);
            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
        }

        [Test]
        public async Task TestMethod6() //use WhenAll parallel async
        {
            var task1 = GetStringAsync("Hello", 1000);
            var task2 = GetStringAsync("World", 2000);

            var res = await Task.WhenAll(task2, task1); //ordered by position

            CollectionAssert.AreEqual(new[] {"WORLD", "HELLO"}, res);

            Assert.AreEqual("HELLO WORLD", $"{task1.Result} {task2.Result}");
        }

        [Test]
        public async Task TestMethod7() //serial async
        {
            var s1 = await GetStringAsync("one", 300);
            var s2 = await GetStringAsync(s1 + " two", 1000);
            var s3 = await GetStringAsync(s2 + " three", 200);
            Assert.AreEqual("ONE TWO THREE", s3);
        }

        [Test]
        public async Task TestMethod8()
        {
            Assert.AreEqual("ONE TWO THREE",
                $"{await GetStringAsync("One")} {await GetStringAsync("Two")} {await GetStringAsync("Three")}");
        }

        [Test]
        public async Task TestMethod9() // Canceling tasks
        {
            var numbers = new List<int>();

            async Task DoWork(CancellationToken cancellationToken)
            {
                for (var i = 0; i < 10; i++)
                {
                    numbers.Add(i);
                    await Task.Delay(1000, cancellationToken);
                }
            }

            var cancelSource = new CancellationTokenSource(5000); // This tells it to cancel in 5 seconds

            try
            {
                await DoWork(cancelSource.Token);
            }
            catch (TaskCanceledException e)
            {
                CollectionAssert.AreEqual(new[] {0, 1, 2, 3, 4}, numbers);
            }
        }

        [Test]
        public async Task TestMethod_10()
        {
            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;
            var numIterations = 0;

            var task = Task.Run(() =>
            {
                for (var i = 0; i < 100000 && !token.IsCancellationRequested; i++)
                {
                    numIterations++;

                    if (numIterations >= 10)
                    {
                        cancelSource.Cancel();
                    }
                }
            }, token);

            await task;

            Assert.AreEqual(10, numIterations);
        }
    }
}