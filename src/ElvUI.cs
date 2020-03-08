using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace ElvUIUpdate
{
    public class ElvUI
    {
        Regex regExVersion = new Regex(@"(?:(\d+)\.)?(?:(\d+)\.)?(?:(\d+)\.\d+)", RegexOptions.Compiled);
        Regex regExHTMLContentClassic = new Regex(@"(<strong>(.*?)<\/strong>)", RegexOptions.Compiled);
        Regex regExHTMLContentRetail = new Regex(@"(<u><b>(.*?)<\/b><\/u>)", RegexOptions.Compiled);
        WebClient client;
        Config config;

        float localClassicVersion = 0f;
        float localRetailVersion = 0f;

        float remoteClassicVersion = 0f;
        float remoteRetailVersion = 0f;

        bool newClassicVersion = false;
        bool newRetailVersion = false;

        Thread classicThread;
        Thread retailThread;

        int interval = 60;

        string ConfigFilePath { get; set; }
        string DownloadFolderPath { get; set; }
        string DownloadFilePath { get; set; }

        string ClassicAddonsPath { get; set; }
        string ClassicElvUIPath { get; set; }

        string RetailAddonsPath { get; set; }
        string RetailElvUIPath { get; set; }

        public ElvUI()
        {
            ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

            config = new Config(ConfigFilePath);

            ClassicAddonsPath = config.Read("Settings", "Path") + @"\_classic_\Interface\AddOns";
            ClassicElvUIPath = ClassicAddonsPath + @"\ElvUI\ElvUI.toc";

            RetailAddonsPath = config.Read("Settings", "Path") + @"\_retail_\Interface\Addons";
            RetailElvUIPath = RetailAddonsPath + @"\ElvUI\ElvUI.toc";

            DownloadFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            DownloadFilePath = Path.Combine(DownloadFolderPath, "elvui.zip");

            interval = int.Parse(config.Read("Settings", "Interval"));
        }

        public void Start()
        {
            if (!Directory.Exists(DownloadFolderPath))
                Directory.CreateDirectory(DownloadFolderPath);

            if (localClassicVersion == 0f)
                ReadLocalClassicVersion();

            classicThread = new Thread(new ThreadStart(CheckForClassic));
            classicThread.Start();

            //retailThread = new Thread(new ThreadStart(CheckForRetail));
            //retailThread.Start();
        }

        public void Stop()
        {
            config.Dispose();
            classicThread.Abort();
            retailThread.Abort();
            CleanDownloadFolder();
        }

        void CheckForRetail()
        {
            while (true)
            {
                ReadLocalRetailVersion();
                ReadRemoteRetailVersion();

                if (newRetailVersion)
                {
                    CleanDownloadFolder();

                    Directory.Delete(RetailAddonsPath + @"\ElvUI", true);
                    Directory.Delete(RetailAddonsPath + @"\ElvUI_OptionsUI", true);

                    client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri("https://www.tukui.org/classic-addons.php?download=2"), DownloadFilePath);
                }

                Thread.Sleep(interval * 60000);
            }
        }

        void CheckForClassic()
        {
            while (true)
            {
                ReadLocalClassicVersion();
                ReadRemoteClassicVersion();

                if (newClassicVersion)
                {
                    CleanDownloadFolder();

                    Directory.Delete(ClassicAddonsPath + @"\ElvUI", true);
                    Directory.Delete(ClassicAddonsPath + @"\ElvUI_OptionsUI", true);

                    client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri("https://www.tukui.org/classic-addons.php?download=2"), DownloadFilePath);
                }

                Thread.Sleep(interval * 60000);
            }
        }

        void ReadLocalClassicVersion()
        {
            Match localVers = regExVersion.Match(File.ReadAllText(ClassicElvUIPath));
            localClassicVersion = Single.Parse(localVers.Value, CultureInfo.InvariantCulture);
        }

        void ReadLocalRetailVersion()
        {
            Match localVers = regExVersion.Match(File.ReadAllText(RetailElvUIPath));
            localRetailVersion = Single.Parse(localVers.Value, CultureInfo.InvariantCulture);
        }

        void ReadRemoteClassicVersion()
        {
            client = new WebClient();
            string htmlCode = client.DownloadString("https://www.tukui.org/classic-addons.php?id=2");
            Match removeVers = regExVersion.Match(regExHTMLContentClassic.Match(htmlCode).Value);
            remoteClassicVersion = Single.Parse(removeVers.Value, CultureInfo.InvariantCulture);
            newClassicVersion = (remoteClassicVersion > localClassicVersion);
        }

        void ReadRemoteRetailVersion()
        {
            client = new WebClient();
            string htmlCode = client.DownloadString("https://www.tukui.org/download.php?ui=elvui");
            Match removeVers = regExVersion.Match(regExHTMLContentRetail.Match(htmlCode).Value);
            remoteClassicVersion = Single.Parse(removeVers.Value, CultureInfo.InvariantCulture);
            newClassicVersion = (remoteClassicVersion > localClassicVersion);
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            //Console.Write("Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            using (ZipArchive archive = ZipFile.Open(DownloadFilePath, ZipArchiveMode.Update))
            {
                archive.ExtractToDirectory(ClassicAddonsPath);
            }                  
        }

        void CleanDownloadFolder()
        {
            DirectoryInfo di = new DirectoryInfo(DownloadFolderPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();

            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);
        }
    }
}
