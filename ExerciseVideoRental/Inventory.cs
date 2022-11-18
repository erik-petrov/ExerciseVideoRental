using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseVideoRental
{
    internal class Inventory
    {
        static private List<Movie> Movies = new List<Movie>();
        static public List<Movie> AvailableMovies = new List<Movie>();
        static public Dictionary<Movie, int> RentedMovies = new Dictionary<Movie, int>();
        static public void AddMovies(List<Movie> movies)
        {
            Movies.AddRange(movies);
            AvailableMovies.Concat(movies);
        }
        static public void AddMovie(Movie movie)
        {
            Movies.Add(movie);
            AvailableMovies.Add(movie);
        }
        static public void RemoveMovie(Movie movie)
        {
            Movies.Remove(movie);
            AvailableMovies.Remove(movie);
        }
        static public void ChangeMovieType(Movie movie, MovieType targetType) => movie.ChangeMovieType(targetType);
        static public Movie GetMovieById(int id)
        {
            Movie movie = Movies.Find(m => m.Id == id);
            if (movie != null)
            {
                return movie;
            }
            else
            {
                return null;
            }
        }
        static public void ClearMovies()
        {
            Movies.Clear();
            AvailableMovies.Clear();
        }
        static public List<Movie> ListMovies() { return Movies; }
        static public IEnumerable<Movie> ListAvailableMovies() { return AvailableMovies; }
    }
}
