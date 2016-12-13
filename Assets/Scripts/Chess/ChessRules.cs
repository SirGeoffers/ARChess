using UnityEngine;
using System.Collections.Generic;

public class ChessRules : MonoBehaviour {

    public static ChessboardManager chessboardManager;

    public class Position {
        public int row, col;
        public Position(int row, int col) {
            this.row = row;
            this.col = col;
        }
    }

    public static List<Position> GetAvailableMoves(GameObject piece) {

        // List to store all available moves
        List<Position> availableMoves = new List<Position>();

        // Get all the properties that we need
        PieceProperties pieceProp = piece.GetComponent<PieceProperties>();
        int team = pieceProp.team;
        int row = pieceProp.row;
        int col = pieceProp.column;
        PieceProperties.Type type = pieceProp.type;

        if (PieceProperties.Type.Pawn == type) {

            int dir = 0;
            int dist = 1;
            if (team == 0) {
                dir = 1;
                if (row == 1) dist = 2; ;
            } else if (team == 1) {
                dir = -1;
                if (row == 6) dist = 2;
            }

            // Moving forwards
            GameObject otherPiece;
            for (int i = 1; i <= dist; i++) {

                // Stop right before any piece
                otherPiece = chessboardManager.FindPiece(row + i * dist, col);
                if (otherPiece != null) break;

                availableMoves.Add(new Position(row + dir * i, col));

            }

            // Moving diagonal if there is an enemy piece
            otherPiece = chessboardManager.FindPiece(row + 1, col + 1);
            if (otherPiece != null && otherPiece.GetComponent<PieceProperties>().team != team) {
                availableMoves.Add(new Position(row + 1, col + 1));
            }
            otherPiece = chessboardManager.FindPiece(row + 1, col - 1);
            if (otherPiece != null && otherPiece.GetComponent<PieceProperties>().team != team) {
                availableMoves.Add(new Position(row + 1, col - 1));
            }

        } else if (PieceProperties.Type.Rook == type) {
            AddOrthogonal(availableMoves, row, col, team, 8);
        } else if (PieceProperties.Type.Bishop == type) {
            AddDiagonal(availableMoves, row, col, team, 8);
        } else if (PieceProperties.Type.Queen == type) {
            AddOrthogonal(availableMoves, row, col, team, 8);
            AddDiagonal(availableMoves, row, col, team, 8);
        } else if (PieceProperties.Type.King == type) {
            AddOrthogonal(availableMoves, row, col, team, 1);
            AddDiagonal(availableMoves, row, col, team, 1);
        } else if (PieceProperties.Type.Knight == type) {
            AddL(availableMoves, row, col, team);
        }

        // Glow all available moves
        FindAndGlowAll(availableMoves);

        return availableMoves;

    }

    // Finds and glows piece and tile at a given position
    private static void FindAndGlow(int row, int col) {

        GameObject foundTile;
        foundTile = chessboardManager.FindTile(row, col);
        if (foundTile != null) {
            foundTile.GetComponent<TileIAction>().SetGlow(true);
        }

        GameObject foundPiece;
        foundPiece = chessboardManager.FindPiece(row, col);
        if (foundPiece != null) {
            foundPiece.GetComponent<ChessPieceIAction>().SetGlow(true);
        }

    }

    // Finds all pieces and tiles at given locations and makes them glow
    private static void FindAndGlowAll(List<Position> positions) {
        foreach(Position p in positions) {
            FindAndGlow(p.row, p.col);
        }
    }

    // Removes glow of all peices an tiles on the board
    public static void ClearGlow() {
        for (int i = 0; i < chessboardManager.tiles.Count; i++) {
            chessboardManager.tiles[i].GetComponent<TileIAction>().SetGlow(false);
        }
        for (int i = 0; i < chessboardManager.pieces.Count; i++) {
            chessboardManager.pieces[i].GetComponent<ChessPieceIAction>().SetGlow(false);
        }
    }

