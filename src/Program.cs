using System;
using System.IO;
using Topshelf;

namespace ElvUIUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ElvUI>(s =>
                {
                    s.ConstructUsing(name => new ElvUI());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.SetDescription("Check for ElvUI Updates");
                x.SetDisplayName("ElvUI Updater");
                x.SetServiceName("ElvUIUpdater");
            });
        }
    }
}
