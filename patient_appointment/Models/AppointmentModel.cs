using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace patient_appointment.Models
{
    public class AppointmentModel
    {
        
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string doctorName { get; set; }
        public string service { get; set; }
        public int userID { get; set; }
        public int doctorID { get; set; }
        public DateTime appointmentDateAndTime { get; set; }
        public int finalCharge { get; set; }
        public string referenceNo { get; set; }
        public string status { get; set; }
    }

}