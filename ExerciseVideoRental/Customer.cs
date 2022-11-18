using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseVideoRental
{
    internal class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int BonusPoints { get; set; } = 0;
        public Dictionary<Movie, int> RentedMovies { get; set; } = new Dictionary<Movie, int>();
        public Customer(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public void DumpInfo()
        {
            Console.WriteLine(
                $"Name: {Name}" +
                $"Address: {Address}" +
                $"Bonus points: {BonusPoints}" +
                $"Rented movies: {getRentedMovies()}"
            );
        }
        private string getRentedMovies()
        {
            if(RentedMovies.Count == 0)
            {
                return "";
            }
            string ret = "";
            foreach (Movie item in RentedMovies.Keys)
            {
                ret += item.Name + "\n";
            }
            return ret;
        }
    }
}
