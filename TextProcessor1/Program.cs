using System;
using System.IO;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessor1
{
    class Program
    {
        static void Main(string[] args)
        {
            int length = args.Length;
            if (args.Length > 1)
            {
                string command;
                command = args[0] + args[1];
                command = command.ToLower();
                if (command == "созданиесловаря")
                {
                    CreateDictionary();
                }
                else if (command == "обновлениесловаря")
                {

                }
                else if (command == "очиститьсловарь")
                {

                }
                else
                {
                    Console.WriteLine("Введена неверная команда, попробуйте еще раз");
                    Console.ReadLine();
                }
            }
            else if(args.Length==0)
            {
                WordProcessing();
                //Console.WriteLine("Введена пустая команда");
                //Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Введена неверная команда, попробуйте еще раз");
                Console.ReadLine();
            }
        }
        static void CreateDictionary()
        {
            using (FrequencyContext db = new FrequencyContext())
            {
                string path = @"C:\Users\mindw\source\repos\TextProcessor1\TextProcessor1\Text.txt";
                string text;
                int count = 1;
                string[] separators = { ",", ".", "!", "?", ";", ":", " " };
                try
                {
                    Console.WriteLine("******считываем весь файл********");
                    using (StreamReader sr = new StreamReader(path))
                    {
                        text = sr.ReadToEnd();
                    }
                    string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    Array.Sort(words);
                    //foreach (var word in words)
                    //{
                    //    Console.WriteLine(word);
                    //}
                    
                    for (int i = 0; i < words.Length-1; i++)
                    {
                        if (words[i]==words[i+1])
                        {
                            count++;
                        }
                        else
                        {
                            if (count>=3 && words[i].Length>=3) db.Frequencies.Add(new Frequency { Word = words[i], Amount = count });
                            count = 1;
                        }
                    }
                    db.SaveChanges();
                    Console.WriteLine(words.Length);

                    //Console.WriteLine(text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //db.Frequencies.Add(new Frequency { Word = "Овощи", Amount = 37 });
                //db.SaveChanges();
                //Frequency frequency1 = new Frequency { Word = "Арбуз", Amount = 7 };
                //Frequency frequency2 = new Frequency { Word = "Автобус", Amount = 2 };
                //db.Frequencies.Add(frequency1);
                //db.Frequencies.Add(frequency2);
                //db.SaveChanges();
                //Console.WriteLine("Объекты успешно сохранены");

                //var frequencies = db.Frequencies;
                //Console.WriteLine("Список объектов:");
                //foreach (Frequency f in frequencies)
                //{
                //    Console.WriteLine("{0} - {1}", f.Word, f.Amount);
                //}
                //Console.ReadLine();
            }
        }
        static void UpdateDictionary()
        {

        }
        static void ClearDictionary()
        {
            using (FrequencyContext db = new FrequencyContext())
            {
                db.Database.SqlQuery<Frequency>("TRUNCATE TABLE [Frequencies]");
            }
        }
        static void WordProcessing()
        {
            string inpWord,sqlQuery1;
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            while (true)
            {
                //cki = Console.ReadKey(true);
                inpWord = Console.ReadLine();
                
                if (inpWord == "") break;
                
                System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("@word", inpWord+"%");
                sqlQuery1 = "select top 5 * from Frequencies where Word like @word order by Amount desc, Word desc";
                using (FrequencyContext db = new FrequencyContext())
                {
                    var frequency1 = db.Database.SqlQuery<Frequency>(sqlQuery1,param);
                    foreach (var item in frequency1)
                    {
                        Console.WriteLine(item.Word);
                    }
                }
            }
        }
    }
}
