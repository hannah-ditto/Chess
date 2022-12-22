using ChessCore.Attributes;
using ChessCore.Models.Pieces;

namespace ChessCore.Models;

public static class Board
{
    public static IDictionary<string, Piece?> PiecePositions { get; } = new Dictionary<string, Piece?>();
    public static Lazy<List<Piece>>? CapturedPieces { get; set; }
    public static readonly File[] Files = new File[8];

    public const int BackRowDark = 8;
    public const int FrontRowDark = 7;
    public const int BackRowLight = 1;
    public const int FrontRowLight = 2;

    static Board()
    {
        SetupBoard();
    }

    public static void SetupBoard()
    {
        var positionMarker = 'a';
        
        for (var i = 0; i < 8; i++)
        {
            Files[i] = new File(positionMarker);
            positionMarker++;
        }
    }

    public static void ResetPieces()
    {
        PiecePositions.Clear();

        SetPlayerPieces(Color.Dark);
        SetPlayerPieces(Color.Light);
    }
    
    public static bool IsLegalSquare(string pieceDestination)
    {
        var (file, square) = ConvertPositionToFileAndSquare(pieceDestination);

        if (Files.All(x => x.Marker != file)) 
            return false;

        return square is >= 0 and <= 8;
    }
    
    public static (char file, int square) ConvertPositionToFileAndSquare(string startingPosition)
    {
        var file = startingPosition.ToCharArray()[0];
        var square = int.Parse(startingPosition[1..]);
        return (file, square);
    }

    private static void SetPlayerPieces(Color color)
    {
        SetPawns(color);
        SetRooks(color);
        SetKnights(color);
        SetBishops(color);
        SetQueen(color);
        SetKing(color);
    }

    private static void SetQueen(Color color)
    {
        var row = color == Color.Dark ? BackRowDark : BackRowLight;
        var position = "d" + row;
        PiecePositions.Add(position, new Pawn(color, position));
    }
    
    private static void SetKing(Color color)
    {
        var row = color == Color.Dark ? BackRowDark : BackRowLight;
        var position = "e" + row;
        PiecePositions.Add(position, new Pawn(color, position));
    }

    private static void SetKnights(Color color, bool leftSide = true)
    {
        var row = color == Color.Dark ? BackRowDark : BackRowLight;
        var position = leftSide ? "b" + row : "g" + row;
        PiecePositions.Add(position, new Pawn(color, position));
        
        while (leftSide)
        {
            leftSide = false;
            SetKnights(color, leftSide);
        }
    }

    private static void SetBishops(Color color, bool leftSide = true)
    {
        var row = color == Color.Dark ? BackRowDark : BackRowLight;
        var position = leftSide ? "c" + row : "f" + row;
        PiecePositions.Add(position, new Pawn(color, position));
        
        while (leftSide)
        {
            leftSide = false;
            SetBishops(color, leftSide);
        }
    }

    private static void SetRooks(Color color, bool leftSide = true)
    {
        var row = color == Color.Dark ? BackRowDark : BackRowLight;
        var position = leftSide ? "a" + row : "h" + row;
        PiecePositions.Add(position, new Pawn(color, position));
        
        while (leftSide)
        {
            leftSide = false;
            SetRooks(color, leftSide);
        }
    }

    private static void SetPawns(Color color)
    {
        int row = color == Color.Dark ? FrontRowDark : FrontRowLight;
        
        foreach (var file in Files)
        {
            var position = file.Marker.ToString() + row;
            PiecePositions.Add(position, new Pawn(color, position));
        }
    }
}

public class File
{
    public readonly char Marker;
    public readonly int[] Squares = new int[8];

    internal File(char marker)
    {
        Marker = marker;
        for (var i = 0; i < 8; i++)
        {
            Squares[i] = i+1;
        }
    }
}