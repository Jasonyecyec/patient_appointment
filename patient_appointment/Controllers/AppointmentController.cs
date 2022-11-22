using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using patient_appointment.DBModel;
using patient_appointment.Models;

namespace patient_appointment.Controllers
{
    public class AppointmentController : Controller
    {
        // GET: Appointment
        public ActionResult Appointment(string selected = "Surgery")
        {

            patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities();


            ServiceAndDoctor model = new ServiceAndDoctor() {
                services = userDBEntities.doctors.Select(m => m.specialty).Distinct(),
                doctors = userDBEntities.doctors.Where(m => m.specialty == selected).Select(m => new doctorObject { doctorID = m.doctorID, doctorName = m.doctor_name, specialty = m.specialty} )
                };

            ViewBag.selected = selected;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Appointment(FormCollection formCollection)
        {

            return RedirectToAction("ConfirmAppointment",new {service = formCollection["service"], doctorName = formCollection["doctor"], date = formCollection["appointment-date"] });
        }


        public ActionResult ConfirmAppointment(string service, string doctorName, string date)
        {
            //convert to int
            //var session_userID = Convert.ToInt32(Session["id"].ToString());
            var doctorID =  Convert.ToInt32(doctorName);

            patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities();


            AppointmentModel appointmentInfo = new AppointmentModel();
            
            //Get user info from db
            var userInfo = userDBEntities.users.Where(m => m.userID == 3).Select(m => new {
             m.email,
              m.phone_number
            }).FirstOrDefault();

            //get doctor info from db
            var doctorInfo = userDBEntities.doctors.Where(m => m.doctorID == doctorID).Select(m => new{
             m.doctor_name,
             m.specialty
            }).FirstOrDefault();

            appointmentInfo.userID = 3;
            appointmentInfo.doctorID = doctorID;
            appointmentInfo.email = userInfo.email;
            appointmentInfo.phoneNumber = userInfo.phone_number;
            appointmentInfo.doctorName = doctorInfo.doctor_name;
            appointmentInfo.service = service;
            appointmentInfo.appointmentDateAndTime = DateTime.Parse(date);
            appointmentInfo.finalCharge = servicePrice(service);
            appointmentInfo.status = "Unpaid";
            appointmentInfo.referenceNo = generateReference(userDBEntities);


            return View(appointmentInfo);
        }

        public string generateReference(patient_appointment_managementEntities userDBEntities)
        {
            //generate referene no
            char[] words = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            Random random = new Random();
            string randomRef = "";

            for(int i=0; i< 15; i++)
            {
                randomRef += words[random.Next(0, 35)].ToString();
            }
           

            //return reference no
            return randomRef.ToString();
        }

        public int servicePrice(string service)
        {
            int price;
            switch (service)
            {
                case "Blood Analysis":
                    price =  1000;
                break;
                case "Cardiology":
                    price = 12000;
                    break;
                case "Dental Care":
                    price = 13000;
                    break;
                case "Dermatology":
                    price = 15000;
                    break;
                case "General":
                    price = 10000;
                    break;
                case "Gynecologist":
                    price = 29000;
                    break;
                case "Neurology":
                    price = 20000;
                    break;
                case "OB/GYN":
                    price = 4000;
                    break;
                case "Pediatrics":
                    price = 3000;
                    break;
                case "Primary Care":
                    price = 5000;
                    break;
                case "Psychiatry":
                    price = 10000;
                    break;
                case "Radiology":
                    price = 500;
                    break;
                case "Surgery":
                    price = 75000;
                    break;
                default:
                    price =  -1;
               break;

            }

            return price;
        }



    }
}