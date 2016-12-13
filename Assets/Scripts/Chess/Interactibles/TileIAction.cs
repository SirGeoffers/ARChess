using UnityEngine;
using System.Collections;

public class TileIAction : InteractibleAction
{

    private ChessboardManager chessboardManager;
    private Material[] defaultMaterials;
    private TileProperties properties;

    private bool isGlowing = false;

    void Start()
    {
        chessboardManager = GameObject.FindGameObjectWithTag("Chessboard").GetComponent<ChessboardManager>();
        properties = GetComponent<TileProperties>();
        defaultMaterials = GetComponent<Renderer>().materials;
    }

    public override void ActionGazeEntered()
    {
        base.ActionGazeEntered();
        chessboardManager.focusedObjectCount++;

    }

    public override void ActionGazeExited()
    {
        base.ActionGazeExited();
        chessboardManager.focusedObjectCount--;

    }

    public override void ActionOnSelect()
    {
        base.ActionOnSelect();
        
        if (chessboardManager.localUser == null) chessboardManager.GetLocalUser();
        //if (chessboardManager.currentTurn != chessboardManager.localUser.GetComponent<UserController>().playerNum) return;
        chessboardManager.SelectPosition("t", properties.id);

    }
    public void SetGlow(bool glow)
    {

        isGlowing = glow;

        int rimActive = 0;
        if (glow)
        {
            rimActive = 1;
        }

        for (int i = 0; i < defaultMaterials.Length; i++)
        {
            defaultMaterials[i].SetInt("_RimActive", rimActive);
        }

        foreach (Transform child in this.transform)
        {
            GameObject childGO = child.gameObject;
            Material[] childDM = childGO.GetComponent<Renderer>().materials;
            for (int i = 0; i < childDM.Length; i++)
            {
                childDM[i].SetInt("_RimActive", rimActive);
            }
        }

    }

    public bool IsGlowing() {
        return isGlowing;
    }

}
