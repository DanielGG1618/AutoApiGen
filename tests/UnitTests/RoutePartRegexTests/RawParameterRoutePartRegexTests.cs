using AutoApiGen;

namespace UnitTests.RoutePartRegexTests;

public class RawParameterRoutePartRegexTests
{
    [Fact]
    public void ShouldMatchNameOnly_WhenInputContainsNameWithoutTypeOrDefaultValue()
    {
        //Arrange
        const string input = "{parameter}";
        const string expectedName = "parameter";
        
        //Act
        var match = Regexes.RawParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;

        //Assert
        name.Should().Be(expectedName);
        type.Should().BeNullOrEmpty();
        defaultValue.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldMatchNameAndType_WhenInputContainsNameAndType()
    {
        //Arrange
        const string input = "{parameter:int}";
        const string expectedName = "parameter";
        const string expectedType = "int";
        
        //Act
        var match = Regexes.RawParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;
        
        //Assert
        name.Should().Be(expectedName);
        type.Should().Be(expectedType);
        defaultValue.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldMatchNameAndDefaultValue_WhenInputContainsNameAndDefaultValue()
    {
        //Arrange
        const string input = "{parameter=5}";
        const string expectedName = "parameter";
        const string expectedDefault = "5";
        
        //Act
        var match = Regexes.RawParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;
        
        //Assert
        name.Should().Be(expectedName);
        type.Should().BeNullOrEmpty();
        defaultValue.Should().Be(expectedDefault);
    }
    
    [Fact]
    public void ShouldMatchNameTypeAndDefaultValue_WhenInputContainsNameTypeAndDefaultValue()
    {
        //Arrange
        const string input = "{parameter:int=5}";
        const string expectedName = "parameter";
        const string expectedType = "int";
        const string expectedDefault = "5";
        
        //Act
        var match = Regexes.RawParameterRoutePartRegex.Match(input);
        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var defaultValue = match.Groups["default"].Value;
        
        //Assert
        name.Should().Be(expectedName);
        type.Should().Be(expectedType);
        defaultValue.Should().Be(expectedDefault);
    }
}
