using ChessCore.Models;
using FluentAssertions;
using Xunit;

namespace Chess.UnitTests.Models;

public class BoardTests : TestBase
{
    [Fact]
    public void Should_have_64_squares()
    {
        // Arrange
        var files = Board.Files;
        
        // Act
        Board.SetupBoard();

        // Assert
        files.Length.Should().Be(8);
        files[0].Squares.Length.Should().Be(8);
    }

    [Fact]
    public void Should_label_files_a_through_h()
    {
        // Arrange
        var files = Board.Files;
        
        // Act
        Board.SetupBoard();

        // Assert
        files[0].Marker.Should().Be('a');
        files[7].Marker.Should().Be('h');
    }

    [Theory]
    [InlineData(Board.BackRowDark)]
    [InlineData(Board.FrontRowDark)]
    [InlineData(Board.BackRowLight)]
    [InlineData(Board.FrontRowLight)]
    public void Should_set_pieces_on_board(int row)
    {
        // Arrange
        var piecePositions = Board.PiecePositions;
        
        // Act
        Board.ResetPieces();

        // Assert
        foreach (var file in Board.Files)
        {
            var piecePosition = file.Marker.ToString() + row;
            piecePositions.TryGetValue(piecePosition, out var piece);
            piece.Should().NotBeNull();
        }
    }
    
    [Theory]
    [InlineData("a2", true)]
    [InlineData("m2", false)]
    [InlineData("g8", true)]
    [InlineData("g18", false)]
    public void Should_determine_whether_target_position_is_legal_square(string targetPosition, bool isLegal)
    {
        // Act
        var result = Board.IsLegalSquare(targetPosition);

        // Assert
        result.Should().Be(isLegal);
    }
    
    [Fact]
    public void Should_convert_position_to_file_and_square()
    {
        // Arrange
        var position = "a2";
        
        // Act
        var result = Board.ConvertPositionToFileAndSquare(position);

        // Assert
        result.file.Should().Be('a');
        result.square.Should().Be(2);
    }
}