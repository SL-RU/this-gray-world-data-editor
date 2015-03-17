using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgwEditor
{
    public class AppSettings
    {
        public static string configFilePath = "config.prop";
        public static string FolderPath = "";

        public static bool ShowInputBoxWindowOnLuaAutocomplete = true;

        public static void Load()
        {
            LoadConfigFile();
            LoadLuaAPIHelp();
        }


        public static void UpdateConfigFile()
        {
            File.WriteAllText(configFilePath, "path=" + FolderPath);
        }

        public static void LoadConfigFile()
        {
            if (File.Exists(configFilePath))
            {
                sData.LOG("Loading configuration");

                var v = File.ReadAllLines(configFilePath);
                foreach (string s in v)
                {
                    string val = s.Split('=')[1];
                    string name = s.Split('=')[0];

                    switch (name)
                    {
                        case "path": FolderPath = val; break;
                    }
                }

                sData.LOG("Configuration loaded");
            }
            else
            {
                sData.LOG("Configuration file is lost");

            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        public static string LuaAPIfile = "lua.txt";
        /// <summary>
        /// Dictionary with help for Lua API
        /// Key - namespace of functions
        /// Value - functions in format: 
        ///         function_name($(var_name|type), 0) --description
        /// </summary>
        public static Dictionary<string, List<string>> LuaAPIHelp;

        /// <summary>
        /// Load Lua API
        /// </summary>
        public static void LoadLuaAPIHelp()
        {
            sData.LOG("Loading Lua API help");
            if (File.Exists(LuaAPIfile))
            {
                var file = File.ReadAllLines("lua.txt", Encoding.UTF8);

                LuaAPIHelp = new Dictionary<string, List<string>>();
                List<string> sss = null;

                foreach (var c in file)
                {
                    if (c.StartsWith("#"))
                    {
                        sss = new List<string>();
                        LuaAPIHelp.Add(c.Substring(1), sss);
                    }
                    else
                    {
                        if (sss != null)
                        {
                            sss.Add(c);
                        }
                    }
                }
                sData.LOG("Loading Lua API help. DONE!");
            }
            else
            {
                sData.LOG_WARNING("Loading Lua API help. FAILED!");
                sData.LOG_WARNING("LUA API FILE " + LuaAPIfile + " NOT FOUND!");
            }
        }
    }
}
