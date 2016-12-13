using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class UserController : NetworkBehaviour {
    
    public int playerNum;

    [SyncVar]
    private GameObject chessboard;

    void Start() {
        playerNum = GameObject.FindGameObjectsWithTag("User").Length - 1;
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (chessboard == null) {
            chessboard = GameObject.FindGameObjectWithTag("Chessboard");
        }

    }

    [Command]
    public void CmdSelectPiece(int pieceId) {
        Debug.Log("Server Select Piece: " + pieceId);

        NetworkIdentity netId = chessboard.GetComponent<NetworkIdentity>();
        netId.AssignClientAuthority(connectionToClient);

        chessboard.GetComponent<ChessboardManager>().SelectPiece(pieceId);
        RpcSelectPiece(pieceId);

        netId.RemoveClientAuthority(connectionToClient);

    }

    [ClientRpc]
    public void RpcSelectPiece(int pieceId) {
        if (!isServer) {
            Debug.Log("Client Select Piece: " + pieceId);
            chessboard.GetComponent<ChessboardManager>().SelectPiece(pieceId);
        }
    }

    [Command]
    public void CmdMovePiece(int tileId) {
        Debug.Log("Server Move Piece to Tile: " + tileId);

        NetworkIdentity netId = chessboard.GetComponent<NetworkIdentity>();
        netId.AssignClientAuthority(connectionToClient);

        chessboard.GetComponent<ChessboardManager>().MovePiece(tileId);
        RpcMovePiece(tileId);

        netId.RemoveClientAuthority(connectionToClient);

    }

    [ClientRpc]
    public void RpcMovePiece(int tileId) {
        if (!isServer) {
            Debug.Log("Client Move Piece to Tile: " + tileId);
            chessboard.GetComponent<ChessboardManager>().MovePiece(tileId);
        }
    }

}
