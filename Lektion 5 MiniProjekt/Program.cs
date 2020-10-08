﻿using System;
using System.IO;

namespace MiniProjekt
{
    class Program
    {
        static void Main(string[] args)
        {

            Menu menu = new Menu("FancyMenu");
            menu.Add(new MenuItem("Punkt1", "HAHAHA"));
            menu.Add(new MenuItem("Punkt2", "HEHEHEHEHE"));
            Menu underMenu = new Menu("undermenu",
                new MenuItem("testpunkt", "KEKEKEKEKEK"),
                new MenuItem("testpunkt2", "TIHIFNIS")
            );
            menu.Add(underMenu);
            menu.Add(new InfiniteMenu("Uendelig menu"));
            menu.Add(new FileSystemMenu("Browse desktop", new DirectoryInfo(@"C:\Users\Morten Jørgensen\Desktop\"))); //Hard codet :(

            menu.Start();

        }
    }
}
