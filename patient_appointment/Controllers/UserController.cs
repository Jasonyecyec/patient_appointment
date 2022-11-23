using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using patient_appointment.DBModel;
using patient_appointment.Models;

namespace patient_appointment.Controllers
{
    public class UserController : Controller
    {
      
        //========== SIGN UP==================
        public ActionResult SignUp()
        {
            UserModel user = new UserModel();
            return View(user);
        }

        [HttpPost]
        public ActionResult SignUp(UserModel userObject)
        {

            if (ModelState.IsValid)
            {
                //create object of database
                using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
                {
                    //check if email doesnt  exist in users table
                    if (!userDBEntities.users.Any(m => m.email == userObject.email))
                    {
                        //Generate 6 random digit for OTP
                        Random r = new Random();
                        int randNum = r.Next(1000000);
                        string userOtp = randNum.ToString("D6");

                        //hash password
                        string salt = Crypto.GenerateSalt();
                        string password = userObject.password + salt;
                        string hashedPass = Crypto.HashPassword(password);

                        //create object of userTable, then pass the UserModel object data
                        user userTable = new DBModel.user();
                        userTable.first_name = userObject.firstName;
                        userTable.last_name = userObject.lastName;
                        userTable.email = userObject.email;
                        userTable.password = hashedPass;
                        userTable.password_salt = salt;
                        userTable.address = userObject.address;
                        userTable.gender = userObject.gender;
                        userTable.phone_number = userObject.phoneNumber;
                        userTable.birthdate = userObject.birthdate;
                        userTable.date_created = DateTime.Now;
                        userTable.user_photo = "~/Images/User_Photo/default_user_photo.jpg";

                        //create session for OTP, store email and userTable in tempdata
                        Session["otp"] = userOtp;
                        TempData["email"] = userObject.email;
                        TempData["userTable"] = JsonConvert.SerializeObject(userTable);
                        return RedirectToAction("EmailValidation");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Account already exist");
                        return View();
                    }

                }
            }

            return View();
        }

        //========== EMAIL VALIDATION ==============================
        public ActionResult EmailValidation()
        {
            using (MailMessage mail = new MailMessage())
            {
                // send the otp to email
                mail.To.Add(TempData["email"].ToString());
                mail.From = new MailAddress("jason.yecyec023@gmail.com");
                mail.Subject = "OTP";
                string Body = Session["otp"].ToString();
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

            return View();
        }

        [HttpPost]
        public ActionResult EmailValidation(string otp)
        {
            //check if the input OTP is same with OPT in session
            if (otp == Session["otp"].ToString())
            {
                //call saveuserdatabase method
                return RedirectToAction("SaveUserToDatabase");
            }
            else
            {
                //return to page, and render invalid OTP
                ModelState.AddModelError("Error", "Invalid OTP");
                return View();
            }
        }

        //============= SAVING USER TO DATABASE ==================
        public ActionResult SaveUserToDatabase()
        {
            //pass the userTable tempdata to new userTable object, then save to DB
            user userTable = JsonConvert.DeserializeObject<user>(TempData["userTable"].ToString());
            using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
            {
                userDBEntities.users.Add(userTable);
                userDBEntities.SaveChanges();
            }

            //remove OTP session
            Session.Remove("otp");
            //create name session and display in Homepage
            Session["name"] = userTable.first_name + " " + userTable.last_name;
            Session["id"] = userTable.userID;
            Session["email"] = userTable.email;
            Session["userPhoto"] = Url.Content(userTable.user_photo.Trim());
            return RedirectToAction("Index", "Home");

        }

        //============== LOGIN ==========================
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginModel loginObject)
        {
            if (ModelState.IsValid)
            {
                using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
                {
                    //check if user email if exist in DB              
                    var findUser = userDBEntities.users.Where(m => m.email == loginObject.email).FirstOrDefault();

                    if (findUser != null)
                    {
                        string inputPassword = loginObject.password + findUser.password_salt;
                        var hashedPassword = findUser.password;
                        var verifyPassword = Crypto.VerifyHashedPassword(hashedPassword, inputPassword);

                        if (verifyPassword)
                        {
                            //get name and store in session, redirect to homepage
                            Session["name"] = findUser.first_name + " " + findUser.last_name;
                            Session["id"] = findUser.userID;
                            Session["email"] = findUser.email;
                            Session["userPhoto"] = Url.Content(findUser.user_photo.Trim());

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            //if error, render errorm message
                            ModelState.AddModelError("Error", "Wrong password");
                            return View();
                        }

                    }
                    else
                    {
                        //if error, render errorm message
                        ModelState.AddModelError("Error", "Email doesn't exist");
                        return View();
                    }
                }
            }
            return View();
        }

        //============== LOGOUT ==========================
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //============== USER PROFILE ====================
        public ActionResult UserProfile()
        {
            //guard clause
            if (Session["id"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userID = Session["id"].ToString();


            user userModel = new user();
            using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
            {
                userModel = userDBEntities.users.Where(x => x.userID.ToString() == userID).FirstOrDefault();
            }

            return View(userModel);
        }

        [HttpPost]
        public ActionResult UserProfile(user userModel)
        {

            var session_userID = Convert.ToInt32(Session["id"].ToString());
            user user = new user { userID = session_userID, first_name = userModel.first_name, last_name = userModel.last_name, email = userModel.email, address = userModel.address };

            using (patient_appointment_managementEntities userDBEntities = new patient_appointment_managementEntities())
            {

                var userDB = userDBEntities.users.Find(Convert.ToInt32(Session["id"].ToString()));

                userDB.first_name = userModel.first_name;
                userDB.last_name = userModel.last_name;
                userDB.email = userModel.email;
                userDB.address = userModel.address;
                userDBEntities.SaveChanges();
            }

            Session["name"] = userModel.first_name + " " + userModel.last_name;
            TempData["alert-message"] = "updated";
            return View(userModel);
        }
    }
}