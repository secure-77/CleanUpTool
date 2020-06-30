using System;
using System.Collections.Generic;
using NDesk.Options;


namespace CleanUpTool
{
    class Program
    {
       
        static void Main(string[] args)
        {

            String path = "";
            String opmode = "";
            Boolean debug = false;
            bool show_help = false;
            int? days = null;
            String filter = "*";

            OptionSet p = new OptionSet
            {
                { "m|mode=", "operation mode ({MODE}= file or folder)", v => opmode = (v) },
                { "f|filter=", "Wildcard {FILTER} (e.g. '*.pdf' for all pdf files or '*' for all folders)", v => filter = (v) },
                { "d|days=", "days", (int v) => days = v },
                { "debug", "debug mode", v => debug = v != null },
                { "h|help", "show this help", v => show_help = v != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("CleanUpTool: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'CleanUpTool.exe --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
            }


            // Check for params
            if (extra.Count > 0)
            {
                path = string.Join(" ", extra.ToArray());
                if (!(System.IO.Directory.Exists(path)))
                {
                    ShowError("the path " + path + " does not exist", p);
                }
            }
            else
            {
                ShowError("the path is missing", p);
            }

            if (!(opmode.Equals("file") || opmode.Equals("folder")))
            {
                ShowError("the mode is invalid or missing", p);
            }

            if (!(days.HasValue))
            {
                ShowError("days paramenter is missing", p);
            }



            // actions
            try
            {
                //getting Date
                DateTime targetDate = DateTime.Now;
                targetDate = targetDate.AddDays(-Convert.ToDouble(days));

                //show Infos
                ShowInfo("Deleting all " + opmode + "s older then " + targetDate.ToString("dd.MM.yyyy HH:mm") + " from " + path);
                ShowInfo("using filter: " + filter);

                // file deletion
                if (opmode.Equals("file"))
                {
                    DeleteFiles(path, filter, debug, targetDate);
                }

                // folder deletion
                if (opmode.Equals("folder"))
                {
                    DeleteFolders(path, filter, debug, targetDate);
                }

                ShowInfo("done");

            }
            catch (Exception e)
            {
                ShowError(e.ToString(), p);
            }

        }



        static void DeleteFiles(String path, String filter, Boolean debug, DateTime targetDate)
        {
            // get all files from folder with filter
            String[] filesInFolder = System.IO.Directory.GetFiles(path, filter, System.IO.SearchOption.TopDirectoryOnly);
            System.Collections.ObjectModel.Collection<String> filesToDelete = new System.Collections.ObjectModel.Collection<String>();

            foreach (String filePath in filesInFolder)
            {
                // check date of files and add them to collection
                if (System.IO.File.GetCreationTime(filePath) < targetDate)
                {
                    filesToDelete.Add(filePath);
                }
            }

            ShowInfo(filesToDelete.Count.ToString() + " files will be deleted");

            if (debug) { Debug(); }

            // delete files
            foreach (String filePath in filesToDelete)
            {
                System.IO.File.Delete(filePath);            
            }
        }

        static void DeleteFolders(String path, String filter, Boolean debug, DateTime targetDate)
        {

            // get all folders from folder
            String[] directoriesInFolder = System.IO.Directory.GetDirectories(path, filter, System.IO.SearchOption.TopDirectoryOnly);
            System.Collections.ObjectModel.Collection<String> directoriesToDelete = new System.Collections.ObjectModel.Collection<String>();


            foreach (String diretoryPath in directoriesInFolder)
            {

                // check date of files and add them to collection
                if (System.IO.Directory.GetCreationTime(diretoryPath) < targetDate)
                {
                    directoriesToDelete.Add(diretoryPath);
                }

            }


            ShowInfo(directoriesToDelete.Count.ToString() + " directories will be deleted");

            if (debug) { Debug(); }

            // delete files
            foreach (String diretoryPath in directoriesToDelete)
            {
                System.IO.Directory.Delete(diretoryPath, true);
            }

        }




        static void Debug()
        {
            ShowInfo("continue? [y/n]");
            if (!(Console.ReadLine().ToLower().Equals("y")))
            {
                ShowInfo("nothing deleted");
                Environment.Exit(0);
            }

        }


        static void ShowError(String message, OptionSet p)
        {
            Console.WriteLine("ERROR: {0}", message);
            Console.WriteLine();
            ShowHelp(p);
        }

        static void ShowInfo(String message)
        {
            Console.WriteLine("Info: {0}", message);
            Console.WriteLine();
        }


        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: CleanUpTool.exe [OPTIONS]+ path");
            Console.WriteLine("delete files or folders older then x days");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Examples: ");
            Console.WriteLine();
            Console.WriteLine("Deleting all pdf files older then 4 days in C:\\Temp");
            Console.WriteLine("CleanUpTool.exe --mode=file -d=4 -f=*.pdf C:\\Temp");
            Console.WriteLine();
            Console.WriteLine("Deleting all folders (including content) older then 7 days in C:\\Trash");
            Console.WriteLine("CleanUpTool.exe --mode=folder -d=7 C:\\Trash");
            Environment.Exit(0);
        }
    }
}
