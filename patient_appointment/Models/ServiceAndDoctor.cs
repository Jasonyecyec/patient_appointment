using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace patient_appointment.Models
{
    public class ServiceAndDoctor
    {

        public IEnumerable<String> services { get; set; }
        public IEnumerable<doctorObject> doctors { get; set; }

    }

    public class doctorObject
    {
        public int doctorID { get; set; }
        public string doctorName { get; set; }
        public string specialty { get; set; }
    }
}