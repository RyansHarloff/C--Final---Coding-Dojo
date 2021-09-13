using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam1.Models
{
    public class Game
    {
        [Key]
        public int GameId {get;set;}
        [Required]
        public string Title{get;set;}

        [Required]
        public DateTime Date {get;set;}
        [Required]
        public string Duration {get;set;}
        [Required]
        public string Time {get;set;}
        [Required]
        public string Description {get;set;}
        public DateTime CreatedAt {get;set;}=DateTime.Now;
        public DateTime UpdatedAt {get;set;}=DateTime.Now;
        public int UserId {get;set;}
        public User User {get;set;}
        public List<Player> PlayersWhoJoined{get;set;}
    }
}