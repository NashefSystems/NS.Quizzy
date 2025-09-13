using OtpNet;

namespace NS.Quizzy.TestUnits
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateRandomOTPKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            var res = Base32Encoding.ToString(key);
            Console.WriteLine($"Result: '{res}'");
            Assert.That(res, Is.Not.Empty);
        }
    }
}
