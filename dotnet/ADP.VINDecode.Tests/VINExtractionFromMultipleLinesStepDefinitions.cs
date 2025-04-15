using Reqnroll;
using ShiftSoftware.ADP.VINDecode;
using Xunit.Abstractions;

namespace ADP.VINDecode.Tests
{
    [Binding]
    public class VINExtractionFromMultipleLinesStepDefinitions
    {
        List<string>? listOfVins;
        HashSet<VIN>? extractedFromlistOfVins;
        HashSet<VIN>? extractedFromText;
        string? multiLineString;


        [Given("multiple lines")]
        public void GivenMultipleLines(DataTable dataTable)
        {
            listOfVins = dataTable.Rows!.Select(x => x.Value()).ToList()!;
        }

        [When("I extract VINs from the lines")]
        public void WhenIExtractVINsFromTheLines()
        {
            extractedFromlistOfVins = ShiftSoftware.ADP.VINDecode.VINExtractor.FindVINs(
                listOfVins.ToArray()
            );
        }

        [Then("I should find a single VIN {string}")]
        public void ThenIShouldFindASingleVIN(string p0)
        {
            Assert.Equal(p0, extractedFromlistOfVins.FirstOrDefault()?.ToString());
        }

        [Given("a multi-line string")]
        public void GivenAMulti_LineString(string multilineText)
        {
            this.multiLineString = multilineText;
        }

        [When("I extract VINs from the string")]
        public void WhenIExtractVINsFromTheString()
        {
            extractedFromText = ShiftSoftware.ADP.VINDecode.VINExtractor.FindVINs(
                multiLineString
            );
        }

        [Then("I should also find a single VIN {string}")]
        public void ThenIShouldAlsoFindASingleVIN(string p0)
        {
            Assert.Equal(p0, extractedFromText.FirstOrDefault()?.ToString());
        }
    }
}
