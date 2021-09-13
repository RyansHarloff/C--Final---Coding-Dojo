using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BeltExam1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace BeltExam1.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in Use!");
                    return View("Index");
                }else{
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("loggedIn", newUser.UserId);
                    return RedirectToAction("Dashboard");
                }
            }else{
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser logUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == logUser.LogEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                    return View("Index");
                    }else{
                        PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>(); var Result = Hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LogPassword);
                        if(Result == 0)
                        {
                            ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                            return View("Index");
                        }
                        HttpContext.Session.SetInt32("loggedIn", userInDb.UserId);
                        return RedirectToAction("Dashboard");
                    }
            }else{
                return View("Index");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                ViewBag.AllGames = _context.Games.Include(f => f.PlayersWhoJoined).Include(f => f.User).OrderBy(d => d.Date).ToList();
                ViewBag.Users = _context.Users.Include(w => w.AllGames).ThenInclude(d => d.PlayersWhoJoined).FirstOrDefault(a => a.UserId == (int)loggedIn);
                return View();
            }else{
                return RedirectToAction("Index");
            }
        }

        [HttpGet("AddGame")]
        public IActionResult AddGame()
        {
            return View();
        }

        [HttpPost("AddGameToDb")]
        public IActionResult AddGameToDb(Game newGame)
        {
            if(ModelState.IsValid)
            {
                newGame.UserId = (int)HttpContext.Session.GetInt32("loggedIn");
                _context.Add(newGame);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }else{
                return View("AddGame");
            }
        }

        [HttpGet("Join/{UserId}/{GameId}/{GoTo}")]

        public IActionResult Join(int UserId, int GameId, string GoTo)
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                if((int) loggedIn != UserId)
                {
                    return RedirectToAction("LogOut");
                } else{
                    Player newPlayer = new Player();
                    newPlayer.UserId = UserId;
                    newPlayer.GameId = GameId;
                    _context.Players.Add(newPlayer);
                    _context.SaveChanges();
                    if(GoTo == "Dashboard")
                    {
                        return RedirectToAction("Dashboard");
                    }else 
                    {
                        return Redirect("/OneGame/" + GameId);
                    }
                    }
            }else{
                return RedirectToAction("Index");
            }
        }

        [HttpGet("Unjoin/{UserId}/{GameId}/{GoTo}")]

        public IActionResult UnJoin(int UserId, int GameId,string GoTo)
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                if((int) loggedIn != UserId)
                {
                    return RedirectToAction("LogOut");
                } else{
                    Player PlayerNotJoin = _context.Players.FirstOrDefault(b => b.GameId == GameId && b.UserId == UserId);
                    _context.Players.Remove(PlayerNotJoin);
                    _context.SaveChanges();
                    if(GoTo == "Dashboard")
                    {
                        return RedirectToAction("Dashboard");
                    }else 
                    {
                        return Redirect("/OneGame/" + GameId);
                    }
                }
            }else{
                return RedirectToAction("Index");
            }
        }

        [HttpGet("OneGame/{GameId}")]
        public IActionResult OneGame(int GameId)
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                Game OneGame = _context.Games.Include(f => f.User).Include(k =>k.PlayersWhoJoined).ThenInclude(u => u.User).FirstOrDefault(p => p.GameId == GameId);
                ViewBag.User = _context.Users.Include(s => s.AllGames).FirstOrDefault(a => a.UserId == (int)loggedIn);
                return View(OneGame);
            }else{
                return RedirectToAction("Index");
            }
        }

        [HttpGet("/delete/{GameId}")]
        public IActionResult Delete(int GameId)
        {
            Game GameToDelete = _context.Games.SingleOrDefault(y => y.GameId == GameId);
            _context.Remove(GameToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
