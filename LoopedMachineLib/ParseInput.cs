using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LoopedMachineLib
{
    public class ParseInput
    {

        public static (int, string[], string, int) Parse(string s)
        {
            var q = 0;
            var m = 0;
            var tf = new string[0];
            var text = "";
            var length = 0;


            var ss = s.Split("\n");

            q = int.Parse(ss[0]);
            m = int.Parse(ss[1]);
            tf = ss[2..(2 + m)];
            text = ss[(2 + m)];
            length = int.Parse(ss[^1]);

            return (q, tf, text, length);
        }

        public static (bool, string) CheckInput(string s)
        {
            var q = 0;
            var m = 0;
            var tf = new string[0];
            var text = "";
            var length = 0;

            var ss = s.Split("\n");
            if (ss.Length >=5)
            {
                Console.WriteLine($"ss.length == {ss.Length}");
                if( int.TryParse(ss[0], out q) )
                {
                    Console.WriteLine($"q == {q}");

                    if (int.TryParse(ss[1], out m) && q >= 1)
                    {
                        Console.WriteLine($"m == {m}");

                        if(CheckTF(s, m) && m >= 1)
                        {
                            tf = GetTF(s);
                            if (CheckText(ss[(2+m)]))
                            {
                                text = GetText(ss[(2 + m)]);
                                if(int.TryParse(ss[^1], out length))
                                {
                                    return (text.Length <= length, text.Length <= length ? "" : "Длина данных на ленте не может быть больше длины ленты.");
                                }
                                return (false, "Неверный формат данных о длине ленты.");
                            }
                            return (false, "Неверный формат текста.");
                        }
                        return (false, m < 1 ? "Количестве строк состояний не может быть меньше 1." :"Неверный формат строк с описанием машины.");

                    }
                    return (false, q < 1 ? "Количество состояний не может быть меньше 1." : "Неверный формат данных о количестве строк состояний.");
                }
                return (false, "Неверный формат данных о количестве состояний.");
            }

            return (false, "Неверный формат данных.");
        }

        public static bool CheckTF(string s, int m)
        {
           var ss = s.Split('\n');
           var tf = Regex.Matches(s, @"q[1-9]{1,}[0-9]{0,}\s{1,}(.)\s{1,}->\s{1,}q[1-9]{1,}[0-9]{0,}\s{1,}(.)\s{1,}(((\+|-)1)|0)");
            return tf.Count == m;
        }

        public static string[] GetTF(string s)
        {
            var tf = new string[0];
            var ss = s.Split('\n');
            var t = Regex.Matches(s, @"q[1-9]{1,}[0-9]{0,}\s{1,}(.)\s{1,}->\s{1,}q[1-9]{1,}[0-9]{0,}\s{1,}(.)\s{1,}(((\+|-)1)|0)");
            foreach(Match i in t)
            {
                tf = tf.Append(i.Value).ToArray();
            }
            return tf;
        }

        public static bool CheckText(string s)
        {
            var ss = Regex.Matches(s, @"\b(.){1,}\b");
            return ss.Count == 1;
        }

        public static string GetText(string s)
        {
            return Regex.Match(s, @"\b(.){1,}\b").Value;
        }

    }
}
