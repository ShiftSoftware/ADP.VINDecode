using Xunit.Abstractions;

namespace ADP.VINDecode.Tests
{
    public class VINTests
    {
        private readonly ITestOutputHelper output;
        public VINTests(ITestOutputHelper testOutputHelper)
        {
            output = testOutputHelper;
        }


        [Fact(DisplayName = "01. Equality")]
        public void Equality()
        {
            var vin1IsValid = VIN.TryParse("19XFB2F57EE209415", out VIN vin1);
            var vin2IsValid = VIN.TryParse("19xfB2F57Ee209415", out VIN vin2);
            var vin3IsValid = VIN.TryParse("19XFb2F57eE209415", out VIN vin3);

            Assert.True(vin1IsValid);
            Assert.True(vin2IsValid);
            Assert.True(vin3IsValid);

            Assert.Equal(vin1, vin2);
            Assert.Equal(vin1, vin3);
            Assert.Equal(vin2, vin3);

            Assert.Equal(vin1.GetHashCode(), vin2.GetHashCode());
            Assert.Equal(vin1.GetHashCode(), vin3.GetHashCode());
            Assert.Equal(vin2.GetHashCode(), vin3.GetHashCode());
        }

        [Fact(DisplayName = "02. Components")]
        public void Components()
        {
            VIN.TryParse("19XFB2F57EE209415", out VIN vin);
            
            Assert.Equal("19X", vin.WMI);
            Assert.Equal("FB2F57", vin.VDS);
            Assert.Equal("E", vin.CD);
            Assert.Equal("EE209415", vin.VIS);
        }
    }
}