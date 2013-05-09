using System;
namespace Athyl
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Encryption.ToggleConfigEncryption("Athyl.exe");
            using (Game1 game = new Game1())
            {
                Encryption.ToggleConfigEncryption("Athyl.exe");
                game.Run();

            }
        }
    }
#endif
}

