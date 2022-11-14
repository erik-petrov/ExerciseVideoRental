using System.Formats.Asn1;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Sources;
using static ExerciseVideoRental.Inventory;

namespace ExerciseVideoRental
{
    internal class RentalStore
    {
        public RentalStore()
        {
        }
        public RentalStore(Movie[] movies)
        {
            for (int i = 0; i < movies.Length; i++)
            {
                AvailableMovies.Add(movies[i]);
            }
        }
        public void Add(Movie movie) => AvailableMovies.Add(movie);
        public void Remove(Movie movie) => AvailableMovies.Remove(movie);
        public IEnumerable<Movie> List()
        {
            return AvailableMovies.ToList();
        }
        public List<Movie> ListAvailable()
        {
            return AvailableMovies;
        }
        ///<summary>Rents the movie, removes it from AvailableMovies and puts it into RentedMovies until returned.</summary>
        public static int getLastId()
        {
            return AvailableMovies.Count != 0 ? AvailableMovies.Last().Id : 0;
        }
        public void Start(Customer user)
        {
            bool stop = false;
            Console.WriteLine("Welcome to out video rental store!");
            do
            {
                Console.WriteLine("What would you like to do: \nRent a movie(r)\nReturn movies(b)\nQuit(q): ");
                string? answer = Console.ReadLine();
                if (answer == "q") break;
                else if(answer == "r") Rent(user);
                else if(answer == "b") Return(user);
                else if(answer == "") continue;
                
            } while (!stop);
            Console.WriteLine("Have a nice day!");
        }
        public void Return(Customer user)
        {
            if (user == null) return;
            if (user.RentedMovies.Count == 0) { Console.WriteLine("You don't have any movies rented."); return; }
            bool stop = false;
            Dictionary<Movie, int> receiptList = new Dictionary<Movie, int>();
            Console.WriteLine("What movie would you like to return?\nAvailable movies: ");
            //will loop through the menu until the user quits or there are no available movies to return
            do
            {
                foreach (KeyValuePair<Movie, int> pair in user.RentedMovies)
                {
                    if (receiptList.ContainsKey(pair.Key)) continue;
                    Console.Write(pair.Key.Id+ 1 + ": ");
                    Console.WriteLine(pair.Key.Name);
                }
                if (user.RentedMovies.Count == 0) break;
                Console.WriteLine("Please enter the movie ID that you would like to return\n(or q for quit and a for all): ");
                string? answer = Console.ReadLine();
                if (answer == "q") break;
                else if (answer == "a")
                {
                    foreach (KeyValuePair<Movie, int> item in user.RentedMovies)
                    {
                        //gets days from promptDays and movie from the keyvaluepair
                        receiptList.Add(item.Key, promptDays(item.Key, item.Value));
                    }
                }
                else if (int.TryParse(answer, out int id))
                {
                    if(id < 1)
                    {
                        Console.WriteLine("Invalid Id!");
                        continue;
                    }
                    KeyValuePair<Movie, int> pair;
                    try { pair = user.RentedMovies.First(pair => pair.Key.Id == id - 1); }
                    catch (ArgumentNullException) { Console.WriteLine("Movie does not exist!"); continue; }
                    receiptList.Add(pair.Key, promptDays(pair.Key, pair.Value));
                }
            } while (!stop);
            if (receiptList.Count == 0) return;
            if (user.RentedMovies.Count == 0)
                Console.WriteLine("You've run out of rented movies! Here's your receipt: ");
            else
                Console.WriteLine("Great! Here's your receipt: ");
            printReturnReceipt(receiptList, user);
        }
        //will ask user how much days they've had the movie for and print a price
        public int promptDays(Movie movie, int days)
        {
            while(true)
            {
                Console.WriteLine("Returning: "+movie.Name);
                Console.WriteLine("For how long did you have the movie for?(in days): ");
                string? answer = Console.ReadLine();
                if (int.TryParse(answer, out int actualDays))
                {
                    //if returned late
                    if (days < actualDays)
                    {
                        Console.WriteLine("It looks like you've kept the film for too long!\nThe store will have to add additional fees on returning this movie!");
                        return actualDays - days;
                    }
                    else
                    {
                        Console.WriteLine("Great! It looks like you've returned the movie on time!");
                        return 0;
                    }
                }
            }
        }
        public void Rent(Customer user)
        {
            if (user == null) return;
            if (AvailableMovies.Count == 0)
            {
                Console.WriteLine("The store doesen't have any movies available at this moment! Please come again later!");
                return;
            }
            bool stop = false;
            Dictionary<Movie, int> receiptList = new Dictionary<Movie, int>();
            Console.WriteLine("What movie would you like to rent?\nAvailable movies: ");
            //will loop through the menu until the user quits or there are no available movies to rent
            do
            {
                foreach (Movie movie in AvailableMovies)
                {
                    Console.Write(movie.Id + 1 + ": ");
                    Console.WriteLine(movie.Name);
                }
                if (AvailableMovies.Count == 0) break;
                Console.WriteLine("Please enter the movie ID(or q to quit): ");
                string? answer = Console.ReadLine();
                if (answer == "q") break;
                if (int.TryParse(answer, out int id))
                {
                    Movie movie;
                    if (id < 1)
                    {
                        Console.WriteLine("Invalid Id!");
                        continue;
                    }
                    try { movie = AvailableMovies.First(pair => pair.Id == id - 1); }
                    catch (ArgumentNullException) { Console.WriteLine("Movie does not exist!"); continue; }
                     movie = AvailableMovies.Single(x => x.Id == id - 1);
                    Console.WriteLine($"Rent movie {movie.Name} for how many days?: ");
                    int days;
                    while (!int.TryParse(Console.ReadLine(), out days)) { }
                    handleRent(movie, days, user);
                    receiptList.Add(movie, days);
                    Console.WriteLine($"Great! Rented {movie.Name} for {days} days!");
                }
            } while (!stop);
            if(receiptList.Count == 0) return;
            if(AvailableMovies.Count == 0)
                Console.WriteLine("Sorry! We've run out of movies. Here's your receipt: ");
            else
                Console.WriteLine("Great! Here's your receipt: ");
            printBuyReceipt(receiptList, user);
        }
        //assigns bonus point depending on the movie type
        void handleRent(Movie movie, int days, Customer user)
        {
            if (movie.Type == MovieType.New_Release)
                user.BonusPoints += 2;
            else
                user.BonusPoints += 1;
            AvailableMovies.Remove(movie);
            RentedMovies.Add(movie, days);
        }
        void printBuyReceipt(Dictionary<Movie, int> movies, Customer user)
        {
            float total = 0;
            foreach (KeyValuePair<Movie, int> movie in movies)
            {
                float movieTotal = getCost(movie.Key, movie.Value);
                Console.WriteLine($"{movie.Key.Name}({movie.Key.Type}) {movie.Value} days {movieTotal} EUR");
                total += movieTotal;
            }
            Console.WriteLine($"Total price: {total} EUR");
            RentedMovies.ToList().ForEach(movie => user.RentedMovies.Add(movie.Key, movie.Value));
        }
        void printReturnReceipt(Dictionary<Movie, int> movies, Customer user)
        {
            float total = 0;
            foreach (KeyValuePair<Movie, int> movie in movies)
            {
                if (movie.Value == 0) continue;
                float movieTotal = getLateCost(movie.Key, movie.Value);
                Console.WriteLine($"{movie.Key.Name}({movie.Key.Type}) {movie.Value} extra days {movieTotal} EUR");
                total += movieTotal;
                user.RentedMovies.Remove(movie.Key);
                AvailableMovies.Add(movie.Key);
            }
            Console.WriteLine($"Total price: {total} EUR");
        }
        float getLateCost(Movie movie, int daysOver)
        {
            switch (movie.Type)
            {
                case MovieType.New_Release:
                    return movie.premiumPrice * daysOver;
                default:
                    return movie.basicPrice * daysOver;
            }
        }
        float getCost(Movie movie, int days)
        {
            switch (movie.Type)
            {
                case MovieType.New_Release:
                    return days * movie.price;
                case MovieType.Regular_Rental:
                    if(days > 3)
                    {
                        float price = movie.price;
                        return price + (movie.price * (days - 3));
                    }
                    return movie.price;
                case MovieType.Old_Film:
                    if (days > 5)
                    {
                        float price = movie.price;
                        return price + (movie.price * (days - 5));
                    }
                    return movie.price;
                default:
                    return 0f;
            }
        }
    }
}
