using System;
using System.Threading;

namespace Contoso.DigitalGoods.TokenService.Libs
{
    public class WaitProcess
    {
        public static void HoldsOnSeconds(int seconds)
        {
            var destinationTime = DateTime.Now.AddSeconds(seconds);
            Console.Write("Waiting to be transacted.....");
            while (DateTime.Now < destinationTime)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            Console.WriteLine("");
        }
    }
}
