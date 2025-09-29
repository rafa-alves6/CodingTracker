using CodingTracker.Controller;
using CodingTracker.Service;
using Spectre.Console;
using static CodingTracker.Input.Enums;

namespace CodingTracker.Input
{
    public class UserInput
    {
        private DateTime? liveSessionStartTime = null;
        private readonly CodingController _codingController = new();
        private readonly CodingService _codingService = new();

        public void MainMenu()
        {
            MenuAction actionChoice;

            do
            {
                Console.Clear();

                var menuPrompt = new SelectionPrompt<MenuAction>()
                    .Title("What do you want to do?")
                    .UseConverter(action => action.ToString().Replace('_', ' '));

                if (liveSessionStartTime.HasValue)
                {
                    AnsiConsole.MarkupLine($"[yellow]Live session in progress. Started at: {liveSessionStartTime.Value:g}[/]\n");
                    menuPrompt.AddChoices(
                        MenuAction.Stop_Live_Session,
                        MenuAction.View_Sessions,
                        MenuAction.Enter_Past_Session,
                        MenuAction.Leave_App
                    );
                }
                else
                {
                    menuPrompt.AddChoices(
                        MenuAction.View_Sessions,
                        MenuAction.Start_Live_Session,
                        MenuAction.Enter_Past_Session,
                        MenuAction.Update_Session,
                        MenuAction.Delete_Session,
                        MenuAction.Reports,
                        MenuAction.Leave_App
                    );
                }

                actionChoice = AnsiConsole.Prompt(menuPrompt);

                switch (actionChoice)
                {
                    case MenuAction.Start_Live_Session:
                        StartLiveSession();
                        break;

                    case MenuAction.Stop_Live_Session:
                        StopLiveSession();
                        break;

                    case MenuAction.Enter_Past_Session:
                        var (startDate, endDate) = _codingService.GetManualSessionDates();
                        _codingController.AddSession(startDate, endDate);
                        AnsiConsole.MarkupLine("\n[bold green]Past session successfully created![/]");
                        break;

                    case MenuAction.View_Sessions:
                        _codingService.ViewFilteredSessionsProcess();
                        break;

                    case MenuAction.Update_Session:
                        _codingService.UpdateSessionProcess();
                        break;

                    case MenuAction.Delete_Session:
                        _codingService.DeleteSessionProcess();
                        break;

                    case MenuAction.Reports:
                        _codingService.ReportsProcess();
                        break;

                    case MenuAction.Leave_App:
                        if (liveSessionStartTime.HasValue)
                        {
                            AnsiConsole.MarkupLine("[bold red]Warning: A live session is still running. It will be discarded.[/]");
                        }
                        AnsiConsole.MarkupLine("[bold blue]Goodbye![/]");
                        continue;
                }

                AnsiConsole.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();

            } while (actionChoice != MenuAction.Leave_App);
        }

        private void StartLiveSession()
        {
            liveSessionStartTime = DateTime.Now;
            AnsiConsole.MarkupLine($"[bold green]Live session started at {liveSessionStartTime:g}! :stopwatch:[/]");
            AnsiConsole.MarkupLine("[grey]You can now return to the menu to perform other actions.[/]");
        }

        private void StopLiveSession()
        {
            if (!liveSessionStartTime.HasValue)
            {
                AnsiConsole.MarkupLine("[red]Error: No live session is currently running.[/]");
                return;
            }

            var endTime = DateTime.Now;
            var startTime = liveSessionStartTime.Value;

            _codingController.AddSession(startTime, endTime);

            AnsiConsole.MarkupLine($"[bold red]Session stopped at {endTime:g}.[/]");
            AnsiConsole.MarkupLine($"[bold yellow]Total duration: {(endTime - startTime):hh\\:mm\\:ss}.[/]");

            liveSessionStartTime = null;
        }
    }
}