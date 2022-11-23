using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace patient_appointment.Models
{
    public class AppointmentHistory
    {

        public IEnumerable<appointmentHistoryObject> appointmentHistory { get; set; }
    }
    public class appointmentHistoryObject
    {
        public string patientName { get; set; }
        public string doctorName { get; set; }
        public string reference { get; set; }
        public string service { get; set; }
        public string status { get; set; }
        public DateTime dateAndTime { get; set; }
        public int totalCharge { get; set; }
    }
}