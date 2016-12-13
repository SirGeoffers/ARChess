using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChessboardManager : NetworkBehaviour {
    
    private NetworkManager networkManager;

    public float scale;

    [SerializeField]
    private ChessboardPrefabs prefabs;

    [SerializeField]
    private Transform piecesTransform;
    public List<GameObject> pieces;

    [SerializeField]
    private Transform board;
    public List<GameObject> tiles;

    HoloToolkit.Unity.GestureManipulator manipulator;
    public HoloToolkit.Unity.GestureManager gestureManager;
    public int focusedObjectCount = 0;
    private bool manipulatingBoard = false;
    [SerializeField]
    Color transparencyColor;
    
    private GameObject selectedPiece;

    public GameObject localUser;
    public int currentTurn = 0;

	public void Start() {
        
        CreateBoard();

        ChessRules.chessboardManager = this;

        GetLocalUser();

        manipulator = GetComponent<HoloToolkit.Unity.GestureManipulator>();
        gestureManager = GameObject.FindGameObjectWithTag("HoloManagers").GetComponent<HoloToolkit.Unity.GestureManager>();
        gestureManager.ManipulationStarted += ManipulateBoardStarted;
        gestureManager.ManipulationCompleted += ManipulateBoardCompleted;
        gestureManager.ManipulationCanceled += ManipulateBoardCanceled;

    }

    public void Update() {

        if (focusedObjectCount > 0 && !manipulator.enabled) {
            manipulator.enabled = true;
        } else if (focusedObjectCount == 0 && manipulator.enabled && !manipulatingBoard) {
            manipulator.enabled = false;
        }

    }

    public void GetLocalUser() {

        if (localUser != null) return;

        foreach (GameObject u in GameObject.FindGameObjectsWithTag("User")) {
            if (u.GetComponent<UserController>().isLocalPlayer) {
                localUser = u;
            }
        }

    }

    public void CreateBoard() {

        int pieceId = 0;
        int tileId = 0;
        tiles = new List<GameObject>();
        pieces = new List<GameObject>();

        // Set up board
        bool isBlack = false;
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {

                // Add tile
                GameObject tile = Instantiate(prefabs.board.tilePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                tile.transform.parent = board;
                tile.transform.localPosition = new Vector3(i * 0.3f - 7 * 0.15f, 0, j * 0.3f - 7 * 0.15f);
                if (isBlack) {
                    tile.GetComponent<Renderer>().material = prefabs.materials.black;
                } else {
                    tile.GetComponent<Renderer>().material = prefabs.materials.white;
                }
                tile.GetComponent<TileProperties>().originalColor = tile.GetComponent<Renderer>().material.GetColor("_ColorTint");

                tile.GetComponent<TileProperties>().id = tileId;
                tile.GetComponent<TileProperties>().row = i;
                tile.GetComponent<TileProperties>().column = j;

                // Increment id for next tile
                tileId++;

                // Add tile to list
                tiles.Add(tile);

                // Choose piece to add based on position
                GameObject piecePrefab = null;
                if (i == 1 || i == 6) { // Pawn
                    piecePrefab = prefabs.pieces.pawnPrefab;
                } else if (i == 0 || i == 7) {
                    if (j == 0 || j == 7) {
                        piecePrefab = prefabs.pieces.rookPrefab;
                    } else if (j == 1 || j == 6) {
                        piecePrefab = prefabs.pieces.knightPrefab;
                    } else if (j == 2 || j == 5) {
                        piecePrefab = prefabs.pieces.bishopPrefab;
                    } else if (j == 3) {
                        piecePrefab = prefabs.pieces.kingPrefab;
                    } else if (j == 4) {
                        piecePrefab = prefabs.pieces.queenPrefab;
                    }
                }

                // Add a piece
                if (tile != null && piecePrefab != null) {

                    // Place piece on board and set parent to tile just placed
                    GameObject piece = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    piece.transform.parent = piecesTransform;
                    piece.transform.localPosition = new Vector3(0, 0.05f, 0);
                    piece.GetComponent<PieceProperties>().id = pieceId;
                    piece.GetComponent<PieceProperties>().row = i;
                    piece.GetComponent<PieceProperties>().column = j;
                    piece.GetComponent<PieceProperties>().parentTile = tile;
                    piece.GetComponent<ChessPieceIAction>().SetManager(this);

                    tile.GetComponent<TileProperties>().childPiece = piece;

                    // Increment id for next peice
                    pieceId++;

                    // Add piece to list
                    pieces.Add(piece);

                    // Set team of piece
                    if (i > 4) piece.GetComponent<PieceProperties>().team = 1;
                    if (piece.GetComponent<PieceProperties>().team == 1) {
                        piece.GetComponent<Renderer>().material = prefabs.materials.team1;
                    } else {
                        piece.GetComponent<Renderer>().material = prefabs.materials.team0;
                    }
                    piece.GetComponent<PieceProperties>().originalColor = piece.GetComponent<Renderer>().material.GetColor("_ColorTint");

                }

                isBlack = !isBlack;

            }
            isBlack = !isBlack;
        }

        for (int x = 0; x < tiles.Capacity; x++)
        {
            int rowx = tiles[x].GetComponent<TileProperties>().row;
            if (rowx == 0 || rowx == 1 || rowx == 6 || rowx == 7)
            {
                tiles[x].GetComponent<TileProperties>().isEmpty = false;
            }
            else
            {
                tiles[x].GetComponent<TileProperties>().isEmpty = true;
            }
        }

        this.transform.localScale = Vector3.one * scale;

    }

    public GameObject FindPiece(int row, int column) {
        foreach (GameObject p in pieces) {
            PieceProperties properties = p.GetComponent<PieceProperties>();
            if (properties.row == row && properties.column == column) {
                return p;
            }
        }
        return null;
    }

    public GameObject GetPiece(int id) {
        foreach (GameObject p in pieces) {
            PieceProperties properties = p.GetComponent<PieceProperties>();
            if (properties.id == id) {
                return p;
            }
        }
        return null;
    }

    public GameObject FindTile(int row, int column) {
        foreach (GameObject t in tiles) {
            TileProperties properties = t.GetComponent<TileProperties>();
            if (properties.row == row && properties.column == column) {
                return t;
            }
        }
        return null;
    }

    public GameObject GetTile(int id) {
        foreach (GameObject t in tiles) {
            TileProperties properties = t.GetComponent<TileProperties>();
            if (properties.id == id) {
                return t;
            }
        }
        return null;
    }

    public void SelectPosition(String type, int id) {

        GameObject piece, tile;

        if (localUser == null) GetLocalUser();

        if ("p" == type) {
            piece = GetPiece(id);
            tile = piece.GetComponent<PieceProperties>().parentTile;
        } else if ("t" == type) {
            tile = GetTile(id);
            piece = tile.GetComponent<TileProperties>().childPiece;
        } else {
            return;
        }

        if (piece != null && (selectedPiece == null || selectedPiece == piece)) {
            localUser.GetComponent<UserController>().CmdSelectPiece(piece.GetComponent<PieceProperties>().id);
        } else if (tile != null && tile.GetComponent<TileIAction>().IsGlowing()) {
            localUser.GetComponent<UserController>().CmdMovePiece(tile.GetComponent<TileProperties>().id);
        }

    }

    public void SelectPiece(int pieceId) {

        if ((selectedPiece != null) && (selectedPiece == GetPiece(pieceId)))
        {
            selectedPiece.GetComponent<ChessPieceIAction>().SetSelected(false);
            selectedPiece = null;
            ChessRules.ClearGlow();

        }
        else if ((selectedPiece != null) && (selectedPiece != GetPiece(pieceId)))
        {
            // do nothing
        }
        else if (selectedPiece == null)
        {
            selectedPiece = GetPiece(pieceId);
            if (selectedPiece != null) selectedPiece.GetComponent<ChessPieceIAction>().SetSelected(true);
            getAvailableMove(selectedPiece);
        }

    }

    public void getAvailableMove(GameObject piece)
    {
        //ChessRules.availableMoves(piece);
        ChessRules.GetAvailableMoves(piece);

    }

    public void MovePiece(int tileId) {

        // Break if no tile is selected
        if (selectedPiece != null)
        {

            // Get tile based on given id
            GameObject tile = GetTile(tileId);
            GameObject parent = selectedPiece.GetComponent<PieceProperties>().parentTile;

            // Capture piece that is on the tile we're moving to
            GameObject pieceToRemove = null;
            foreach (GameObject p in pieces)
            {

                // Remove game object
                if (tile == p.GetComponent<PieceProperties>().parentTile)
                {
                    pieceToRemove = p;
                }

            }

            if (pieceToRemove != null)
            {
                pieces.Remove(pieceToRemove);
                Destroy(pieceToRemove);
            }

            // TODO: May want to keep track of pieces removed

            // Move piece
            parent.GetComponent<TileProperties>().childPiece = null;

            selectedPiece.GetComponent<PieceProperties>().parentTile = tile;
            selectedPiece.GetComponent<PieceProperties>().row = tile.GetComponent<TileProperties>().row;
            selectedPiece.GetComponent<PieceProperties>().column = tile.GetComponent<TileProperties>().column;
            selectedPiece.GetComponent<ChessPieceIAction>().SetSelected(false);

            tile.GetComponent<TileProperties>().childPiece = selectedPiece;

            selectedPiece = null;

            ChessRules.ClearGlow();
            tile.GetComponent<TileProperties>().isEmpty = false;
            parent.GetComponent<TileProperties>().isEmpty = true;

        }

        currentTurn = (currentTurn + 1) % 2;

    }

    private void ManipulateBoardStarted() {
        if (!manipulator.enabled) return;
        manipulatingBoard = true;
        SetTransparency(true);
    }

    private void ManipulateBoardCompleted() {
        manipulatingBoard = false;
        if (!manipulator.enabled) return;
        SetTransparency(false);
    }

    private void ManipulateBoardCanceled() {
        manipulatingBoard = false;
        if (!manipulator.enabled) return;
        SetTransparency(false);
    }

    private void SetTransparency(bool isTransparent) {

        Color toColor = transparencyColor;
        int isTexActive = 1;
        if (isTransparent) {
            isTexActive = 0;
        }

        foreach (GameObject t in tiles) {
            foreach (Material m in t.GetComponent<Renderer>().materials) {
                if (!isTransparent) toColor = t.GetComponent<TileProperties>().originalColor;
                m.SetColor("_ColorTint", toColor);
                m.SetInt("_TexActive", isTexActive);
            }
        }

        foreach (GameObject p in pieces) {
            foreach (Material m in p.GetComponent<Renderer>().materials) {
                if (!isTransparent) toColor = p.GetComponent<PieceProperties>().originalColor;
                m.SetColor("_ColorTint", toColor);
                m.SetInt("_TexActive", isTexActive);
            }
        }

    }

    [Serializable]
    public class ChessboardPrefabs {

        public Pieces pieces;
        public Board board;
        public Materials materials;

        [Serializable]
        public class Pieces {
            public GameObject pawnPrefab;
            public GameObject rookPrefab;
            public GameObject knightPrefab;
            public GameObject bishopPrefab;
            public GameObject queenPrefab;
            public GameObject kingPrefab;
        }

        [Serializable]
        public class Board {
            public GameObject tilePrefab;
        }

        [Serializable]
        public class Materials {
            public Material black;
            public Material white;
            public Material team0;
            public Material team1;
        }
        

    }


}
