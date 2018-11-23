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

        public static string LongOperation(string value, int waitTime = 1)
        {
            Thread.Sleep(waitTime * 1000);
            return value.ToUpper();
        }


        [TestMethod]
        public async Task TestMethod1()
        {
            long answer = 0;
            await Task.Run(() => answer = Fibonacci(20));
            Assert.AreEqual(answer, 6765);
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            Assert.AreEqual(await FibonacciAsync(18), 2584);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.AreEqual(Fibonacci(20), 6765);
        }

    }
}