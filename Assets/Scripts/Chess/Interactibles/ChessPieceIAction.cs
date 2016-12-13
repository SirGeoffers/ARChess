using UnityEngine;
using System.Collections;

public class ChessPieceIAction : InteractibleAction {

    private ChessboardManager chessboardManager;
    private PieceProperties properties;

    private Material[] defaultMaterials;

    private bool isSelected = false;
    private float lastSelectActionTime = -999;

    private Vector3 iPosition;
    private Vector3 ePosition;
    private float journeyLength;
    private float maxHeight = 0.15f;
    private float speed = 1.0f;

    void Start() {

        properties = GetComponent<PieceProperties>();

        defaultMaterials = GetComponent<Renderer>().materials;

        // Add a BoxCollider if the interactible does not contain one.
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null) {
            gameObject.AddComponent<BoxCollider>();
        }

        iPosition = this.transform.localPosition;
        ePosition = iPosition + new Vector3(0, maxHeight, 0);
        journeyLength = Vector3.Distance(iPosition, ePosition);

    }

    void Update() {

        if (isSelected)
        {
            float distCovered = (Time.time - lastSelectActionTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.localPosition = Vector3.Lerp(iPosition, ePosition, fracJourney);
        }
        else {
            float distCovered = (Time.time - lastSelectActionTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.localPosition = Vector3.Lerp(ePosition, iPosition, fracJourney);
        }

        // Move to parent tile
        GameObject parentTile = properties.parentTile;
        if (parentTile != null) {
            transform.localPosition = new Vector3(parentTile.transform.localPosition.x, transform.localPosition.y, parentTile.transform.localPosition.z);
        }
        

    }

    public override void ActionGazeEntered() {
        base.ActionGazeEntered();

        SetGlow(true);
        chessboardManager.focusedObjectCount++;

    }

    public override void ActionGazeExited() {
        base.ActionGazeExited();

        SetGlow(false);
        chessboardManager.focusedObjectCount--;

    }

    public override void ActionOnSelect() {
        base.ActionOnSelect();


        if (chessboardManager.localUser == null) chessboardManager.GetLocalUser();
        //if (chessboardManager.currentTurn != chessboardManager.localUser.GetComponent<UserController>().playerNum) return;
        chessboardManager.SelectPosition("p", GetComponent<PieceProperties>().id);

    }

    public void SetManager(ChessboardManager chessboardManager) {
        this.chessboardManager = chessboardManager;
    }

    public void SetSelected(bool isSelected) {
        this.isSelected = isSelected;
        lastSelectActionTime = Time.time;
    }

    public void SetGlow(bool glow) {

        int rimActive = 0;
        if (glow) {
            rimActive = 1;
        }

        for (int i = 0; i < defaultMaterials.Length; i++) {
            defaultMaterials[i].SetInt("_RimActive", rimActive);
        }

        foreach (Transform child in this.transform) {
            GameObject childGO = child.gameObject;
            Material[] childDM = childGO.GetComponent<Renderer>().materials;
            for (int i = 0; i < childDM.Length; i++) {
                childDM[i].SetInt("_RimActive", rimActive);
            }
        }

    }

}
