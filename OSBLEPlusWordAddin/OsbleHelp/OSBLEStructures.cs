using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSBLEPlusWordAddin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

//Osble structures used to create the required ProfileCourse and SubmissionAssignment objects
namespace OSBLEStructures
{
    //Originally from the http://localhost/services/AuthenticationService.svc
    //and http://localhost/services/OsbleServices.svc service references

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

    public class ProfileCourse : ICourse
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string NamePrefix { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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

    public class SubmisionAssignment : IAssignment
    {
        public int Id { get; set; }
        public AssignmentTypes AssignmentType { get; set; }
        public int CourseId { get; set; }
        public ICourse Course { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
