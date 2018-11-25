using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncTests
{
    [TestClass]
    public class AsyncTests
    {
        public static int Id = 1000;

        // Recursively calculates Fibonacci numbers
        public static long Fibonacci(long n)
        {
            if (n == 0 || n == 1) return n;

            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        public Task<long> FibonacciAsync(long n)
        {
            return Task.Run(() => Fibonacci(n));
        }

        public static string GetString(string value)
        {
            return value.ToUpper();
        }

        public static async Task<string> GetStringAsync(string value, int waitTime = 0)
        {
            await Task.Delay(waitTime);
            Console.WriteLine(value);
            return GetString(value);
        }


        [TestMethod]
        public async Task TestMethod1()
        {
            long answer = 0;
            await Task.Run(() => answer = Fibonacci(20));
            Assert.AreEqual(answer, 6765);
        }

        [TestMethod]
        public async Task TestMethod11()
        {
            var captured = 5;

            void MultiplyByTwo()
            {
                captured = captured * 2;
            }
            
            async Task MultiplyByTwoAsync() //note: not returning anything
            {
                await Task.Run(() => captured = captured * 2);
            }

            Task MultiplyByTwo2Async() //note: have to return task
            {
                return Task.Run(() => captured = captured * 2);
            }

            MultiplyByTwo();
            Assert.AreEqual(10, captured);

            await MultiplyByTwoAsync();
            Assert.AreEqual(20, captured);

            await MultiplyByTwo2Async();
            Assert.AreEqual(40, captured);
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            Assert.AreEqual(await FibonacciAsync(18), 2584);
        }

        [TestMethod]
        public async Task TestMethod21()
        {
            long total = 0;

            for (var i = 10; i < 20; i++)
            {
                total += await FibonacciAsync(i);
            }

            Assert.AreEqual(10857, total);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.AreEqual(Fibonacci(20), 6765);
        }

        [TestMethod]
        public void TestMethod4() // broken without await
        {
            var task1 = GetStringAsync("time out.", 1000);
            var task2 = GetStringAsync("Hello", 2000);
            var task3 = GetStringAsync("World", 3000);
            var res = Task.WhenAny(task3, task2, task1);
            Assert.IsFalse(task3.IsCompleted);
            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
        }

        [TestMethod]
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

        [TestMethod]
        public async Task TestMethod6() //use WhenAll parallel async
        {
            var task1 = GetStringAsync("Hello", 1000);
            var task2 = GetStringAsync("World", 2000);

            var res = await Task.WhenAll(task2, task1); //ordered by position

            CollectionAssert.AreEqual(new[] {"WORLD", "HELLO"}, res);

            Assert.AreEqual("HELLO WORLD", $"{task1.Result} {task2.Result}");
        }

        [TestMethod]
        public async Task TestMethod7() //serial async
        {
            var s1 = await GetStringAsync("one", 300);
            var s2 = await GetStringAsync(s1 + " two", 1000);
            var s3 = await GetStringAsync(s2 + " three", 200);
            Assert.AreEqual("ONE TWO THREE", s3);
        }

        [TestMethod]
        public async Task TestMethod8()
        {
            Assert.AreEqual("ONE TWO THREE",
                $"{await GetStringAsync("One")} {await GetStringAsync("Two")} {await GetStringAsync("Three")}");
        }

        [TestMethod]
        public async Task TestMethod9()
        {
            var cancelSource = new CancellationTokenSource(5000); // This tells it to cancel in 5 seconds
        }
    }
}