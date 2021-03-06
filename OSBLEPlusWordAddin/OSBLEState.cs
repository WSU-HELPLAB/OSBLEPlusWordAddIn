﻿using System;
using System.Collections.Generic;
using Osble.Model.ProfileCourse;
using Osble.Model.SubmissionAssignment;
using System.Threading;
using Osble.Client.Authentication;
using Osble.Client.AsyncServiceClient;

namespace OSBLEPlusWordAddin
{
    public class OSBLEState
    {
        private string m_user, m_pass, m_authtoken;

        private string m_name;

        private ProfileCourse[] m_courses = null;

        private SubmisionAssignment[] m_assignments = null;

        private EventHandler m_onComplete = null;

        public OSBLEState(string userName, string password)
        {
            m_user = userName;
            m_pass = password;
        }

        public string UserName
        {
            get { return m_user; }
        }

        public string Password
        {
            get { return m_pass; }
        }

        public string AuthToken
        {
            get { return m_authtoken; }
        }

        public string FullName
        {
            get { return m_name; }
        }

        public ProfileCourse[] Courses
        {
            get { return m_courses; }
        }

        public SubmisionAssignment[] Assignments
        {
            get { return m_assignments; }
        }

        /// <summary>
        /// Asyncrhonous form validation function called in the login form.
        /// </summary>
        /// <param name="onComplete">event to be fired in the login form afterwards</param>
        public void RefreshAsync(EventHandler onComplete)
        {
            m_onComplete = onComplete;
            ThreadStart ts = new ThreadStart(RefreshProc);
            Thread t = new Thread(ts);
            t.Start();
        }

        /// <summary>
        /// Verifies the username and password submitted by the login form.
        /// If the information is valid then the user's course will be 
        /// populated and the ribbon will be updated to a logged in state.
        /// Otherwise an error message will appear.
        /// </summary>
        private void RefreshProc()
        {
            //validate login information
            try
            {
                m_authtoken = AuthenticationClient.Login(m_user, m_pass);
                m_name = AsyncServiceClient.GetName(m_authtoken).Result;

                if (m_name == null)
                    m_name = "";
            }

            //connection error
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                m_onComplete(this, new OSBLEStateEventArgs(false,
                    "Could not connect to the OSBLE server. " +
                    "Please contact support if this problem persists."));
                return;
            }

            //credentials error
            if (string.IsNullOrEmpty(m_authtoken))
            {
                m_onComplete(this, new OSBLEStateEventArgs(false,
                    "Could not log into OSBLE. " +
                    "Please check your user name and password."));
                return;
            }

            //login was successful so update collection of the user's courses
            UpdateCourses();

            //don't bother continuing to login if no courses found
            if (null == m_courses || 0 == m_courses.Length)
            {
                m_onComplete(this, new OSBLEStateEventArgs(false,
                    "No courses were found for this user."));
                return;
            }

            //TODO: consider filtering courses by the user's role

            m_onComplete(this, new OSBLEStateEventArgs(true, string.Empty));
        }

        private string Authenticate(string m_user, string m_pass)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous web api call to update the array of courses.
        /// </summary>
        /// <param name="c"></param>
        public void UpdateCourses()
        {
            List<ProfileCourse> c = null;

            //web api call to collect courses
            try
            {
                var task = AsyncServiceClient.GetCoursesForUser(m_authtoken);
                c = task.Result;
            }

            catch (Exception e)
            {
                //TODO: Handle unknown exception
            }


            //set courses to the web api result converted to an array
            if (c == null)
                m_courses = null;
            else
                m_courses = c.ToArray();
        }

        /// <summary>
        /// Asynchronous web api call to update the array of submission assignments.
        /// </summary>
        /// <param name="c"></param>
        public void UpdateAssignments(ProfileCourse c)
        {
            List<SubmisionAssignment> a = null;

            //web api call to collect assignments
            try
            {
                var task = AsyncServiceClient.GetAssignmentsForCourse(c.Id, m_authtoken);
                a = task.Result;
            }

            catch (Exception e)
            {
                //TODO: Handle unknown exception
            }


            //set assignments to the web api result converted to an array
            if (a == null)
                m_assignments = null;
            else
                m_assignments = a.ToArray();
        }
    }

    internal class OSBLEStateEventArgs : EventArgs
    {
        private bool m_success = false;

        private string m_message;

        private OSBLEState m_state = null;

        public OSBLEStateEventArgs(bool success, string message)
        {
            m_message = message;
            m_success = success;
        }

        public OSBLEStateEventArgs(bool success, string message, OSBLEState state)
        {
            m_message = message;
            m_success = success;
            m_state = state;
        }

        public static readonly OSBLEStateEventArgs Empty = new OSBLEStateEventArgs(true, string.Empty);

        public string Message
        {
            get { return m_message; }
        }

        public OSBLEState State
        {
            get { return m_state; }
        }

        public bool Success
        {
            get { return m_success; }
        }
    }
}
