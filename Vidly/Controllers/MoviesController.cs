﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext _context;

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Movies
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.CAN_MANAGE_MOVIES))
                return View("AdminList");

            return View("ReadOnlyList");
        }

        // GET:Movies/Detail
        public ActionResult Detail(int Id)
        {
            var movies = _context.Movies.Include(m => m.GenreType).SingleOrDefault(m => m.Id == Id);
            return View(movies);
        }

        [Authorize(Roles = RoleName.CAN_MANAGE_MOVIES)]
        public ActionResult New()
        {
            var viewModel = new MovieFormViewModel
            {
                GenreTypes = _context.GenreTypes.ToList()
            };
            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new MovieFormViewModel(movie)
                {
                    GenreTypes = _context.GenreTypes.ToList()
                };
                return View("MovieForm", viewModel);
            }

            if (movie.Id == 0)
            {
                _context.Movies.Add(movie);
            }
            else
            {
                var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);
                movieInDb.Name = movie.Name;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.GenreTypeId = movie.GenreTypeId;
                movieInDb.NumberInStock = movie.NumberInStock;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return RedirectToAction("Index", "Movies");
        }

        public ActionResult Edit(int id)
        {
            var movie = _context.Movies.Single(m => m.Id == id);
            var viewModel = new MovieFormViewModel(movie)
            {
                GenreTypes = _context.GenreTypes.ToList()
            };
            return View("MovieForm", viewModel);
        }
    }
}