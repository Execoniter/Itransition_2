using System;
using Org.BouncyCastle.Security;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace rps
{
    static class Program
    {
        public static string key = RandomString(32);
        public static int pcMove = 0;
        public static int userMove = 0;
        public static int totalLength = 0;
        public static string[] totalMoves;

        static void Main(string[] args)
        {
            if (args.Length - 1 < 3)
            {
                Console.WriteLine("Error: args must be >= 3");
                Environment.Exit(1);
            }
            else if (args.Length % 2 == 1)
            {
                Console.WriteLine("Error: args must be odd");
                Environment.Exit(1);
            }

            totalMoves = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                totalMoves[i - 1] = args[i];
            }

            PrintDublicates_HashSet(totalMoves);

            Console.WriteLine("Available move:");            
            for (int i = 1; i < args.Length; i++)
            {
                Console.WriteLine($"{i} - {totalMoves[i - 1]}");
            }
            Console.WriteLine("0 - quit");
            totalLength = args.Length;
            Run();
        }

        public static void PrintDublicates_HashSet(string[] n)
        {
            var set = new HashSet<string>();
            foreach (var item in n)
                if (!set.Add(item))
                {
                    Console.WriteLine("No duplicates pls!");
                    Environment.Exit(1);
                }
        }

        static byte[] hmacSHA256(String data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key)))
            {
                return hmac.ComputeHash(Encoding.ASCII.GetBytes(data));
            }
        }

        public static string RandomString(int length)
        {
            char[] allowableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZqwertyuiopasdfghjklzxcvbnm1234567890".ToCharArray();
            SecureRandom random = new SecureRandom();
            string str = "";
            for (var i = 0; i < length; i++)
            {
                str += allowableChars[random.Next(0, 60)].ToString();
            }
            return new string(str);
        }

        static void Run()
        {
            Random rnd = new Random();
            pcMove = rnd.Next(1, totalLength);
            Console.WriteLine("HMAC:" + BitConverter.ToString(hmacSHA256(Convert.ToString(pcMove))).Replace("-", ""));
            Console.Write("Enter your move(number): ");
            userMove = Convert.ToInt32(Console.ReadLine());
            if (userMove == 0) Environment.Exit(0);
            Console.WriteLine("Your Move: " + totalMoves[userMove - 1]);
            Console.WriteLine("Computer Move: " + totalMoves[pcMove - 1]);
            Console.WriteLine(GameResult(userMove, pcMove));
            Console.WriteLine();
            Console.WriteLine("HMAC key: " + key);
        }

        static string GameResult(int uMove, int pMove) 
        {
            int center = (totalMoves.Length - 1) / 2;
            List<string> tMovesList = totalMoves.ToList();
            string[] totaMovesNew = totalMoves;
            int KK = totalMoves.Length-(center+1);
            string userMove = tMovesList[uMove - 1];

            while (totaMovesNew[center] != userMove)
            {
                totaMovesNew = totaMovesNew.Skip(totaMovesNew.Length - KK).Take(KK).Concat(totaMovesNew.Take(totaMovesNew.Length - KK)).ToArray();
            }

            string pcMoveTemp = totalMoves[pcMove-1];
            int pcMoveIndexTemp = 0;

            for (int i = 0; i < totaMovesNew.Length; i++)
            {
                if (totaMovesNew[i] == pcMoveTemp)
                {
                    pcMoveIndexTemp = i;
                }
            }

            if (pcMoveIndexTemp == center)
            {
                return "Draw";
            }
            else if (pcMoveIndexTemp < center)
            {
                return "Win";
            }
            else
            {
                return "Lose";
            }
        }
    }
}

