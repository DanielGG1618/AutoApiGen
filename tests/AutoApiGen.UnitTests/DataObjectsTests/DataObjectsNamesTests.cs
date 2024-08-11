using AutoApiGen.DataObjects;

namespace AutoApiGen.UnitTests.DataObjectsTests;

public class DataObjectsNamesTests
{
    [Fact]
    public void TemplateDataImplementationsShouldEndWithData()
    {
        // Arrange
        // Get all classes that implement ITemplateData from AutoApiGen assembly
        var templateDataImplementations =
            typeof(ITemplateData).Assembly.GetTypes()
                .Where(type =>
                    typeof(ITemplateData).IsAssignableFrom(type)
                    && type is { IsInterface: false, IsAbstract: false }
                );
        
        // Assert
        templateDataImplementations.Should().AllSatisfy(type => type.Name.Should().EndWith("Data"));
    }
}
