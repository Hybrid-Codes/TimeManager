using System;
using System.Collections.Generic;


namespace TimeManagerLibrary
{
    public class Module
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ClassHoursPerWeek { get; set; }
        public double SelfStudyHours { get; set; }
        
        public double CalculateSelfStudyHours(int weeksInSemester)
        {
            return (Credits * 10.0) / (weeksInSemester - ClassHoursPerWeek);
        }
        
        public List<StudyRecord> StudyRecords { get; set; } = new List<StudyRecord>();
    }
    public class StudyRecord
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }
    public class Semester
    {
        public List<Module> Modules { get; set; }
        public int WeeksInSemester { get; set; }
        public DateTime StartDate { get; set; }

        public Semester()
        {
            Modules = new List<Module>();
        }
    }
}


   
