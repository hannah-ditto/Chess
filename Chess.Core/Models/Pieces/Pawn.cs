using ChessCore.Attributes;
using ChessCore.Extensions;

namespace ChessCore.Models.Pieces;

public class Pawn : Piece
{
    private readonly Lazy<Dictionary<string, MoveType>> _validMoves = new ();

    public Pawn(Color color, string piecePosition) : base(color, piecePosition, 1)
    {
        
    }

    public string Move(bool useFirstMove = false, bool useSpecialMove = false)
    {
        string targetPosition;

        if (useFirstMove && IsFirstMove)
            targetPosition = 
                ChooseMove(MoveType.First) 
                ?? throw new InvalidOperationException("No valid moves for this piece.");
        
        else if (useSpecialMove && IsEligibleForSpecial)
            targetPosition = 
                ChooseMove(MoveType.Special)
                ?? throw new InvalidOperationException("No valid moves for this piece.");
        else
            targetPosition = ChooseMove(MoveType.Regular) ?? throw new InvalidOperationException("No valid moves for this piece.");
        
        IsFirstMove = false;
        Board.PiecePositions[targetPosition] = this;
        return targetPosition;
    }

    public string Capture(bool moveLeft = false)
    {
        var targetPosition = ChooseMove(MoveType.Capture, moveLeft) ??
                             throw new InvalidOperationException("No valid capture moves for this piece.");
        
        Board.CapturedPieces?.Value.Add(Board.PiecePositions[targetPosition] ?? throw new InvalidOperationException("No piece to capture."));
        IsFirstMove = false;
        Board.PiecePositions[targetPosition] = this;
        return targetPosition;
    }

    public string? ChooseMove(MoveType moveType, bool moveLeft = true)
    {
        CalculateAvailableTargetPositions(Color);

        return moveType switch
        {
            MoveType.Capture => ChooseCaptureMove(moveLeft),
            MoveType.First => ChooseFirstMove(),
            MoveType.Special => ChooseSpecialMove(),
            MoveType.Regular =>
                (from m in _validMoves.Value
                    where m.Value == 0
                    select m.Key).FirstOrDefault(),
            _ => null
        };
    }

    private string? ChooseSpecialMove()
    {
        return (from m in _validMoves.Value
            where (MoveType.Special & m.Value) == m.Value
            select m.Key).FirstOrDefault();
    }

    private string? ChooseFirstMove()
    {
        var targetPosition = (from m in _validMoves.Value
            where (MoveType.First & m.Value) == m.Value
            select m.Key).FirstOrDefault();

        if (targetPosition == null)
            return null;
        
        CheckForEnPassant(targetPosition);

        return targetPosition;
    }

    private string? ChooseCaptureMove(bool moveLeft)
    {
        return (from m in _validMoves.Value
                where (MoveType.Capture & m.Value) == m.Value
                select m.Key)
            .FirstOrDefault(m =>
            {
                var (currentFile, _) = Board.ConvertPositionToFileAndSquare(CurrentPosition);
                var (moveFile, _) = Board.ConvertPositionToFileAndSquare(m);

                return moveLeft && moveFile < currentFile || !moveLeft && moveFile > currentFile;
            });
    }

    private void SetSpecialMove(bool moveLeft = false)
    {
        string targetPosition;
        
        var yAxisDirection = 1;

        if (Color == Color.Dark)
            yAxisDirection = -1;
        
        // En Passant
        // Capture a pawn which has made an initial move (two square advance) as if it had only advanced one square. 
        // To be enabled by enemy pawn upon making a qualifying move.
        
        if (moveLeft)
        {
            targetPosition = Move(x => --x, y => y + 1 * yAxisDirection);
            _validMoves.Value[targetPosition] = MoveType.Special & MoveType.Capture;
        }
        else
        {
            targetPosition = Move(x => ++x, y => y + 1 * yAxisDirection);
            _validMoves.Value[targetPosition] = MoveType.Special & MoveType.Capture;
        }
    }
    
    private static void CheckForEnPassant(string targetPosition)
    {
        var (file, square) = Board.ConvertPositionToFileAndSquare(targetPosition);

        EnableEnPassant(file, square, 1);
        EnableEnPassant(file, square, -1);
    }

    private static void EnableEnPassant(char file, int square, int xAxis)
    {
        var enemyFile = (char) (file - 1 * xAxis);
        var enemyPiecePosition = enemyFile.ToString() + square;

        if (!Board.PiecePositions.TryGetValue(enemyPiecePosition, out var enemyPiece) ||
            enemyPiece is not Pawn enemyPawn) return;
        
        enemyPawn.IsEligibleForSpecial = true;
        enemyPawn.SetSpecialMove(true);
    }

    private void CalculateAvailableTargetPositions(Color color)
    {
        var yAxis = 1;

        if (color == Color.Dark)
            yAxis = -1;

        if (IsFirstMove) 
            _validMoves.Value.AddIfValid(Move(y => y + 2 * yAxis), MoveType.First & MoveType.Regular, Color);
        
        // Capture moves
        
        _validMoves.Value.AddIfValid(Move(x => --x, y => y + 1 * yAxis), MoveType.Capture, Color);
        _validMoves.Value.AddIfValid(Move(x => ++x, y => y + 1 * yAxis), MoveType.Capture, Color);
        _validMoves.Value.AddIfValid(Move(y => y + 1 * yAxis), MoveType.Regular, Color);
    }
}
    