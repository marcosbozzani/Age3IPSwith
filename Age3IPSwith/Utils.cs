using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Age3IPSwith
{
    public class Utils
    {
        private static readonly string UserCfgPath;
        
        static Utils()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            UserCfgPath = Path.Combine(documents + @"\My Games\Age of Empires 3\Startup\user.cfg");
        }

        public static string FindLauncherPath()
        {
            var keyPrefixs = new[]
            {
                @"SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\",
                @"SOFTWARE\Microsoft\Microsoft Games\"
            };

            var gameVersions = new[]
            {
                @"Age of Empires 3 Expansion Pack 2\1.0",
                @"Age of Empires 3 Expansion Pack 1\1.0",
                @"Age of Empires 3\1.0"
            };

            var binNames = new[]
            {
                "age3y.exe",
                "age3x.exe",
                "age3.exe"
            };

            foreach (var keyPrefix in keyPrefixs)
            {
                foreach (var gameVersion in gameVersions)
                {
                    var key = Registry.LocalMachine.OpenSubKey(keyPrefix + gameVersion);
                    if (key != null)
                    {
                        var value = key.GetValue("SetupPath");
                        if (value != null)
                        {
                            foreach (var binName in binNames)
                            {
                                var path = Path.Combine(value.ToString(), binName);
                                if (File.Exists(path))
                                {
                                    return path;
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }

        public static string ReadAddress()
        {
            if (File.Exists(UserCfgPath))
            {
                using (var reader = new StreamReader(UserCfgPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var data = line.Trim().Split('=');
                        if (data.Length == 2 && data[0].Trim() == "OverrideAddress")
                        {
                            return data[1].Trim().Replace("\"", "");
                        }
                    }
                }
            }

            return "";
        }

        public static void WriteAddress(string address)
        {
            if (File.Exists(UserCfgPath))
            {
                var count = 0;
                var lines = File.ReadAllLines(UserCfgPath);

                foreach (var line in lines)
                {
                    var data = line.Trim().Split('=');
                    if (data.Length == 2 && data[0].Trim() == "OverrideAddress")
                    {
                        lines[count] = string.Format("OverrideAddress=\"{0}\"", address);
                    }

                    count++;
                }

                File.WriteAllLines(UserCfgPath, lines);
            }
            else
            {
                var content = string.Format("OverrideAddress=\"{0}\"", address);
                File.WriteAllText(UserCfgPath, content);
            }
        }

        public static IEnumerable<string> GetMachineIPs()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (var addressInfo in ipProps.UnicastAddresses)
                {
                    if (!IPAddress.IsLoopback(addressInfo.Address)
                        && addressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        yield return addressInfo.Address.ToString();
                    }
                }
            }
        }
    }
}
