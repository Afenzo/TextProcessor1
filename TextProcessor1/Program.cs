using System;
using System.IO;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TextProcessor1
{
    class Program
    {
        static string path = "";

        static void Main(string[] args)
        {        
            if (args.Length > 1)
            {
                string command;
                command = args[0] + args[1];
                command = command.ToLower();
                path = args[2];
                if (command == "созданиесловаря")
                {
                    CreateDictionary();
                }
                else if (command == "обновлениесловаря")
                {
                    UpdateDictionary();
                    Console.WriteLine("Обновление завершено");
                }
                else if (command == "очиститьсловарь")
                {
                    ClearDictionary();
                    Console.WriteLine("Очистка завершена");
                }
                else
                {
                    Console.WriteLine("При запуске задан неверный параметр");
                }
            }
            else
            {
                Console.WriteLine("При запуске задан неверный параметр");
            }
            WordProcessing();
        }
        static void CreateDictionary()
        {
            using (FrequencyContext db = new FrequencyContext())
            {
                string sqlQuery1 = "select * from Frequencies";
                var countFreq = db.Database.SqlQuery<Frequency>(sqlQuery1);
                if (countFreq.Count() == 0)
                {
                    int count = 1;
                    try
                    {
                        string[] words = ReadAndSort(path);  //читаем файл, разбиваем на слова и сортируем их
                        for (int i = 0; i < words.Length - 1; i++)
                        {
                            if (words[i] == words[i + 1]) //считаем количество повторяющихся слов
                            {
                                count++;
                            }
                            else //если повторения закончились - добавляем информацию и сбрасываем счетчик
                            {
                                if (count >= 3 && words[i].Length >= 3)
                                    db.Frequencies.Add(new Frequency { Word = words[i], Amount = count });

                                count = 1;
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Console.WriteLine("Создание завершено");
                }
                else
                {
                    Console.WriteLine("В словаре уже есть записи. Вы можете очистить словарь, либо обновить его.");
                }
            }
        }
        static void UpdateDictionary()
        {
            using (FrequencyContext db = new FrequencyContext())
            {
               
                string[] newWords = ReadAndSort(path);
                int j = 0, count=1, laterId=0;
                var oldFreq = db.Frequencies.OrderBy(f => f.Word);
                int countOldFreq=oldFreq.Count();
                bool found;

                    for (int i = 0; i < newWords.Length-1; i++)
                    {
                        if (newWords[i] == newWords[i + 1])
                        {
                            count++; //считаем повторения
                        }
                        else
                        {
                            found = false;
                            for (j = laterId; j < countOldFreq; j++)
                            {
                                if (oldFreq.AsEnumerable().ElementAt(j).Word == newWords[i]) //если слово из нового текста нашлось в бд
                                {
                                    oldFreq.AsEnumerable().ElementAt(j).Amount += count;
                                    //оба набора данных отсортированы, поэтому при новой итерации сравнения
                                    //можно не смотреть на пройденные элементы
                                    //сохраняем номер последнего совпавшего элемента
                                    //и следующие итерации начинаем с него, пока не обнаружим новое совпадение
                                    laterId = j;
                                    found = true;
                                    break;
                                }
                            }
                            if ((found == false) 
                                && (count>=3) 
                                && (newWords[i].Length>=3))
                            {   //если не нашли совпадения с бд - добавляем новый элемент
                                db.Frequencies.Add(new Frequency { Word = newWords[i], Amount = count });
                            }
                            count = 1;
                        }
                        db.SaveChanges();

            }
            }
        }
        static void ClearDictionary()
        {
            using (FrequencyContext db = new FrequencyContext())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM Frequencies");
            }

        }
        static void WordProcessing()
        {
            string inpWord,sqlQuery1;
            while (true)
            {
                inpWord = "";
                
                //цикл обработки нажатых клавиш
                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter) //переводим строку, выходим из цикла
                    {
                        Console.WriteLine();
                        break;
                    }

                    if (key.Key == ConsoleKey.Escape) return; //завершаем метод

                    if (key.Key == ConsoleKey.Backspace) //затираем последний символ по backspace
                    {
                        inpWord = inpWord.Substring(0, inpWord.Length - 1);
                        Console.Write("\r" + inpWord + " \b");
                    }
                    else
                    {
                        Console.Write(key.KeyChar); //выводим каждую нажатую клавишу в строку
                        inpWord += key.KeyChar; //накапливаем символы в отдельной переменной
                    }
                }

                if (inpWord == "") return;

                System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("@word", inpWord+"%");
                sqlQuery1 = "select top 5 * from Frequencies where Word like @word order by Amount desc, Word desc";
                using (FrequencyContext db = new FrequencyContext())
                {
                    var frequency1 = db.Database.SqlQuery<Frequency>(sqlQuery1,param);
                    foreach (var item in frequency1) //выводим полученные слова на экран
                    {
                        Console.WriteLine(item.Word);
                    }
                }
            }
        }
        static string[] ReadAndSort(string path)
        {

            string text="";
            string[] separators = { ",", ".", "!", "?", ";", ":", " ","(",")","\"" };
            Console.WriteLine("Файл считывается");
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            text = text.ToLower();
            string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries); //разбиваем на слова
            Array.Sort(words); //сортируем
            return words;
        }
    }
}
