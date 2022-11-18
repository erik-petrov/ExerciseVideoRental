using System.Formats.Asn1;
using System.Globalization;
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
            Console.WriteLine("Welcome to out video rental store!");
            do
            {
                Console.WriteLine("What would you like to do: \nRent a movie(r)\nReturn movies(b)\nManage store inventory(m)\nGet user info(i)\nQuit(q): ");
                string? answer = Console.ReadLine();
                switch (answer)
                {
                    case "q":
                        return;
                    case "r":
                        Rent(user);
                        break;
                    case "b":
                        Return(user);
                        break;
                    case "m":
                        Manage();
                        break;
                    case "i":
                        user.DumpInfo();
                        break;
                    default:
                        continue;
                }
            } while (true);
            Console.WriteLine("Have a nice day!");
        }
        //will ask user how much days they've had the movie for and print a price
        public void Manage()
        {
            do
            {
                Console.WriteLine("What would you like to do: \nAdd a movie(a)\nManage a specific movie(m)\nList all movies(l)\nList all available movies(la)\nRemove all movies(removeAll)");
                string answer = Console.ReadLine();
                switch (answer)
                {
                    case "a":
                        AddMovie();
                        break;
                    case "m":
                        MovieManage();
                        break;
                    case "l":
                        printMoviesWithIds(ListMovies());
                        break;
                    case "la":
                        printMoviesWithIds(AvailableMovies);
                        break;
                    case "removeAll":
                        Console.WriteLine("Are you sure you want to do this?(y)");
                        string dec = Console.ReadLine();
                        if(dec == "y")
                        {
                            ClearMovies();
                        }
                        break;
                    default:
                        break;
                }

            } while (true);
        }

        public void MovieManage()
        {
            Movie movie = null;

            Console.WriteLine("Please select a movie to manage: ");
            printMoviesWithIds(ListMovies());

            int id;
            while(!int.TryParse(Console.ReadLine(), out id) && movie != null)
            {
                movie = GetMovieById(id);
            }

            do
            {
                Console.WriteLine("What would you like to do: \nChange type(t)\nRemove the movie(r)\nQuit(q)");
                string decision = Console.ReadLine();
                switch (decision)
                {
                    case "t":
                        handleTypeChange(movie);
                        break;
                    case "r":
                        RemoveMovie(movie);
                        break;
                    case "q":
                        return;
                }
            } while (true);
        }
        void handleTypeChange(Movie movie)
        {
            MovieType? type = null;
            do
            {
                Console.WriteLine("Please choose the new movie type: \nNew release(n)\nRegular rental(r)\nOld movie(o)\n");
                string newType = Console.ReadLine();
                switch (newType)
                {
                    case "r":
                        type = MovieType.Regular_Rental;
                        break;
                    case "o":
                        type = MovieType.Old_Film;
                        break;
                    case "n":
                        type = MovieType.New_Release;
                        break;
                }
            } while (type == null);
            movie.ChangeMovieType((MovieType)type);
        }
        public void AddMovie()
        {
            Console.WriteLine("Please enter the movie name: ");
            string name = Console.ReadLine();
            while (name == "" || name == null)
            {
                Console.WriteLine("Name can't be empty!");
                name = Console.ReadLine();
            }

            MovieType? type = null;

            while(type == null)
            {
                Console.WriteLine("Now please choose the release type \nNew release(n)\nRegular rental(r)\nOld movie(o)");
                string typeStr = Console.ReadLine();

                switch (typeStr)
                {
                    case "r":
                        type = MovieType.Regular_Rental;
                        break;
                    case "o":
                        type = MovieType.Old_Film;
                        break;
                    case "n":
                        type = MovieType.New_Release;
                        break;
                }
            }
            Inventory.AddMovie(new Movie(name, (MovieType)type));
        }
        public void Return(Customer user)
        {
            if (user == null)
            {
                return;
            }

            if (user.RentedMovies.Count == 0)
            {
                Console.WriteLine("It seems that you have no movies rented!");
                return;
            }

            Dictionary<Movie, int> receiptList = new Dictionary<Movie, int>();

            Console.WriteLine("What movie would you like to return?\nAvailable movies: ");

            do
            {
                printMoviesWithIds(user.RentedMovies.Keys.ToList());

                if (user.RentedMovies.Count == 0)
                {
                    break;
                }

                Console.WriteLine("Please enter the movie ID(or q to quit; a for all): ");
                int movieId;
                string strMovieId = Console.ReadLine();
                while (!int.TryParse(strMovieId, out movieId) || strMovieId == "a")
                {
                    strMovieId = Console.ReadLine();
                }

                Console.WriteLine($"Rented for how many days?: ");
                int days;
                while (!int.TryParse(Console.ReadLine(), out days)) { }

                if(strMovieId == "a")
                {
                    foreach (Movie item in user.RentedMovies.Keys)
                    {
                        receiptList.Add(item, days);
                    }
                    break;
                }
            }
            while (true);

            if (receiptList.Count == 0)
            {
                return;
            }

            Console.WriteLine("Great! Here's your receipt: ");
            printBuyReceipt(receiptList, user);
        }
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
            if (user == null)
            {
                return;
            }

            if (AvailableMovies.Count == 0)
            {
                Console.WriteLine("The store doesen't have any movies available at this moment! Please come again later!");
                return;
            }

            if(user.BonusPoints >= 25)
            {
                Console.WriteLine("Great news! You have 25 bonus points and rent a New release movie for 1 day free of charge!");
            }

            Dictionary<Movie, int> receiptList = new Dictionary<Movie, int>();

            Console.WriteLine("What movie would you like to rent?\nAvailable movies: ");

            do
            {
                bool payWithBonus = false;

                printMoviesWithIds(AvailableMovies);

                if (AvailableMovies.Count == 0)
                { 
                    break; 
                }

                Console.WriteLine("Please enter the movie ID(or q to quit): ");
                int movieId;
                while (!int.TryParse(Console.ReadLine(), out movieId)) { }

                Console.WriteLine($"Rent for how many days?: ");
                int days;
                while (!int.TryParse(Console.ReadLine(), out days)) { }

                if(days == 1 && user.BonusPoints >= 25)
                {
                    Console.WriteLine("Would you like to pay with bonus points? y/n");
                    string ans = Console.ReadLine();
                    if(ans == "y")
                    {
                        payWithBonus = true;   
                    }
                }

                Movie movie;
                try
                {
                    movie = handleRent(movieId, days, user, payWithBonus);
                    if (movie == null)
                    {
                        continue;
                    }
                    receiptList.Add(movie, days);
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Movie not found.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            while (true);

            if (receiptList.Count == 0)
            {
                return;
            }

            if (AvailableMovies.Count == 0)
            {
                Console.WriteLine("Sorry! We've run out of movies. Here's your receipt: ");
            }
            else
            {
                Console.WriteLine("Great! Here's your receipt: ");
                printBuyReceipt(receiptList, user);
            }
        }
        //assigns bonus point depending on the movie type
        Movie handleRent(int movieId, int days, Customer user, bool bonus)
        {
            if (movieId < 1)
            {
                throw new Exception("Invalid ID!");
            }

            Movie movie = AvailableMovies.First(pair => pair.Id == movieId - 1);

            if (bonus && movie.Type == MovieType.New_Release)
            {
                movie.paidWithBonus = true;
                AvailableMovies.Remove(movie);
                RentedMovies.Add(movie, days);
                return movie;
            }

            if (movie.Type == MovieType.New_Release)
            {
                user.BonusPoints += 2;
            }
            else
            {
                user.BonusPoints += 1;
            }

            AvailableMovies.Remove(movie);
            RentedMovies.Add(movie, days);
            return movie;
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
                if (movie.Value == 0)
                {
                    continue;
                }

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
                    return Movie.premiumPrice * daysOver;
                default:
                    return Movie.basicPrice * daysOver;
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
        void printMoviesWithIds(List<Movie> movies)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                Console.Write(movies[i].Id + 1 + ": ");
                Console.WriteLine(movies[i].Name);
            }
        }
    }
}
