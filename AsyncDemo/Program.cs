using System;
using System.Threading.Tasks;

internal static class Program
{
    private static async Task<bool> ToggleBoolAsync(bool value)
    {
        await Task.Delay(1000);
        return await Task.Run(() => !value);
    }

    public static async Task Main()
    {
        var result = await ToggleBoolAsync(true);
        Console.WriteLine(result);
    }
}
