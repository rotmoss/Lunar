using System;
using System.IO;

namespace Lunar
{
    internal partial class FileManager
    {
        public static readonly string Seperator = System.IO.Path.DirectorySeparatorChar.ToString();
        public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + Seperator + "GameData" + Seperator;

        public static string[] GetDirectories(string directory, out bool error)
        {
            try
            {
                error = false;
                return Directory.GetFiles(Path + directory, "*.*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return null;
            }
        }

        public static string FindFile(string file, string directory)
        {
            if (file == null) { return null; }

            string[] directories = GetDirectories(directory, out bool dir_error);

            if (!dir_error)
            {
                foreach (string temp in directories)
                {
                    string[] tokens = temp.Split(Seperator);

                    if (file.ToLower() == tokens[tokens.Length - 1].ToLower())
                    {
                        return temp;
                    }
                }
            }
            return "";
        }

        public static bool ReadLines(string file, string directory, out string[] value)
        {
            string[] directories = GetDirectories(directory, out bool dir_error);

            try
            {
                value = File.ReadAllLines(FindFile(file, directory));
                return true;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public static string ReadText(string file, string directory, out bool error)
        {
            try
            {
                error = false;
                return File.ReadAllText(FindFile(file, directory));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return null;
            }
        }

        private static void WriteLines(string file, string directory, string[] lines, out bool error)
        {
            try
            {
                error = false;
                File.WriteAllLines(FindFile(file, directory), lines);
                return;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                error = true;
                return;
            }
        }    
    }
}