using System;

namespace MonoGameTest.Client
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TestPathfinder())
                game.Run();
        }
    }
}
