using CodingTracker.Controller;
using CodingTracker.Entity;
using Spectre.Console;
using System.Globalization;
using static CodingTracker.Input.Enums;
using static CodingTracker.Input.Validation;

namespace CodingTracker.Service
{
    public class CodingService
    {
        private readonly CodingController _codingController = new();

        public void ViewFilteredSessionsProcess()
        {
            var period = AnsiConsole.Prompt(
                new SelectionPrompt<TimePeriod>()
                    .Title("Select a period to view:")
                    .AddChoices(Enum.GetValues<TimePeriod>()));

            var order = AnsiConsole.Prompt(
                new SelectionPrompt<SortOrder>()
                    .Title("Select the order:")
                    .AddChoices(Enum.GetValues<SortOrder>()));
            
            var sessions = _codingController.GetSessionsByPeriod(period, order);
            DisplaySessionsTable(sessions, $"Coding Sessions - {period.ToString().Replace('_', ' ')} ({order})");
        }
        
        public void ReportsProcess()
        {
            var period = AnsiConsole.Prompt(
                new SelectionPrompt<TimePeriod>()
                    .Title("Select a period for the report:")
                    .AddChoices(Enum.GetValues<TimePeriod>()));

            var sessions = _codingController.GetSessionsByPeriod(period, SortOrder.Ascending);

            if (!sessions.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]No sessions found for the selected period: {period}.[/]");
                return;
            }

            var totalDuration = TimeSpan.FromSeconds(sessions.Sum(s => s.Duration.TotalSeconds));
            var averageDuration = TimeSpan.FromSeconds(sessions.Average(s => s.Duration.TotalSeconds));

            var panel = new Panel(
                $"[bold]Total Coding Time:[/] {totalDuration:hh\\:mm\\:ss}\n" +
                $"[bold]Average Session Time:[/] {averageDuration:hh\\:mm\\:ss}\n" +
                $"[bold]Total Sessions:[/] {sessions.Count}"
            )
            {
                Header = new PanelHeader($"[cyan]Report for {period.ToString().Replace('_', ' ')}[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };

            AnsiConsole.Write(panel);
        }

        private void DisplaySessionsTable(List<CodingSession> sessions, string title)
        {
            var table = new Table();
            table.Title($"[yellow]{title}[/]");
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
            var allSessions = _codingController.GetAllSessions();
            DisplaySessionsTable(allSessions, "All Coding Sessions");
            
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
            var allSessions = _codingController.GetAllSessions();
            DisplaySessionsTable(allSessions, "All Coding Sessions");

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