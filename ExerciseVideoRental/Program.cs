using ExerciseVideoRental;

RentalStore rs = new RentalStore(new Movie[]
{
    new Movie(0, "Matrix 11", MovieType.New_Release),
    new Movie(1, "Spider man", MovieType.Regular_Rental),
    new Movie(2, "Spider man 2", MovieType. Regular_Rental),
    new Movie(3, "Out of Africa", MovieType.Old_Film),
});
Customer me = new Customer("Erik", "afhsudf");
//testing purposes
me.BonusPoints = 25;
rs.Start(me);