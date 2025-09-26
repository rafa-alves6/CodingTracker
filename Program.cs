using CodingTracker.Input;

namespace CodingTracker;

class Program
{
    static void Main(string[] args)
    {
        UserInput ui = new();
        ui.MainMenu();
    }
}
