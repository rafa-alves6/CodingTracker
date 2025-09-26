using Spectre.Console;
using System.Globalization;
using static CodingTracker.Input.Enums;
using static CodingTracker.Input.Validation;

namespace CodingTracker.Input
{
    public class UserInput
    {
        public void MainMenu()
        {
            MenuAction actionChoice;
            CodingController controller = new();

            do
            {
                Console.Clear();

                actionChoice = AnsiConsole.Prompt(
                     new SelectionPrompt<MenuAction>()
                     .Title("What do you want to do next?")
                     .AddChoices(Enum.GetValues<MenuAction>())
                 );

                switch (actionChoice)
                {
                    case MenuAction.Create_Session:
                        var (startDate, endDate) = GetSessionDates();
                        // TODO: implement controller creation
                        controller.CreateSession(startDate, endDate);
                        AnsiConsole.MarkupLine("[bold green]Session successfully created![/]");
                        AnsiConsole.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                }
            } while (actionChoice != MenuAction.Leave_App);
        }

        public static (DateTime, DateTime) GetSessionDates()
        {
            string format = GetDateTimeFormat();
            
            while (true)
            {
                var startDateString = AnsiConsole.Ask<string>($"Enter the [green]start[/] date and time in [bold]{format}[/] format: ");
                var endDateString = AnsiConsole.Ask<string>($"Enter the [red]end[/] date and time in [bold]{format}[/] format: ");

                if (!IsValidDateTime(startDateString) || !IsValidDateTime(endDateString))
                {
                    AnsiConsole.MarkupLine($"[red]Invalid date format. Please use {format} and ensure the date is not in the future.[/]");
                    continue;
                }

                var startDate = DateTime.ParseExact(startDateString, format, CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact(endDateString, format, CultureInfo.InvariantCulture);

                if (endDate < startDate)
                {
                    AnsiConsole.MarkupLine("[red]The end date cannot be before the start date.[/]");
                    continue;
                }

                return (startDate, endDate);
            }
        }
    }
}