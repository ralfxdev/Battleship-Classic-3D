using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] ships;
    [Header("Hud")]
    public Button nextBtn;
    public Button rotateBtn;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private int shipIndex = 0;
    private ShipScript shipScript;

    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
    }

    private void NextShipClicked()
    {
        if (shipIndex < ships.Length - 1)
        {
            shipIndex++;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            //shipScript.FlashColor(Color.yellow);
        }
    }

    void Update()
    {
        
    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            // drop a missile - Boom!
        }else if (!setupComplete)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVect(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    private void RotateClicked()
    {
        shipScript.RotateShip();
    }

    private void SetShipClickedTile(GameObject tile)
    {
        shipScript.SetClickedTile(tile);
    }
}
