namespace AutoApiGen.UnitTests;

public class RegexTests
{
    [Fact]
    public void Test1()
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
    public void Test2()
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
    public void Test3()
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

    [Fact]
    public void Test4()
    {
        //Arrange
        const string input = "{parameter}";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }

    [Fact]
    public void Test5()
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
    public void Test6()
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
    public void Test7()
    {
        //Arrange
        const string input = "{parameter:int=5}";
        
        //Act
        var match = Regexes.OptionalParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }

    [Fact]
    public void Test8()
    {
        //Arrange
        const string input = "{parameter}";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }

    [Fact]
    public void Test9()
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
    public void Test10()
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
    public void Test11()
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
    public void Test12()
    {
        //Arrange
        const string input = "{*parameter:int?}";
        
        //Act
        var match = Regexes.CatchAllParameterRoutePartRegex.Match(input);
        
        //Assert
        match.Success.Should().Be(false);
    }
}
