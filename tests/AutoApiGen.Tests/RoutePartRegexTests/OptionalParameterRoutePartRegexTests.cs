namespace AutoApiGen.Tests.RoutePartRegexTests;

public class OptionalParameterRoutePartRegexTests
{
    [Fact]
    public void ShouldNotMatch_WhenInputDoesNotContainOptionalIndicator()
    {
        //Arrange
        const string input = "{parameter}";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }

    [Fact]
    public void ShouldMatchName_WhenInputContainsNameWithOptionalIndicator()
    {
        //Arrange
        const string input = "{parameter?}";
        const string expectedName = "parameter";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        
        //Assert
        name.Should().Be(expectedName);
        type.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldMatchNameAndType_WhenInputContainsNameAndTypeWithOptionalIndicator()
    {
        //Arrange
        const string input = "{parameter:int?}";
        const string expectedName = "parameter";
        const string expectedType = "int";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        
        //Assert
        name.Should().Be(expectedName);
        type.Should().Be(expectedType);
    }

    
    [Fact]
    public void ShouldNotMatch_WhenInputContainsDefaultValueWithOptionalIndicator()
    {
        //Arrange
        const string input = "{parameter=5}";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }
    
    [Fact]
    public void ShouldNotMatch_WhenInputContainsDefaultValueAndTypeWithOptionalIndicator()
    {
        //Arrange
        const string input = "{parameter:int=5}";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }
}