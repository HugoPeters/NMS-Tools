using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Games;
using System;
using System.IO;
using System.Threading;

namespace NMSView
{
    class NMSViewApp
    {
        public static string Credits = "Hugo_Peters, ladislavlang (half-float lib)";
        public static string Version = "0.1";

        static void PrintCredits()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NMS-View by {0}, version {1}", Credits, Version);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void Main(string[] args)
        {
            PrintCredits();

            System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (args.Length > 0 && !File.Exists(args[0]))
            {
                Console.WriteLine("Error: File doesn't seem to exist: " + args[0]);
                return;
            }

            var thread = new Thread(() =>
            {
                var inst = new NMSViewGame();
                var form = new RenderWin(inst);

                form.Show();
                form.Focus();

                NMSViewGame.ReqFname = args.Length > 0 ? args[0] : null;

                var ctx = new GameContext(form.RenderPanel, false);

                inst.Run(ctx);
            });

            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
