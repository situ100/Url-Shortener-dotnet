using Infrastructure.Services;
using Xunit;


namespace UnitTests;

public class SlugServiceTests
{
    [Fact]
    public void Generate_ReturnsStringOfCorrectLength()
    {
        //Arange
        var service = new SlugService();

        //Act
        var slug = service.GenerateSlug(7);

        //Assert
        Assert.Equal(7, slug.Length);
    }

    [Fact]
    public void Generate_RetunsDiffrentValues()
    {
        //Arange
        var service = new SlugService();

        //Act
        var slug1 = service.GenerateSlug(7);
        var slug2 = service.GenerateSlug(7);

        //Assert
        Assert.NotEqual(slug1, slug2);
    }

}