using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;


namespace SyncMedia
{
    public class XmlData
    {
        //public interface IAllMedia
        //{
        //    String Hash { get; set; }
        //}

        //public class AllMedia : IAllMedia
        //{
        //    public AllMedia(string hash)
        //    {
        //        this.Hash = hash;
        //    }
        //    public string Hash { get; set; }
        //}

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

        public static string CreateXmlDoc(string file, List<string> hashlist)
        {
            try
            {
                XDocument xdoc = new XDocument(new XElement("hashes", from hash in hashlist
                                                                      select new XElement("hash", hash)
                                                                      ));


                xdoc.Save(@file);
            }
            catch (Exception)
            {

                return "Error";
            }
            return "Saved";
        }

        public static IEnumerable<string> GetHashesList(string file)
        {

            XElement po = XElement.Load(file);
           return from el in po.Elements("hash")
                select el.Value.ToString();

            //return XDocument.Load(file)
            //.Elements("hash")
            //.Select(f => new T
            //{
            //    Hash = f.Element("Hash").Value,
            //});

        }
    }
}
