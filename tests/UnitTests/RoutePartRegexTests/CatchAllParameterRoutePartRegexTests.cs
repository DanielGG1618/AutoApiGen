using AutoApiGen;

namespace UnitTests.RoutePartRegexTests;

public class CatchAllParameterRoutePartRegexTests
{
    [Fact]
    public void ShouldNotMatch_WhenInputDoesNotContainCatchAllIndicator()
    {
        //Arrange
        const string input = "{parameter}";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }

    [Fact]
    public void ShouldMatchName_WhenInputContainsNameWithCatchAllIndicator()
    {
        //Arrange
        const string input = "{*parameter}";
        const string expectedName = "parameter";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;

        //Assert
        name.Should().Be(expectedName);
        type.Should().BeNullOrEmpty();
        defaultValue.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldMatchNameAndType_WhenInputContainsNameAndTypeWithCatchAllIndicator()
    {
        //Arrange 
        const string input = "{*parameter:int}";
        const string expectedName = "parameter";
        const string expectedType = "int";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;

        //Assert
        name.Should().Be(expectedName);
        type.Should().Be(expectedType);
        defaultValue.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldMatchNameTypeAndDefaultValue_WhenInputContainsNameTypeAndDefaultValueWithCatchAllIndicator()
    {
        const string input = "{*parameter:int=5}";
        const string expectedName = "parameter";
        const string expectedType = "int";
        const string expectedDefault = "5";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;

        //Assert
        name.Should().Be(expectedName);
        type.Should().Be(expectedType);
        defaultValue.Should().Be(expectedDefault);
    }

    [Fact]
    public void ShouldNotMatch_WhenInputContainsOptionalIndicatorWithCatchAll()
    {
        //Arrange
        const string input = "{*parameter:int?}";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }
}