using ChessCore.Attributes;

namespace ChessCore.Models.Pieces;

public abstract class Piece
{
    public readonly Color Color;

    public bool IsFirstMove;
    public int PointValue { get; }
    public string CurrentPosition { get; set; }


    protected Piece(Color color, string currentPosition, int pointValue)
    {
        Color = color;
        CurrentPosition = currentPosition;
        PointValue = pointValue;
    }

    public bool IsEligibleForSpecial { get; set; }
    
    protected char ChangeFile(char fileMarker, int numFiles, bool moveLeft)
    {
        for (var i = 0; i < numFiles; i++)
        {
            if (moveLeft)
                --fileMarker;
            else
            {
                ++fileMarker;
            }
        }
        return fileMarker;
    }

    protected string Move(Func<char, char> fileChange, Func<int, int> squareChange)
    {
        var (file, square) = Board.ConvertPositionToFileAndSquare(CurrentPosition);
        
        var targetSquare = squareChange(square);
        var targetFile = fileChange(file);

        return targetFile.ToString() + targetSquare;
        
    }

    protected string Move(Func<int, int> squareChange)
    {
        var (file, square) = Board.ConvertPositionToFileAndSquare(CurrentPosition);
        
        var targetSquare = squareChange(square);

        return file.ToString() + targetSquare;
    }
}