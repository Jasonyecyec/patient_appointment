using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace patient_appointment.Models
{
    public class UserModel
    {
        public int userID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is Required.")]
        [Display(Name = "First name: ")]
        public string firstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is Required.")]
        [Display(Name = "Last name: ")]
        public string lastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required.")]
        [Display(Name = "Email: ")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required.")]
        [Display(Name = "Password: ")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "A minimum 8 characters password contains a combination of uppercase and lowercase letter and number are required.")]
        public string password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password do not match")]
        public string confirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is Required.")]
        [Display(Name = "Address: ")]
        public string address { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is Required.")]
        [Display(Name = "Gender: ")]
        public string gender { get; set; }

        [Display(Name = "Birthdate : ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Birthday is Required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime birthdate { get; set; }

        [Display(Name = "Phone number: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PhoneNumber is Required.")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^[0-9]\{1,11}\$",ErrorMessage = "Not more than 11 digits")]
        public string phoneNumber { get; set; }

        public DateTime dateCreated { get; set; }
    }
}