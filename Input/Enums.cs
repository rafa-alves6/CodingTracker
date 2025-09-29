namespace CodingTracker.Input
{
    public class Enums
    {
        public enum MenuAction
        {
            View_Sessions,
            Start_Live_Session,
            Stop_Live_Session,
            Enter_Past_Session,
            Update_Session,
            Delete_Session,
            Reports,
            Leave_App
        }

        public enum TimePeriod
        {
            All_Time,
            Last_7_Days,
            Last_30_Days,
            This_Year
        }

        public enum SortOrder
        {
            Ascending,
            Descending
        }
    }
}