using System;
using ChessCore.Models;

namespace Chess.UnitTests;

public class TestBase : IDisposable
{
    protected TestBase()
    {
        Board.PiecePositions.Clear();
    }
    
    private static void Dispose(bool disposing)
    {
        if (disposing)
        {
            Board.CapturedPieces?.Value.Clear();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}