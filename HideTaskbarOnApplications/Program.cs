using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideTaskbarOnApplications
{
    internal class Program
    {
        public const string AppName = "HideTaskbarOnApplications";
        public const double Revision = 0.1;
        public static readonly string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        public static readonly string ConfigFile = Path.Combine(AppDataDir, "config.json");
        public static readonly string CurrentProgramPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string NewProgramPath = Path.Combine(AppDataDir, Path.GetFileName(CurrentProgramPath));
        public static readonly bool RunningInstallation = CurrentProgramPath == NewProgramPath;

        static bool runningSilently = false;


        static void Main(string[] args)
        {
            ProcessArgs(args);
            Console.Title = $"{AppName} | Revision {Revision.ToString()}";

            if (!File.Exists(ConfigFile)){
                Console.WriteLine($"Hey there! it looks like it's your first time using {AppName}");
                Console.WriteLine("Press enter to install, otherwise, close the application to abort.");
                Console.ReadLine();
                if (!Directory.Exists(AppDataDir)) Directory.CreateDirectory(AppDataDir);

                JObject ConfigData = new JObject(new JProperty("Revision", Revision), new JProperty("Programs", new string[] { }));

                using (var inStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    string ConfigStr = ConfigData.ToString();
                    inStream.SetLength(0);
                    inStream.Write(new UTF8Encoding(true).GetBytes(ConfigStr), 0, ConfigStr.Length);
                }

                File.Copy(CurrentProgramPath, NewProgramPath, true);
                //TODO: create startup entry
                Process.Start(NewProgramPath);
                Environment.Exit(0);
            }
            else
            {
                JObject ConfigData;
                using (var outStream = new StreamReader(new FileStream(ConfigFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    ConfigData = JObject.Parse(outStream.ReadToEnd());
                }

                double ConfigRevision = ConfigData["Revision"].ToObject<double>();
                if (RunningInstallation)
                {
                    // TODO: Show control panel and run silent (if not running) or run just silently if ran from startup (determined from args)
                }
                else
                {
                    if (Revision > ConfigRevision)
                    {
                        Console.WriteLine($"Upgrade is available. Installed Version: {ConfigRevision.ToString()}, Current Version: {Revision.ToString()}");
                        Console.WriteLine("Press any key to install...");
                        Console.ReadKey();

                        foreach (Process proc in Process.GetProcessesByName(AppName)) {
                            if (Process.GetCurrentProcess().Id != proc.Id){
                                proc.Kill();
                            }
                        }

                        ConfigData["Revision"] = Revision;
                        using (var inStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                        {
                            string ConfigStr = ConfigData.ToString();
                            inStream.SetLength(0);
                            inStream.Write(new UTF8Encoding(true).GetBytes(ConfigStr), 0, ConfigStr.Length);
                        }
                        File.Copy(CurrentProgramPath, NewProgramPath, true);
                    }

                    Process.Start(NewProgramPath);
                    Environment.Exit(0);
                }
            }
        }

        static void ProcessArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.FirstOrDefault() == '-')
                {
                    switch (arg.Substring(1))
                    {
                        case "startup":
                            runningSilently = true;
                            break;
                    }
                }
            }
        }
    }
}
