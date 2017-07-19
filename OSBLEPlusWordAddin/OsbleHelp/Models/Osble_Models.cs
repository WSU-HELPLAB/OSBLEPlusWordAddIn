using System;
using System.ComponentModel;

//Originally from the http://localhost/services/AuthenticationService.svc
//and http://localhost/services/OsbleServices.svc service references

namespace Osble.Models
{
    public interface IUser
    {
        int IUserId { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string FullName { get; }
        int ISchoolId { get; set; }
        string Identification { get; set; }
        bool IsAdmin { get; set; }
        bool EmailAllActivityPosts { get; set; }
        bool EmailSelfActivityPosts { get; set; }
        bool EmailAllNotifications { get; set; }
        bool EmailNewDiscussionPosts { get; set; }
        int IDefaultCourseId { get; set; }
        ICourse DefalutCourse { get; set; }
    }

    public interface ICourse
    {
        int Id { get; set; }
        string Number { get; set; }
        string NamePrefix { get; set; }
        string Description { get; set; }
        string Name { get; }
        string Semester { get; }
        int Year { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }

    public interface IAssignment
    {
        int Id { get; set; }
        AssignmentTypes AssignmentType { get; set; }
        int CourseId { get; set; }
        ICourse Course { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime ReleaseDate { get; set; }
        DateTime DueDate { get; set; }
    }

    public enum AssignmentTypes : byte
    {
        Basic = 1,
        CriticalReview = 2,
        DiscussionAssignment = 3,
        TeamEvaluation = 4,
        CriticalReviewDiscussion = 5,
        CommitteeDiscussion = 6,
        ReviewOfStudentWork = 7,
        CommitteeReview = 8,
        AggregateAssessment = 9,
        AnchoredDiscussion = 10
    };

    public interface IActivityEvent : IEventLog
    {
        int EventId { get; set; }
        string SolutionName { get; set; }
        string EventName { get; }

        bool CanDelete { get; set; }
        bool CanMail { get; set; }
        bool HideMail { get; set; }
        string EventVisibilityGroups { get; set; }
        string EventVisibleTo { get; set; }
        bool IsAnonymous { get; set; }
        bool CanReply { get; set; }
        bool CanEdit { get; set; }
        bool ShowProfilePicture { get; set; }
        string DisplayTitle { get; set; }
    }

    public interface IEventLog
    {
        int EventLogId { get; set; }
        EventType EventType { get; }
        DateTime EventDate { get; }
        int SenderId { get; set; }
        IUser Sender { get; set; }
        int? CourseId { get; set; }
    }

    public enum EventType
    {
        [Description("Help Requests")] // Finalized
        AskForHelpEvent = 1,
        [Description("Build Event")]
        BuildEvent = 2,
        [Description("Cut Copy Paste Event")]
        CutCopyPasteEvent = 3,
        [Description("Debug Event")]
        DebugEvent = 4,
        [Description("Editor Activity Event")]
        EditorActivityEvent = 5,
        [Description("Runtime Errors")] // Finalized
        ExceptionEvent = 6,
        [Description("Posts")] // Finalized
        FeedPostEvent = 7,
        [Description("Helpful Responses")] // Finalized
        HelpfulMarkGivenEvent = 8,
        [Description("Log Comment Event")]
        LogCommentEvent = 9,
        [Description("Save Event")]
        SaveEvent = 10,
        [Description("Assignment Submissions")] // Finalized
        SubmitEvent = 11,
        [Description("Null")]
        Null = 12
    }
}
