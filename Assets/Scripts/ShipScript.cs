using System;
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
    int hitCount = 0;
    public int shipSize;

    private Material[] allMaterials;
    List<Color> allColors = new List<Color>();

    private void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        for (int i = 0; i < allMaterials.Length; i++)
        {
            allColors.Add(allMaterials[i].color);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(collision.gameObject);
        }
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
        if(clickedTile == null)
        {
            return;
        }
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
        ClearTileList();
        transform.localPosition = new Vector3(newVect.x + xOffset, 2, newVect.z +zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public void FlashColor(Color tempColor)
    {
        ChangeColorRecursively(gameObject, tempColor);
        Invoke("ResetColor", 0.5f);
    }

    private void ChangeColorRecursively(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            foreach (Material mat in materials)
            {
                mat.color = color;
            }
        }

        foreach (Transform child in obj.transform)
        {
            ChangeColorRecursively(child.gameObject, color);
        }
    }


    private void ResetColorRecursively(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = allColors[i];
            }
        }

        foreach (Transform child in obj.transform)
        {
            ResetColorRecursively(child.gameObject);
        }
    }

    private void ResetColor()
    {
        ResetColorRecursively(gameObject);
    }

}
