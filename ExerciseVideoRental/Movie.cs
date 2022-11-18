using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseVideoRental
{
    public enum MovieType { New_Release, Regular_Rental, Old_Film }
    internal class Movie
    {
        public int Id { get; }
        public string Name { get; set; }
        public MovieType Type { get; set; }
        public const float basicPrice = 3;
        public const float premiumPrice  = 4;
        public bool paidWithBonus = false;
        public float price { get; set; }
        public Movie(int id, string name, MovieType type)
        {
            Id = id;
            Name = name;
            Type = type;
            if (type == MovieType.New_Release)
                price = premiumPrice;
            else
                price = basicPrice;
        }
        public Movie(string name, MovieType type)
        {
            Id = RentalStore.getLastId();
            Name = name;
            Type = type;
            if (type == MovieType.New_Release)
                price = premiumPrice;
            else
                price = basicPrice;
        }
        public void ChangeMovieType(MovieType type) => Type = type;
    }
}
