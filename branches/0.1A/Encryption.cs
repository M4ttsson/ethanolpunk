using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace Athyl
{
    class Encryption
    {
        public static void ToggleConfigEncryption(string exeConfigName)
        {
            try
            {
                // Öppnar .config filen och hämtar connectionstrings delen
                Configuration config = ConfigurationManager.
                    OpenExeConfiguration(exeConfigName);

                ConfigurationSection section =
                    config.GetSection("nlog");

                if (section.SectionInformation.IsProtected)
                {
                    // dekryptera.
                    section.SectionInformation.UnprotectSection();
                }
                else
                {
                    // kryptera.
                    section.SectionInformation.ProtectSection(
                        "DataProtectionConfigurationProvider");
                }
                // sparar config filen
                config.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
