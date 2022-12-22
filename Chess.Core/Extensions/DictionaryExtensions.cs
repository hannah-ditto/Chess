using ChessCore.Attributes;
using ChessCore.Models;

namespace ChessCore.Extensions;

public static class DictionaryExtensions
{
    public static void AddIfValid(this IDictionary<string, MoveType> dictionary, string key, MoveType value, Color pieceColor)
    {
        // Not valid if off the chessboard
        if (!Board.IsLegalSquare(key))
            return;

        // Capture moves would land on a friendly piece
        if (value.HasFlag(MoveType.Capture) && Board.PiecePositions.TryGetValue(key, out var targetPiece) && targetPiece?.Color == pieceColor)
            return;
        
        // Capture moves would land on no piece
        if (value.HasFlag(MoveType.Capture) && !Board.PiecePositions.TryGetValue(key, out targetPiece))
            return;
        
        dictionary.TryAdd(key, value);
        
        // Special moves break other validation rules so we go ahead and add these
        // if they are on legal squares and not already present
        if (value.HasFlag(MoveType.Special))
            dictionary.TryAdd(key, value);
    }  
}