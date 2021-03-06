﻿using System;

namespace Osble.Model.ProfileCourse
{
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
}
