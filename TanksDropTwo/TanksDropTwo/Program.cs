using System;

namespace TanksDropTwo
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TanksDrop game = new TanksDrop())
            {
                game.Run();
            }
        }
    }
#endif
}

