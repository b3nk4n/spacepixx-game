using System;

namespace SpacepiXX
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Spacepixx game = new Spacepixx())
            {
                game.Run();
            }
        }
    }
#endif
}

