using IniParser;
using IniParser.Model;
using System;
using System.IO;

namespace ElvUIUpdate
{
    public class Config : IDisposable
    {
        string filePath;
        FileIniDataParser parser;
        IniData data;

        public Config(string _filePath)
        {
            filePath = _filePath;

            parser = new FileIniDataParser();
            data = new IniData();

            if (!File.Exists(filePath))
            {
                WriteDefaultConfig();
            }

            data = parser.ReadFile(filePath);
        }

        void WriteDefaultConfig()
        {
            //Add a new section and some keys
            data.Sections.AddSection("Settings");

            KeyData key = new KeyData("Path");
            key.Comments.Add("Path to your World of Warcraft installation");
            key.Value = @"C:\Program Files (x86)\World of Warcraft";
            data["Settings"].AddKey(key);

            key = new KeyData("Interval");
            key.Comments.Add("How often should it check for new Updates in minutes");
            key.Value = "60";
            data["Settings"].AddKey(key);

            key = new KeyData("Autostart");
            key.Comments.Add("Should the program start with Windows? (False = off | True = on)");
            key.Value = "False";
            data["Settings"].AddKey(key);

            key = new KeyData("AutostartCheck");
            key.Comments.Add("Should checking process start automatically? (False = off | True = on)");
            key.Value = "False";
            data["Settings"].AddKey(key);

            parser.WriteFile(filePath, data);
        }

        public void Write(string section, string key, string value)
        {
            data[section][key] = value;
        }

        public string Read(string section, string key)
        {
            return data[section][key];
        }

        ~Config()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            parser.WriteFile(filePath, data);
        }
    }
}
