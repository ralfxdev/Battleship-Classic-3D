using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    List<GameObject> touchTiles = new List<GameObject>();
    public float xOffset = 0;
    public float zOffset = 0;
    public float nextZRotation = 90f;
    private GameObject clickedTile;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffsetVect(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + xOffset, 2, tilePos.z + zOffset);
    }

    public void RotateShip()
    {
        touchTiles.Clear();
        if (gameObject.CompareTag("Plane"))
        {
            // Rotate on the Y axis for airplanes
            transform.localEulerAngles += new Vector3(0, nextZRotation, 0);
        }
        else
        {
            // Rotate on Z axis for boats
            transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        }
        nextZRotation *= -1;
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;
        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVect)
    {
        transform.localPosition = new Vector3(newVect.x + xOffset, 2, newVect.z +zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }
}
