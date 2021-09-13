using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam1.Models
{
    public class LogUser
    {
        [Required]
        [EmailAddress]
        public string LogEmail {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage = "Password must be 8 characters")]
        public string LogPassword {get;set;}
    }
}