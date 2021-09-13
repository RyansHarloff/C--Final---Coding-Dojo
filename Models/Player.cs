using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam1.Models
{
    public class Player
    {
        [Key]
        public int PlayerId {get;set;}
        public int UserId {get;set;}
        public int GameId {get;set;}
        public User User {get;set;}
        public Game Game {get;set;}
    }
}