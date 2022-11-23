using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

            return RedirectToAction("ConfirmAppointment",new {
                service = formCollection["service"],
                doctorName = formCollection["doctor"],
                date = formCollection["appointment-date"],
                message = formCollection["message"]});
        }


        public ActionResult ConfirmAppointment(string service, string doctorName, string date, string message)
        {
            //convert to int
            var session_userID = Convert.ToInt32(Session["id"].ToString());
            var doctorID =  Convert.ToInt32(doctorName);

            patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities();


            AppointmentModel appointmentInfo = new AppointmentModel();
            
            //Get user info from db
            var userInfo = userDBEntities.users.Where(m => m.userID == session_userID).Select(m => new {
                m.first_name,
                m.last_name,
             m.email,
              m.phone_number
            }).FirstOrDefault();

            //get doctor info from db
            var doctorInfo = userDBEntities.doctors.Where(m => m.doctorID == doctorID).Select(m => new{
             m.doctor_name,
             m.specialty
            }).FirstOrDefault();

            appointmentInfo.patientName = userInfo.first_name +" " + userInfo.last_name;
            appointmentInfo.email = userInfo.email;
            appointmentInfo.phoneNumber = userInfo.phone_number;
            appointmentInfo.doctorName = doctorInfo.doctor_name;
            appointmentInfo.service = service;
            appointmentInfo.appointmentDateAndTime = DateTime.Parse(date);
            appointmentInfo.finalCharge = servicePrice(service);
            appointmentInfo.status = "Unpaid";
            appointmentInfo.referenceNo = generateReference(userDBEntities);
            appointmentInfo.message = message;


            return View(appointmentInfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAppointment(AppointmentModel appointmentModel)
        {
    

            using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
            {    //create object of appointmentScheduleTable, then pass the appointmentModel object data
                appointment_schedule appointmentScheduleTable = new DBModel.appointment_schedule();

                int doctorID = Convert.ToInt32(appointmentModel.doctorName);
                string doctorName = userDBEntities.doctors.Where(m => m.doctorID == doctorID).Select(m => m.doctor_name).FirstOrDefault();
                appointmentScheduleTable.patient_name = appointmentModel.patientName;
                appointmentScheduleTable.email = appointmentModel.email;
                appointmentScheduleTable.phone_number = appointmentModel.phoneNumber;
                appointmentScheduleTable.doctor_name = doctorName;
                appointmentScheduleTable.date_and_time = appointmentModel.appointmentDateAndTime;
                appointmentScheduleTable.reference_no = appointmentModel.referenceNo;
                appointmentScheduleTable.total_charge = appointmentModel.finalCharge;
                appointmentScheduleTable.service = appointmentModel.service;
                appointmentScheduleTable.appointment_status = appointmentModel.status;
                appointmentScheduleTable.message = appointmentModel.message;


                userDBEntities.appointment_schedule.Add(appointmentScheduleTable);
                userDBEntities.SaveChanges();

                //convert datetime to string
                DateTime now = appointmentModel.appointmentDateAndTime;
                string dateAppointment = now.ToString("MMMM dd yyyy hh:mm tt");

                //Send email confirmation
                SendEmailAppointmentSchedule(appointmentModel.email, dateAppointment,
                    doctorName, appointmentModel.service, appointmentModel.finalCharge.ToString(), appointmentModel.patientName);


            }

            return RedirectToAction("AppointmentHistory", "Appointment");

        }

        public void SendEmailAppointmentSchedule(string patientEmail, string date, string doctorName, string service, string charge, string name) {
            using (MailMessage mail = new MailMessage())
            {
                // send the otp to email
                mail.To.Add(patientEmail);
                mail.From = new MailAddress("jason.yecyec023@gmail.com");
                mail.Subject = "Appointment Schedule";
                string Body = "Hi! "+ name+", your appointment schedule is now confirmed, for "+date+ "\n"+ "Doctor: Dr."+doctorName+ "\n"+ "Service: "+service+"\n"+ "Final charge :"+ charge;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("jason.yecyec023@gmail.com", "hjjwsqbmhhppqemk"); // Enter sender User name and password       
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

        }

        public ActionResult AppointmentHistory()
        {

            patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities();

            string email = Session["email"].ToString();

          AppointmentHistory appointmentList = new AppointmentHistory()
            {
              appointmentHistory = userDBEntities.appointment_schedule.Where(m => m.email == email).Select(m => new appointmentHistoryObject
              {
                  patientName = m.patient_name,
                  doctorName = m.doctor_name,
                  reference = m.reference_no,
                  service = m.service,
                  status = m.appointment_status,
                  dateAndTime = m.date_and_time,
                  totalCharge = m.total_charge
              })};

              
            return View(appointmentList);
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