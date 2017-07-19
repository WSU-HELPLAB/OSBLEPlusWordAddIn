using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Osble.Models;

//SubmissionRequest is in the OSBLEPlus.Logic.DomainObjects.ActivityFeeds namespace on the server
//namespaces must be equivalent to send a SubmissionReuqest object to the server
namespace OSBLEPlus.Logic.DomainObjects.ActivityFeeds
{
    [Serializable]
    public class ActivityEvent : IActivityEvent
    {
        // EventLogs table contents
        public int EventLogId { get; set; }
        public virtual int EventTypeId { get; set; }
        public string EventName { get; }

        public EventType EventType { get { return (EventType)EventTypeId; } }
        public DateTime EventDate { get; set; }
        public int SenderId { get; set; }
        public IUser Sender { get; set; }

        // Detailed events table contents
        public int EventId { get; set; }
        public string SolutionName { get; set; }
        public int? CourseId { get; set; }

        // for posting
        public bool CanMail { get; set; }
        public bool CanDelete { get; set; }
        public bool CanReply { get; set; }
        public bool CanEdit { get; set; }
        public bool CanVote { get; set; }
        public bool ShowProfilePicture { get; set; }
        public string DisplayTitle { get; set; }
        public bool HideMail { get; set; }
        public string EventVisibilityGroups { get; set; }
        public string EventVisibleTo { get; set; }
        public bool IsAnonymous { get; set; }

        public ActivityEvent() // NOTE!! This is required by Dapper ORM
        {
            EventDate = DateTime.UtcNow;
        }
    }

    [Serializable]
    public sealed class SubmitEvent : ActivityEvent
    {
        public int AssignmentId { get; set; }
        public byte[] SolutionData { get; set; }

        public SubmitEvent() // NOTE!! This is required by Dapper ORM
        {
            //EventTypeId = (int)Utility.Lookups.EventType.SubmitEvent;
        }

        public SubmitEvent(DateTime dateTimeValue)
            : this()
        {
            EventDate = dateTimeValue;
        }
    }

    public class SubmissionRequest
    {
        public string AuthToken { get; set; }
        public SubmitEvent SubmitEvent { get; set; }
    }
}
