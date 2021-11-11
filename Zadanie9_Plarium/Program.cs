﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace Zadanie9_Plarium
{
    class Program
    {
        
        static SemaphoreSlim _sem = new SemaphoreSlim(1);
        static void Main(string[] args)
        {
              
            DataBase dataBase=new DataBase();
            Pogoda wether = new Pogoda();

            AddStartValue addStartValue;
            addStartValue = Cleener;
            addStartValue += AddPeoples;
            addStartValue += AddRegion;
            addStartValue += AddWether;
            #region Обработка нажатия клавиши
            KeyEvent evnt = new KeyEvent();
            evnt.KeyDown += async (sender, e) =>
            {
                switch (e.ch)
                {
                    case '1':
                        {
                            try
                            {
                            
                                addStartValue();
                            }
                            catch
                            {

                            }
                           
                            break;
                        }
                    case '2':
                        {
                            try
                            {    
                                bool Test = false;
                                using (StreamReader sr = new StreamReader("People.txt", System.Text.Encoding.Default))
                                {
                                    string line;
                                  
                                    while ((line = sr.ReadLine()) != null)
                                    {

                                        
                                        if(line=="True") Test=true;
                                    }

                                }
                                if (Test)
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(DataBase));
                                    using (FileStream fs = new FileStream("DataBase.xml", FileMode.OpenOrCreate))
                                    {
                                        dataBase = (DataBase)serializer.Deserialize(fs);
                                    };
                                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("SeriaState.txt"))
                                    {

                                        file.WriteLine("False");

                                        file.Close();
                                    }
                                }
                                else dataBase.GetToColection();
                            }
                            catch
                            {
                                Console.WriteLine($"База данных пуста");
                            }

                            break;
                        }
                    case '3':
                        {
                            wether.Notify += AddToReturnFile;

                            try
                            {
                                new Thread(Get1).Start();
                                new Thread(Get2).Start();
                                new Thread(Get3).Start();
                                new Thread(Get4).Start();
                            }
                            catch
                            {
                                Console.WriteLine($" Еще не установлены значения");
                            }

                           
                            break;
                        }

                    case '7':
                        {
                            try
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(DataBase));
                                using (FileStream fs = new FileStream("DataBase.xml", FileMode.OpenOrCreate))
                                {
                                    serializer.Serialize(fs, dataBase);
                                }
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter("SeriaState.txt"))
                            {

                                file.WriteLine("True");

                                file.Close();
                            }
                            }
                            catch
                            {

                            }
                            
                            
                            break;
                        }
                    case '0':
                        {
 
                            Console.WriteLine($"Программа завершена");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine($"такого значения не предусмотрено");
                            break;
                        }
                }
            };

            Console.WriteLine($"Управляющие команды: \n" +
                $"1-Заполнить все исхордными данными\n" +
                $"2-Восстановить данные из базы данных\n" +
                $"3-Вывести все задачи в файл\n" +
                $"7-Рассериализовать\n" +
                $"0-Выход\n");
            char ch;
            do
            {
                Console.Write("Введите комманду: ");
                ConsoleKeyInfo key;
                key = Console.ReadKey();
                ch = key.KeyChar;
                Console.WriteLine("");
                evnt.OnKeyDown(key.KeyChar);

            }
            while (ch != '0');
            System.Console.ReadKey(true);
            #endregion
            #region Методы для добавления начальных значений
            void AddPeoples()
            { 
                dataBase.AddData(new People("Евреи", "иврит"));
                dataBase.AddData(new People("Англичане", "английский"));
            }
            void AddRegion()
            {
                dataBase.AddData(new Country("Британия", 7000, dataBase.peoples[1]));
                dataBase.AddData( new City("Израиль", 10000, dataBase.peoples[0]));
                dataBase.AddData( new Oblast("Шотландия", 4000, dataBase.peoples[1]));
            }
            void AddWether()
            {
                dataBase.AddData(new Pogoda(dataBase.regions[0], new DateTime(2021, 10, 21, 00, 00, 00), 3, "Дождь"));
                dataBase.AddData(new Pogoda(dataBase.regions[0], new DateTime(2021, 10, 26, 00, 00, 00), -3, "Снег"));
                dataBase.AddData(new Pogoda(dataBase.regions[0], new DateTime(2021, 10, 23, 00, 00, 00), 13, "Солнце"));

                dataBase.AddData(new Pogoda(dataBase.regions[1], new DateTime(2021, 10, 2, 00, 00, 00), -6, "Снег"));
                dataBase.AddData(new Pogoda(dataBase.regions[1], new DateTime(2021, 10, 25, 00, 00, 00), -3, "Снег"));
                dataBase.AddData(new Pogoda(dataBase.regions[1], new DateTime(2021, 10, 30, 00, 00, 00), 13, "Дождь"));

                dataBase.AddData(new Pogoda(dataBase.regions[2], new DateTime(2021, 10, 19, 00, 00, 00), 3, "Дождь"));
                dataBase.AddData(new Pogoda(dataBase.regions[2], new DateTime(2021, 10, 25, 00, 00, 00), 13, "Снег"));
                dataBase.AddData(new Pogoda(dataBase.regions[2], new DateTime(2021, 10, 30, 00, 00, 00), 13, "Солнце"));
            }
            void Get1()
            {
                Console.WriteLine("Get1 хочет зайти");
                _sem.Wait();
                Console.WriteLine("Get1 вошел");
                wether.GetPogoda(dataBase.pogodas, dataBase.regions[0]);//Вывести сведения о погоде в заданном регионе.
                Thread.Sleep(1000);
                Console.WriteLine("Get1 выходит");
                _sem.Release();
            }
            void Get2()
            {
                Console.WriteLine("Get2 хочет зайти");
                _sem.Wait();
                Console.WriteLine("Get2 вошел");
                wether.GetData(dataBase.pogodas, dataBase.regions[1], "Снег", 0);// Вывести даты, когда в заданном регионе шел снег и температура была ниже заданной отрицательной.
                Thread.Sleep(1000);
                Console.WriteLine("Get2 выходит");
                _sem.Release();
            }
            void Get3()
            {
                Console.WriteLine("Get3 хочет зайти");
                _sem.Wait();
                Console.WriteLine("Get3 вошел");
                wether.GetPogoda(dataBase.pogodas, "английский");//Вывести информацию о погоде за прошедшую неделю в регионах, жители которых общаются на заданном языке.
                Thread.Sleep(1000);
                Console.WriteLine("Get3 выходит");
                _sem.Release();
            }
            void Get4()
            {
                Console.WriteLine("Get4 хочет зайти");
                _sem.Wait();
                Console.WriteLine("Get4 вошел");
                wether.GetTemp(dataBase.pogodas, 6000, dataBase.regions);//Вывести среднюю температуру за прошедшую неделю в регионах с площадью больше заданной.
                Thread.Sleep(1000);
                Console.WriteLine("Get4 выходит");
                _sem.Release();
            }
            #endregion
        }

        private static void DisplayMessage(string message) => Console.WriteLine(message);

        delegate void AddStartValue();
        private static void AddToReturnFile(string s)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Return.txt",true))
            {

                file.WriteLine(s, true);

                file.Close();
            }
        }

        private static void Cleener()
        {
            System.IO.File.WriteAllBytes("People.txt", new byte[0]);
            System.IO.File.WriteAllBytes("Region.txt", new byte[0]);
            System.IO.File.WriteAllBytes("Pogoda.txt", new byte[0]);
            System.IO.File.WriteAllBytes("Return.txt", new byte[0]);
        }

    }
}
