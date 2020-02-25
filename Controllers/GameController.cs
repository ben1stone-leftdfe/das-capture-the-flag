using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAS_Capture_The_Flag.ViewModels;
using DAS_Capture_The_Flag.Views.Game;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Capture_The_Flag.Controllers
{
    public class GameController : Controller
    {
        
        public IActionResult Index()
        {
            //var map = new Map();

            //game.GameBoard = new string[5, 5]
            //  {
            //    { "grass", "wall", "wall", "wall", "wall"},
            //    { "wall", "wall", "grass", "grass", "grass"},
            //    { "grass", "grass", "grass", "grass", "grass"},
            //    { "grass", "grass", "grass", "grass", "grass"},
            //    { "grass", "grass", "grass", "grass", "grass"}
            //  };
            //game.PlayerSoldiers = new List<Soldier>();
            //game.PlayerSoldiers.Add(new Soldier("Ben", 100, 1, 1));

            //return View(game);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetNicknamePost(SetNickNameViewModel vm) { 
           // var view = viewmodel;
           if (!ModelState.IsValid)
           {
               return View("SetNickname");

            }

           return RedirectToAction("FindGame", "Game",);
           // return View();
        }

        [HttpGet]
        public IActionResult SetNickname()
        {
            return View();
        }

        public IActionResult FindGame()
        {
            return View();
        }
    }
}