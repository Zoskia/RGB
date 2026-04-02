using FluentAssertions;
using Moq;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories.Interfaces;
using RedGreenBlue.Services;

namespace RedGreenBlue.UnitTests.Services;

public class GridServiceTests
{
    [Fact]
    public async Task GetCellsAsync_ShouldReturnCellsFromRepository()
    {
        var expected = new List<Cell>
        {
            new() { Q = 1, R = 2, TeamColor = TeamColor.Green, HexColor = "#00ff00" }
        };

        var repository = new Mock<IGridRepository>();
        repository
            .Setup(r => r.GetCellsAsync(TeamColor.Green))
            .ReturnsAsync(expected);

        var sut = new GridService(repository.Object);

        var result = await sut.GetCellsAsync(TeamColor.Green);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task UpdateCellColorAsync_ShouldUpdate_WhenColorMatchesTeamSpectrum()
    {
        var dto = new UpdateCellColorDto
        {
            Q = 5,
            R = 4,
            TeamColor = TeamColor.Red,
            HexColor = "#ff2200"
        };

        var repository = new Mock<IGridRepository>();
        repository
            .Setup(r => r.UpdateCellColorAsync(dto))
            .ReturnsAsync(true);

        var sut = new GridService(repository.Object);

        var result = await sut.UpdateCellColorAsync(dto);

        result.Should().BeTrue();
        repository.Verify(r => r.UpdateCellColorAsync(dto), Times.Once);
    }

    [Fact]
    public async Task UpdateCellColorAsync_ShouldThrow_WhenColorIsOutsideTeamSpectrum()
    {
        var dto = new UpdateCellColorDto
        {
            Q = 0,
            R = 0,
            TeamColor = TeamColor.Red,
            HexColor = "#0000ff"
        };

        var repository = new Mock<IGridRepository>();
        var sut = new GridService(repository.Object);

        var act = async () => await sut.UpdateCellColorAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Wrong color spectrum");
        repository.Verify(r => r.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCellColorAsync_ShouldThrow_WhenTeamIsUnsupported()
    {
        var dto = new UpdateCellColorDto
        {
            Q = 0,
            R = 0,
            TeamColor = (TeamColor)99,
            HexColor = "#ff0000"
        };

        var repository = new Mock<IGridRepository>();
        var sut = new GridService(repository.Object);

        var act = async () => await sut.UpdateCellColorAsync(dto);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        repository.Verify(r => r.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>()), Times.Never);
    }
}
