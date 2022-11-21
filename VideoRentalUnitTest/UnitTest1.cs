using Moq;
using System.Text;
using ExerciseVideoRental;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace VideoRentalUnitTest
{
    public class UnitTest1
    {
        StringBuilder _ConsoleOutput;
        Mock<TextReader> _ConsoleInput;

        [SetUp]
        public void Setup()
        {
            _ConsoleOutput = new StringBuilder();
            var consoleOutputWriter = new StringWriter(_ConsoleOutput);
            _ConsoleInput = new Mock<TextReader>();
            Console.SetOut(consoleOutputWriter);
            Console.SetIn(_ConsoleInput.Object);
        }

        [Test]
        public void RentMovies()
        {
            SetupUserResponses("r", "1", "1", "n", "2", "5", "3", "2", "4", "7", "q");
            var expected = "Total price: 25 EUR";

            var output = RunAndGetOutput();

            Assert.AreEqual(expected, output[^4]);
        }

        [Test]
        public void BuyWithBonusPoints()
        {
            SetupUserResponses("r", "1", "1", "y", "q", "q");
            var expected = "Total price: 0 EUR";

            var output = RunAndGetOutput();

            Assert.AreEqual(expected, output[^4]);
        }

        [Test]
        public void ReturnMovies()
        {
            SetupUserResponses("r", "1", "1", "n", "2", "5", "q", "b", "1", "3", "2", "6", "q");
            var expected = "Total late charge: 11 EUR";

            var output = RunAndGetOutput();

            Assert.AreEqual(expected, output[^3]);
        }

        [TearDown]
        public void TearDown()
        {
            _ConsoleInput = null;
            _ConsoleOutput = null;
        }

        private string[] RunAndGetOutput()
        {
            RentalStore rs = new RentalStore(new Movie[]
                 {
                        new Movie(0, "Matrix 11", MovieType.New_Release),
                        new Movie(1, "Spider man", MovieType.Regular_Rental),
                        new Movie(2, "Spider man 2", MovieType. Regular_Rental),
                        new Movie(3, "Out of Africa", MovieType.Old_Film),
                 });
            Customer me = new Customer("Erik", "afhsudf");
            me.BonusPoints = 25;
            rs.Start(me);
            return _ConsoleOutput.ToString().Split("\r\n");
        }

        MockSequence SetupUserResponses(params string[] userResponses)
        {
            var sequence = new MockSequence();
            foreach (var response in userResponses)
            {
                _ConsoleInput.InSequence(sequence).Setup(x => x.ReadLine()).Returns(response);
            }
            return sequence;
        }
    }
}