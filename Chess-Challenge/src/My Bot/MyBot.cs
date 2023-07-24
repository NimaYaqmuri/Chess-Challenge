using System;
using System.Linq;
using System.Collections.Generic;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    private const int MaxDepth = 3;
    private static readonly Random Random = new Random();
    private static readonly Dictionary<PieceType, int> PieceValues = new Dictionary<PieceType, int>
    {
        [PieceType.Pawn] = 1,
        [PieceType.Knight] = 3,
        [PieceType.Bishop] = 3,
        [PieceType.Rook] = 5,
        [PieceType.Queen] = 9,
    };

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = default;
        double bestScore = double.NegativeInfinity;

        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            double score = -AlphaBeta(-1, board, MaxDepth, double.NegativeInfinity, double.PositiveInfinity);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private double AlphaBeta(int player, Board board, int depth, double alpha, double beta)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return EvaluateBoard(board, player);
        }

        double bestScore = double.NegativeInfinity;

        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            bestScore = Math.Max(bestScore, -AlphaBeta(-player, board, depth - 1, -beta, -alpha));
            board.UndoMove(move);

            alpha = Math.Max(alpha, bestScore);
            if (beta <= alpha)
            {
                break;
            }
        }

        return bestScore;
    }

    private double EvaluateBoard(Board board, int player)
    {
        double score = 0;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            foreach (Piece piece in pieceList)
            {
                int pieceValue = PieceValues.ContainsKey(piece.PieceType) ? PieceValues[piece.PieceType] : 0;
                score += pieceValue * (piece.IsWhite == board.IsWhiteToMove ? 1 : -1);
            }
        }

        return player * score + Random.NextDouble() - 0.5;
    }

}
