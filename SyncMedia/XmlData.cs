using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;
using System.IO;


namespace SyncMedia
{
    public class XmlData
    {
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return "Error";
            }
        }

        public static string AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                return "Error";
            }
            return "Success";
        }

        public static string CreateXmlDoc(string filen, List<string> hashlist)
        {
            bool emergency = false;
            if (filen == string.Empty)
            {
                //Emergency save the hashes to the user MyPicture directory and attempt to reload it on next launch.
                AddUpdateAppSettings("EmergencySave", @Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\MediaSync_SaveFile_" + Environment.MachineName + @".xml");
                filen = @Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\MediaSync_SaveFile_" + Environment.MachineName + @".xml";
                emergency = true;
            }
            try
            {
                XDocument xdoc = new XDocument(new XElement("hashes", from hash in hashlist
                                                                      select new XElement("hash", hash)
                                                                      ));

                xdoc.Save(filen);
                try
                {
                    if (emergency == false)
                    {
                        File.Delete(@ReadSetting("EmergencySave"));
                        AddUpdateAppSettings("EmergencySave","");
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                var sd = ex.Message;
                return "Error";
            }
            return "Saved";
        }

        public static IEnumerable<string> GetHashesList(string file)
        {

            XElement po = XElement.Load(file);
           return from el in po.Elements("hash")
                select el.Value.ToString();
        }
    }
}
