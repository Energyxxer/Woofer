using System;
using System.IO;
using GameBase;

using WooferGame.Controller;

namespace WooferGame
{
    public static class Woofer
    {
        public static WooferController Controller;

        public static readonly string DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Woofer");
        public static readonly string ScenesPath = Path.Combine(DirectoryPath, "scenes");

        [STAThread]
        static void Main(string[] args)
        {
            GameStart.Start(Controller = new WooferController());

        }
    }
}
