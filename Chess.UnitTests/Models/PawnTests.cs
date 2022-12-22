using AutoFixture;
using ChessCore.Attributes;
using ChessCore.Models;
using ChessCore.Models.Pieces;
using FluentAssertions;
using Xunit;

namespace Chess.UnitTests.Models;

public class PawnTests : TestBase
{
    private readonly IFixture _testFixture;
    
    public PawnTests()
    {
        _testFixture = new Fixture();
    }
    
    [Theory]
    [InlineData(Color.Light, "b3")]
    [InlineData(Color.Dark, "b1")]
    public void Should_move_one_square_only_on_regular_move(Color pawnColor, string expectedPosition)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, "b2")
            .With(x=>x.Color, pawnColor)
            .Create();
        
        // Act
        var position = pawn.ChooseMove(MoveType.Regular);

        // Assert
        position.Should().Be(expectedPosition);
    }
    
    [Theory]
    [InlineData(Color.Light, "b2",  "b4")]
    [InlineData(Color.Dark, "b7", "b5")]
    public void Should_move_two_squares_on_first_move(Color pawnColor, string startingPosition, string expectedPosition)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, startingPosition)
            .With(x => x.IsFirstMove, true)
            .With(x => x.IsEligibleForSpecial, false)
            .With(x=>x.Color, pawnColor)
            .Create();
        
        // Act
        var position = pawn.ChooseMove(MoveType.First);

        // Assert
        position.Should().Be(expectedPosition);
    }
    
    [Theory]
    [InlineData(Color.Light, true, "a3")]
    [InlineData(Color.Dark, true, "a1")]
    [InlineData(Color.Light, false, "c3")]
    [InlineData(Color.Dark, false, "c1")]
    public void Should_move_one_square_and_one_file_on_capture(Color pawnColor, bool moveLeft, string expectedPosition)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, "b2")
            .With(x => x.IsEligibleForSpecial, false)
            .With(x => x.IsFirstMove, false)
            .With(x=>x.Color, pawnColor)
            .Create();

        // Place enemy pawn at capture destination
        var enemyPawnColor = pawnColor == Color.Light ? Color.Dark : Color.Light;
        var enemyPawn = _testFixture.Build<Pawn>()
            .With(x => x.Color, enemyPawnColor)
            .Create();
        Board.PiecePositions[expectedPosition] = enemyPawn;
        
        // Act
        var position = pawn.Capture(moveLeft);

        // Assert
        position.Should().Be(expectedPosition);
    }
    
    [Theory]
    [InlineData(Color.Light, "b5", "a6", "a7")]
    [InlineData(Color.Dark, "b4", "a3", "a2")]
    public void Should_move_one_square_and_one_file_on_special_move(Color pawnColor, string startingPosition, string expectedPosition, string enemyPawnStartingPosition)
    {
        // Arrange
        var pawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, startingPosition)
            .With(x=>x.Color, pawnColor)
            .Create();
        Board.PiecePositions[startingPosition] = pawn;
        
        var enemyPawnColor = pawnColor == Color.Light ? Color.Dark : Color.Light;
        var enemyPawn = _testFixture.Build<Pawn>()
            .With(x=>x.CurrentPosition, enemyPawnStartingPosition)
            .With(x => x.IsFirstMove, true)
            .With(x=>x.Color, enemyPawnColor)
            .Create();

        var enemyPawnPosition = enemyPawn.Move(true);
        Board.PiecePositions[enemyPawnPosition!] = enemyPawn;
        
        // Act
        var position = pawn.Move(false, true);

        // Assert
        position.Should().Be(expectedPosition);
    }
}