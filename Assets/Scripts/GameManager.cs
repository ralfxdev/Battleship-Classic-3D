using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] ships;
    [Header("Hud")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Text topText;
    public Text playerText;
    public Text enemyText;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;
    public List<TileScript> allTileScript;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private int shipIndex = 0;
    private ShipScript shipScript;
    public EnemyScript enemyScript;
    private List<int[]> enemyShips;
    private List<GameObject> playerFires;
    private List<GameObject> enemyFires;


    private int enemyShipCount = 5;
    private int playerShipCount = 5;

    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        enemyShips = enemyScript.PlaceEnemyShips();
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            
            shipScript.FlashColor(Color.red);
        }
        else
        {
            if(shipIndex < ships.Length - 1)
            {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                topText.text = "Guess an enemy tile";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++)
                {
                    ships[i].SetActive(false);
                }
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            Vector3 tilePos = tile.transform.position;
            tilePos.y += 15;
            playerTurn = false;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
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

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum)){
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    topText.text = "Sunk!";
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "Hit!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }
        }
        if(hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Miss!";
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 0.2f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if(hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipCount--;
            playerText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }
        Invoke("EndEnemyTurn", 2.0f);
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(true);
        }
        foreach (GameObject fire in playerFires)
        {
            fire.SetActive(true);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(false);
        }
        enemyText.text = enemyShipCount.ToString();
        topText.text = "Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if(playerShipCount < 1)
        {
            GameOver("You Win!");
        }
    }

    private void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(false);
        }
        foreach (GameObject fire in playerFires)
        {
            fire.SetActive(false);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(true);
        }
        playerText.text = playerShipCount.ToString();
        topText.text = "Select a tile";
        enemyScript.NPCTurn();
        ColorAllTiles(1);
        if (enemyShipCount < 1)
        {
            GameOver("You Lose!");
        }
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach(TileScript tileScript in allTileScript)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    void GameOver(string message)
    {
        topText.text = "Game Over " + message;
        replayBtn.gameObject.SetActive(true);
    }

    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
