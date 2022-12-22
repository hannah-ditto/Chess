using System.Collections.Generic;
using ChessCore.Attributes;
using ChessCore.Extensions;
using ChessCore.Models;
using ChessCore.Models.Pieces;
using FluentAssertions;
using Xunit;

namespace Chess.UnitTests.Extensions;

public class DictionaryExtensionTests : TestBase
{
    [Fact]
    public void Should_not_add_invalid_square_positions()
    {
        // Arrange
        var movesDict = new Dictionary<string, MoveType>();
        
        // Act
        movesDict.AddIfValid("m32", MoveType.Regular, Color.Light);

        // Assert
        movesDict.Should().BeEmpty();
    }
    
    [Fact]
    public void Should_add_valid_square_positions()
    {
        // Arrange
        var movesDict = new Dictionary<string, MoveType>();
        
        // Act
        movesDict.AddIfValid("a3", MoveType.Regular, Color.Light);

        // Assert
        movesDict.Should().ContainKey("a3");
    }
    
    [Fact]
    public void Should_allow_capture_when_enemy_pawn_occupies_target_square()
    {
        // Arrange
        var movesDict = new Dictionary<string, MoveType>();
        var enemyPawn = new Pawn(Color.Dark, "a3");
        Board.PiecePositions.Add("a3", enemyPawn);
        
        // Act
        movesDict.AddIfValid("a3", MoveType.Capture, Color.Light);

        // Assert
        movesDict.Should().ContainKey("a3");
    }
    
    [Fact]
    public void Should_not_allow_capture_when_friendly_piece_occupies_target_square()
    {
        // Arrange
        var movesDict = new Dictionary<string, MoveType>();
        var enemyPawn = new Pawn(Color.Light, "a3");
        Board.PiecePositions.Add("a3", enemyPawn);
        
        // Act
        movesDict.AddIfValid("a3", MoveType.Capture, Color.Light);

        // Assert
        movesDict.Should().NotContainKey("a3");
    }
    
    [Fact]
    public void Should_not_allow_capture_when_no_piece_occupies_target_square()
    {
        // Arrange
        var movesDict = new Dictionary<string, MoveType>();
        
        // Act
        movesDict.AddIfValid("a3", MoveType.Capture, Color.Light);

        // Assert
        movesDict.Should().NotContainKey("a3");
    }
}