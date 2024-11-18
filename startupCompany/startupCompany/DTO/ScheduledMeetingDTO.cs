namespace startupCompany.DTO
{
    public class ScheduledMeetingDTO
    {
        public string Email { set; get; }
        //public string SubjectMeeting { set; get; }
        public string MeetingPlatform { set; get; }
        public string MeetingLink { set; get; }
        public string desc { set; get; }
        public DateTime MeetingDate { set; get; }
        public string? ClientPhone { get; set; }
        public string? ClientName { get; set; }

    }
}
