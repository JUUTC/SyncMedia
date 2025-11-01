using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;


namespace SyncMedia
{
    public class XmlData
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SyncMedia"
        );
        
        private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.xml");

        // Ensure settings directory exists
        static XmlData()
        {
            try
            {
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }
            }
            catch
            {
                // If we can't create the directory, settings will fail gracefully
            }
        }

        // Load all settings from XML file
        private static XDocument LoadSettingsDocument()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    return XDocument.Load(SettingsFilePath);
                }
            }
            catch
            {
                // If load fails, return new document
            }
            
            // Return new settings document
            return new XDocument(new XElement("settings"));
        }

        // Save settings document to file
        private static void SaveSettingsDocument(XDocument doc)
        {
            try
            {
                doc.Save(SettingsFilePath);
            }
            catch
            {
                // Silently fail if save doesn't work
            }
        }

        public static string ReadSetting(string key)
        {
            try
            {
                var doc = LoadSettingsDocument();
                var element = doc.Root?.Elements("setting")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == key);
                
                return element?.Attribute("value")?.Value ?? "";
            }
            catch
            {
                return "";
            }
        }

        public static string AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var doc = LoadSettingsDocument();
                var root = doc.Root;
                
                if (root == null)
                {
                    root = new XElement("settings");
                    doc.Add(root);
                }

                var element = root.Elements("setting")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == key);
                
                if (element == null)
                {
                    // Add new setting
                    root.Add(new XElement("setting",
                        new XAttribute("key", key),
                        new XAttribute("value", value ?? "")));
                }
                else
                {
                    // Update existing setting
                    element.SetAttributeValue("value", value ?? "");
                }
                
                SaveSettingsDocument(doc);
                return "Success";
            }
            catch
            {
                return "Error";
            }
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
