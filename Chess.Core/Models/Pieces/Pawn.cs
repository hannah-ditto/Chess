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
        
        // Set up enemy pawn for En Passant if relevant

        var (file, square) = Board.ConvertPositionToFileAndSquare(targetPosition);
        var leftFile = (char) (file-1);
        var rightFile = (char) (file+1);
        
        var enemyPieceLeftPosition = leftFile.ToString() + square;
        var enemyPawnRightPosition = rightFile.ToString() + square;

        if (Board.PiecePositions.TryGetValue(enemyPieceLeftPosition, out var enemyPieceLeft))
        {
            if (enemyPieceLeft is Pawn enemyPawn)
            {
                enemyPawn.IsEligibleForSpecial = true;
                enemyPawn.SetSpecialMove(true);
            }
        }

        if (Board.PiecePositions.TryGetValue(enemyPawnRightPosition, out var enemyPieceRight))
        {
            if (enemyPieceRight is Pawn enemyPawn)
            {
                enemyPawn.IsEligibleForSpecial = true;
                enemyPawn.SetSpecialMove(true);
            }
        } 

        return targetPosition;
    }

    private string? ChooseCaptureMove(bool moveLeft)
    {
        return (from m in _validMoves.Value
                where (MoveType.Capture & m.Value) == m.Value
                select m.Key)
            .Select( m=>
            {
                var currentPiece = Board.ConvertPositionToFileAndSquare(CurrentPosition);
                var targetPosition = Board.ConvertPositionToFileAndSquare(m);
                    
                if (moveLeft && targetPosition.file < currentPiece.file)
                    return m;
                    
                if (!moveLeft && targetPosition.file > currentPiece.file)
                {
                    return m;
                }

                return null;
            })?
            .First(x=>x != null);
    }

    public string SetSpecialMove(bool moveLeft = false)
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
        
        return targetPosition;
    }


    private void CalculateAvailableTargetPositions(Color color)
    {
        var yAxisDirection = 1;

        if (color == Color.Dark)
            yAxisDirection = -1;

        if (IsFirstMove) 
            _validMoves.Value.AddIfValid(Move(y => y + 2 * yAxisDirection), MoveType.First & MoveType.Regular, Color);
        
        // Capture moves
        
        _validMoves.Value.AddIfValid(Move(x => --x, y => y + 1 * yAxisDirection), MoveType.Capture, Color);
        _validMoves.Value.AddIfValid(Move(x => ++x, y => y + 1 * yAxisDirection), MoveType.Capture, Color);
        _validMoves.Value.AddIfValid(Move(y => y + 1 * yAxisDirection), MoveType.Regular, Color);
    }
}
    