    // Moving for peices going in a straight line
    private static void AddOrthogonal(List<Position> locs, int row, int col, int team, int maxDistance) {

        int dRow, dCol;
        for (int step = 0; step < 4; step++) {

            if (step == 0) {
                dRow = 1;
                dCol = 0;
            } else if (step == 1) {
                dRow = -1;
                dCol = 0;
            } else if (step == 2) {
                dRow = 0;
                dCol = 1;
            } else {
                dRow = 0;
                dCol = -1;
            }

            int i = 1;
            GameObject tile = chessboardManager.FindTile(row + dRow * i, col + dCol * i);
            while (tile != null && i <= maxDistance) {

                // Stop right before friendly piece
                TileProperties tProp = tile.GetComponent<TileProperties>();
                if (tProp.childPiece != null && tProp.childPiece.GetComponent<PieceProperties>().team == team) {
                    break;
                }

                locs.Add(new Position(row + dRow * i, col + dCol * i));
                i++;
                tile = chessboardManager.FindTile(row + dRow * i, col + dCol * i);

                // Stop on enemy piece
                if (tProp.childPiece != null && tProp.childPiece.GetComponent<PieceProperties>().team != team) {
                    break;
                }

            }

        }
        
    }

    // Movement for pieces going diagonal
    private static void AddDiagonal(List<Position> locs, int row, int col, int team, int maxDistance) {

        int dRow, dCol;
        for (int step = 0; step < 4; step++) {

            if (step == 0) {
                dRow = 1;
                dCol = 1;
            } else if (step == 1) {
                dRow = 1;
                dCol = -1;
            } else if (step == 2) {
                dRow = -1;
                dCol = -1;
            } else {
                dRow = -1;
                dCol = 1;
            }

            int i = 1;
            GameObject tile = chessboardManager.FindTile(row + dRow * i, col + dCol * i);
            while (tile != null && i <= maxDistance) {

                // Stop right before friendly piece
                TileProperties tProp = tile.GetComponent<TileProperties>();
                if (tProp.childPiece != null && tProp.childPiece.GetComponent<PieceProperties>().team == team) {
                    break;
                }

                locs.Add(new Position(row + dRow * i, col + dCol * i));
                i++;
                tile = chessboardManager.FindTile(row + dRow * i, col + dCol * i);

                // Stop on enemy piece
                if (tProp.childPiece != null && tProp.childPiece.GetComponent<PieceProperties>().team != team) {
                    break;
                }

            }

        }

    }

    // Movement for Knights
    private static void AddL(List<Position> locs, int row, int col, int team) {

        GameObject tile, piece;
        int dRow, dCol;
        for (int step = 0; step < 8; step++) {

            if (step == 0) {
                dRow = 2;
                dCol = 1;
            } else if (step == 1) {
                dRow = 2;
                dCol = -1;
            } else if (step == 2) {
                dRow = -2;
                dCol = 1;
            } else if (step == 3) {
                dRow = -2;
                dCol = -1;
            } else if (step == 4) {
                dCol = 2;
                dRow = 1;
            } else if (step == 5) {
                dCol = 2;
                dRow = -1;
            } else if (step == 6) {
                dCol = -2;
                dRow = 1;
            } else {
                dCol = -2;
                dRow = -1;
            }

            tile = chessboardManager.FindTile(row + dRow, col + dCol);
            piece = chessboardManager.FindPiece(row + dRow, col + dCol);
            if (tile != null && (piece == null || piece.GetComponent<PieceProperties>().team != team)) {
                locs.Add(new Position(row + dRow, col + dCol));
            }

        }

    }

    // Check if given team's king is in check
    private static bool IsInCheck(int team) {

        // Find team's king piece and get its row and column
        GameObject king = null;
        foreach (GameObject piece in chessboardManager.pieces) {
            PieceProperties pProp = piece.GetComponent<PieceProperties>();
            if (pProp.team == team && pProp.type == PieceProperties.Type.King) {
                king = piece;
                break;
            }
        }

        int kRow = king.GetComponent<PieceProperties>().row;
        int kCol = king.GetComponent<PieceProperties>().column;

        // Check if any enemy pieces are pressuring king
        foreach (GameObject p in chessboardManager.pieces) {
            if (p.GetComponent<PieceProperties>().team != team) {

                List<Position> availableMoves = GetAvailableMoves(p);
                foreach(Position pos in availableMoves) {
                    if (pos.row == kRow && pos.col == kCol) {
                        return true;
                    }
                }

            }
        }

        // If not, return false
        return false;

    }

}

