using CodingTracker.Controller;
using CodingTracker.Entity;
using CodingTracker.Input;
using Spectre.Console;
using System.Globalization;
using static CodingTracker.Input.Validation;

namespace CodingTracker.Service
{
    public class CodingService
    {
        private readonly CodingController _codingController = new();

        public void ViewAllSessions()
        {
            var sessions = _codingController.GetAllSessions();

            var table = new Table();
            table.Title("[yellow]Coding Sessions[/]");
            table.AddColumn("ID");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");

            foreach (var session in sessions)
            {
                table.AddRow(
                    session.Id.ToString(),
                    session.StartTime.ToString("g"),
                    session.EndTime.ToString("g"),
                    $"{session.Duration:hh\\:mm\\:ss}"
                );
            }
            AnsiConsole.Write(table);
        }

        public (DateTime, DateTime) GetManualSessionDates()
        {
            string format = "yyyy-MM-dd HH:mm";

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

        public void DeleteSessionProcess()
        {
            ViewAllSessions();
            var sessionId = AnsiConsole.Ask<int>("Enter the [red]ID[/] of the session you want to [red]delete[/]:");

            var session = _codingController.GetSessionById(sessionId);
            if (session == null)
            {
                AnsiConsole.MarkupLine("[red]Session not found.[/]");
                return;
            }

            _codingController.DeleteSession(sessionId);
            AnsiConsole.MarkupLine("[bold green]Session successfully deleted![/]");
        }

        public void UpdateSessionProcess()
        {
            ViewAllSessions();
            var sessionId = AnsiConsole.Ask<int>("Enter the [yellow]ID[/] of the session you want to [yellow]update[/]:");

            var session = _codingController.GetSessionById(sessionId);
            if (session == null)
            {
                AnsiConsole.MarkupLine("[red]Session not found.[/]");
                return;
            }

            AnsiConsole.MarkupLine("Enter the new dates for the session:");
            var (newStartDate, newEndDate) = GetManualSessionDates();

            session.StartTime = newStartDate;
            session.EndTime = newEndDate;

            _codingController.UpdateSession(session);
            AnsiConsole.MarkupLine("[bold green]Session successfully updated![/]");
        }
    }
}