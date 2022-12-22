using AutoFixture;
using ChessCore.Attributes;
using ChessCore.Models;
using ChessCore.Models.Pieces;
using FluentAssertions;
using Xunit;

namespace Chess.UnitTests.Models;

public class PieceTests : TestBase
{
    private readonly IFixture _testFixture;
    
    public PieceTests()
    {
        _testFixture = new Fixture();
    }
    
    [Theory]
    [InlineData("m8")]
    [InlineData("g8")]
    [InlineData("h8")]
    public void Should_only_allow_moves_to_a_legal_board_square(string startingDestination)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, startingDestination)
            .With(x=>x.Color, Color.Light)
            .Create();
        
        // Act
        var targetDestination = pawn.ChooseMove(MoveType.Regular);

        // Assert
        targetDestination.Should().BeNull();
    }
    
    [Theory]
    [InlineData("a3", true)]
    [InlineData("c3", false)]
    public void Should_return_captured_piece_on_capture(string targetPiecePosition, bool moveLeft)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, "b2")
            .With(x=>x.IsEligibleForSpecial, false)
            .With(x=>x.Color, Color.Light)
            .Create();
        
        // Place enemy pawn at capture destination
        var enemyPawn = _testFixture.Build<Pawn>()
            .With(x => x.Color, Color.Dark)
            .Create();
        Board.PiecePositions[targetPiecePosition] = enemyPawn;

        // Act
        pawn.Capture(moveLeft);

        // Assert
        Board.CapturedPieces?.Value.Contains(enemyPawn).Should().BeTrue();
    }
}