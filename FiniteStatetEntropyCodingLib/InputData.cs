using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FiniteStatetEntropyCodingLib
{
    public class InputData
    {

        public static (bool, string) CheckInput1(string s)
        {
            var ss = s.Split('\n');
            if (ss.Length == 3)
            {
                if (Regex.IsMatch(ss[0], @"\b[1-9]{1,}[0-9]{0,}\b") && Regex.Matches(ss[1], @"([A-Z]:[1-9]{1,}[0-9]{0,}){1,}").Count == int.Parse(ss[0]) && ss[2].Length > 0)
                {
                    if (!CheckSum(Regex.Matches(ss[1], @":([1-9]{1,}[0-9]{0,})\b").Select(m => m.Groups[1].Value).ToArray())) {
                        return (false, "Сумма частот не степень двойки.");
                    }
                    if (!CheckChars(ss[2], ss[1]))
                    {
                        return (false, "В тексте содержатся символы не из указанного алфавита.");
                    }
                    return (true, "");
                }
                return (false, "Неверный формат данных.");
            }
            return (false, "Неверный формат данных.");
        }

        public static (bool, string) CheckInput2(string s)
        {
            var ss = s.Split('\n');
            if (ss.Length == 4)
            {
                if (Regex.IsMatch(ss[0], @"\b[1-9]{1,}[0-9]{0,}\b") && Regex.IsMatch(ss[1], @"([A-Z]:[1-9]{1,}[0-9]{0,}){1,}") &&
                    Regex.IsMatch(ss[2], @"\b[0-1]{1,}\b") && Regex.IsMatch(ss[3], @"\b[1-9]{1,}[0-9]{0,}\b"))
                {
                    if (!CheckSum(Regex.Matches(ss[1], @":([1-9]{1,}[0-9]{0,})\b").Select(m => m.Groups[1].Value).ToArray()))
                    {
                        return (false, "Сумма частот не степень двойки.");
                    }
                    return (true, "");
                }
                return (false, "Неверный формат данных.");
            }
            return (false, "Неверный формат данных.");
        }

        static bool CheckSum(string[] arr)
        {
            Console.WriteLine($"sum = {arr.Select(x => int.TryParse(x, out int y) ? y : 0).Sum()}");
            return CodingChars.IsPowOfTwo(arr.Select(x => int.TryParse(x, out int y) ? y : 0).Sum());
        }

        static bool CheckChars(string text, string ss)
        {
            var chars = Regex.Matches(ss, @"[A-Z]").Select(m => m.Value).ToArray();
            var real = text.Trim().Distinct();
            foreach(var r in real)
            {
                if (!chars.Contains(r + ""))
                {
                    return false;
                }
            }
            return true;
        }

        public static (int, string) CodingProg(string s)
        {
            (char[] charArr, int[] prob, string text) = InputData.ParseData(s);
            
            var codingObj = new CodingChars(0, charArr, prob);
            codingObj.condArr[0] = codingObj.GetFirstCond(text[0]);


            var coded = codingObj.CodeStringCycle(codingObj.condArr[0], text);


            return (codingObj.condArr[0], coded);
           
        }

        public static string DecodingProg(string s)
        {

            (char[] charArr, int[] prob, string text) = InputData.ParseData(s);
            int q = int.Parse(s.Split('\n')[3]);
            var decodingObj = new CodingChars(q, charArr, prob);
            var decoded = decodingObj.DecodeStringCycle(q, text);
            
            Console.WriteLine();
            Console.WriteLine($"---> Расжатая строка : {decoded}");

            Console.WriteLine();

            return decoded;

        }

        static void PrintEncodingInfo(CodingChars codingObj)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~ Таблица состояний ~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
            codingObj.PrintCondTable();
            Console.WriteLine();

            Console.WriteLine("~~~~~~~~~~~~~~~~~ Таблица кодировок ~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
            codingObj.PrintCodingDict();
        }


        static (char[], int[], string s) ParseData(string s)
        {
            var ss = s.Split("\n");
            var charArr = new char[int.Parse(ss[0])];
            var prob = new int[charArr.Length];
            var groups = ss[1].Split(' ');
           for(var i = 0; i < charArr.Length; i++)
            {
                charArr[i] = groups[i][0];
                prob[i] = int.Parse(groups[i].Split(':')[1]);
            }
            return (charArr, prob, ss[2]);
        }

    }
}
