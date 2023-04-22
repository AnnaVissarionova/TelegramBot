using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopedMachineLib
{
    public class Machine
    {
        

        Dictionary<int[], int> comb;
        Dictionary<int, Dictionary<char, int[]>> q_table;
        int length;
        int steps_count;
        public Machine(int conds, string[] transf, int l)
        {
            q_table = DefineConditionsDictionary(CreateArr(conds), transf);
            comb = CreateCombDict(q_table);
            length = l;
            steps_count = 0;
        }


        //создает словарь со строками-описаниями машины для каждого возможного состояния 
        static Dictionary<int, Dictionary<char, int[]>> DefineConditionsDictionary(int[] conds, string[] transforms)
        {
            var dict = new Dictionary<int, Dictionary<char, int[]>>();
            for (var i = 0; i < conds.Length; i++)
            {
                string[] cur_tf = transforms.Where(x => x.StartsWith($"q{i + 1}")).ToArray();
                var inner_dict = new Dictionary<char, int[]>();
                foreach (var c in cur_tf)
                {
                    var ss = c.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    var t = "";
                    foreach (var h in ss[3..])
                    {
                        t = t + " " + h;
                    }
                    inner_dict.Add(ss[1][0], ParseTransormation(t));
                }
                dict.Add(conds[i], inner_dict);

            }
            return dict;
        }


        //создает словарь со всеми строками-описаниями работы машины и количеством их повторений
        static Dictionary<int[], int> CreateCombDict(Dictionary<int, Dictionary<char, int[]>> q_table)
        {
            var dict = new Dictionary<int[], int>();
            foreach (var i in q_table)
            {
                foreach (var d in i.Value)
                {
                    var tf = GetTransformInArr(q_table, i.Key, d.Key);
                    dict.Add(tf, 0);
                }
            }
            return dict;
        }


        //проверяет повтор комбинации
        bool CheckCobminations(Dictionary<int[], int> comb)
        {
            foreach (var i in comb)
            {
                if (i.Value == 0)
                {
                    return false;
                }
            }
            return true;
        }

        //возвращает строку-описание в виде массива [входное_состояние, символ, выходное_состояние, новый_символ, смещение]
        static int[] GetTransformInArr(Dictionary<int, Dictionary<char, int[]>> q_table, int q, char c)
        {
            var res = new int[5];
            res[0] = q;
            var dict = q_table.GetValueOrDefault(q);
            res[1] = c;
            res[2] = dict.GetValueOrDefault(c)[0];
            res[3] = dict.GetValueOrDefault(c)[1];
            res[4] = dict.GetValueOrDefault(c)[2];
            return res;
        }



        public (int, int, string) OneStep(int q, string text, int cur_pos)
        {
            try
            {
                text = CheckText(text);
                (int next_cond, int next_pos) = (-1, -1);
                var cur_char = text[cur_pos];
                var dict = q_table.GetValueOrDefault(q);
                if (dict.GetValueOrDefault(cur_char) == null)
                {
                    return (-1, -1, text); //сигнал об окончании работы
                }
                text = text[..cur_pos] + (char)dict.GetValueOrDefault(cur_char)[1] + text[(cur_pos + 1)..];
                next_cond = dict.GetValueOrDefault(cur_char)[0];
                next_pos = cur_pos + dict.GetValueOrDefault(cur_char)[2];


                if (next_pos == -1) { next_pos = length - 1; }
                if (next_pos == length) { next_pos = 0; }

                steps_count++;
                return (next_cond, next_pos, text);
            }
            catch (Exception ex)
            {
                return (-404, -404, text);
            }
            
        }

        //проверяет на наличие комбинации, которая зацикливает машину на одном месте 
        //пример: q1 a -> q1 a 0
        (bool, string) CheckForCycleInOnePosition()
        {
            foreach (var q in q_table)
            {
                foreach (var i in q.Value)
                {
                    // Console.WriteLine($"{q.Key} == {i.Value[0]} && {i.Key} == {i.Value[1]} && {i.Value[2]} == 0");
                    if (q.Key == i.Value[0] && i.Key == (char)i.Value[1] && i.Value[2] == 0)
                    {
                        return (true, "q" + q.Key + " " + i.Key + " -> q" + i.Value[0] + " " + (char)i.Value[1] + " " + i.Value[2]);
                    }
                }
            }
            return (false, "");
        }

        public int RunMachine(int q, string text, int cur_pos)
        {
            try
            {
                text = CheckText(text);
                if (CheckForCycleInOnePosition().Item1)
                {
                    Console.WriteLine($"------> Машина содержит зацикливающую комбинацию {CheckForCycleInOnePosition().Item2}");
                    return -1;
                }
                else
                {
                    while (q != -1)
                    {
                        if (CheckCobminations(comb) && steps_count > length)
                        {
                            Console.WriteLine("------> Машина прошла все комбинации и зациклилась");
                            return -1;
                        }

                        (int q_prev, int pos_prev, char prev) = (q, cur_pos, text[cur_pos]);
                        (q, cur_pos, text) = OneStep(q, text, cur_pos);
                        if (q >= 0)
                        {
                            UpdateComb(q_prev, prev);
                        }

                    }
                    if (q == -1)
                    {
                        Console.WriteLine("------> Машина не зациклилась");
                        return 1;

                    }
                    else if (q == -404)
                    {
                        Console.WriteLine("ArgumentNullException");
                        return -404;
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ArgumentNullException");
                return -404;
            }
     
        }

        //обновляет количество выполненных комбинаций
        void UpdateComb(int q, char c)
        {
            
            var arr = new int[5];
            arr[0] = q;
            arr[1] = c;
            var dict = q_table.GetValueOrDefault(q);
            arr[2] = dict.GetValueOrDefault(c)[0];
            arr[3] = dict.GetValueOrDefault(c)[1];
            arr[4] = dict.GetValueOrDefault(c)[2];

            var kvarr = comb.Keys;
            for (var i = 0; i < kvarr.Count; i++)
            {
                if (CheckArrs(kvarr.ElementAt(i), arr))
                {
                    var s = comb.GetValueOrDefault(kvarr.ElementAt(i));
                    comb[kvarr.ElementAt(i)] = s + 1;
                }
            }

        }




        string CheckText(string text)
        {
            text = text.Trim();
            while (text.Length < length)
            {
                text = text + "^";
            }
            return text;
        }

        static int[] CreateArr(int n)
        {
            var res = new int[n];
            for (var i = 0; i < n; i++)
            {
                res[i] = i + 1;
            }
            return res;
        }


        static bool CheckArrs(int[] a, int[] b)
        {
            if (a.Length == b.Length)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        static int[] ParseTransormation(string tf)
        {
            var res = new int[3];
            var ss = tf.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
            res[0] = int.Parse(ss[0].Substring(1));
            res[1] = ss[1][0];
            res[2] = ss[2].Contains('+') ? 1 : int.Parse(ss[2]);
            return res;
        }

    }


}

