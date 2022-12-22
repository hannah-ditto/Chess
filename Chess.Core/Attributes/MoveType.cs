namespace ChessCore.Attributes;

[Flags]
public enum MoveType
{
    Regular = 0,
    Capture = 1,
    First = 2,
    Special = 4,
}