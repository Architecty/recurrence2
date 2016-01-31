    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class tileManager : MonoBehaviour {

    public GameObject playerController;
    private Camera playerCamera;

    //These two tiles mark the start and end of the level
    public GameObject startTile;
    public GameObject endTile;

    //These tiles are generated as you move forward. There will always be one more transit tile than there is action tile
    //(for the space between the last action tile and the end tile)
    public GameObject[] tileList;

    public GameObject currentTileGameObject;
    public List<GameObject> nextTilePool;

    public GameObject[][] transitPools;

    //This is your state of progress, and used to score at the end of the level, as well as show where your next step is.
    //0 means you haven't passed through the level. 1 is that you passed it. All other numbers are failures of some variety.
    public int[] levelSuccess = new int[0];
    public bool canProgress = true;

    //The current stage you're in. This informs the system regarding which tile to put next.
    public int currentTile = 0;

    // Use this for initialization
    void Start () {
        initializeVariables();
        createNextTiles(startTile);
    }
	
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            canProgress = !canProgress;
        }
    }

    void initializeVariables()
    {
        playerCamera = playerController.transform.GetComponentInChildren<Camera>();
        levelSuccess = new int[tileList.Length];
        for(int i = 0; i < levelSuccess.Length; i++)
        {
            levelSuccess[i] = 0;
        }
    }

    public void enterTile(GameObject thisTile)
    {
        //Remove this tile from the 
        nextTilePool.Remove(thisTile);
        //Record the tile we just passed through. We'll delete it once we've assigned the current reference somewhere else. 
        GameObject previousGameObject = currentTileGameObject;
        if(previousGameObject != thisTile)
        {
            nextTilePool.Add(previousGameObject);
        }
        currentTileGameObject = thisTile;
        //Destroy(previousGameObject);
        //Now that this is done, go to the next tile.
        moveToNextTile(thisTile);
    }

    void moveToNextTile(GameObject centerTile)
    {

        //Deactivate all of the old tiles
        foreach(GameObject oldTile in nextTilePool)
        {
            deactivateTile(oldTile);
        }
        //Create a new pool object to fill
        nextTilePool = new List<GameObject>();
        //Make the new tiles.
        createNextTiles(centerTile);
    }

    void createNextTiles(GameObject centerTile)
    {

        var currentTileInt = getTileNumber(tileList, centerTile);

        Debug.Log("Current Tile Is: " + currentTileInt);
        GameObject nextTile;
        if (canProgress)
        {
            Debug.Log("Goes Up " + canProgress.ToString());
            nextTile = tileList[currentTileInt + 1];
            canProgress = true;
        } else
        {
            Debug.Log("Goes Down " + canProgress.ToString());
            int whichInt = currentTileInt > 1 ? currentTileInt - 1 : 1;
            nextTile = tileList[whichInt];
            canProgress = true;
        }

        //Activate a number of the tiles equal to the current tile's entrance/exit points
        foreach (Transform t in centerTile.transform)
        {
            if (t.name == "Portals")
            {
                foreach (Transform x in t.transform)
                {
                    if (x.tag == "Entrance" || x.tag == "Exit")
                    {
                        activateTile(nextTile, x);
                    }
                }
            }
        }
        //If we've entered a 'choice' tile, then stop progression until we've done it right
        if (centerTile.tag == "Choice")
        {
            canProgress = false;
        }
    }

    void deactivateTile(GameObject tile)
    {
        if(tile.name == "Start")
        {
            tileList[0] = new GameObject();
        }
        Destroy(tile);
    }

    //Create / activate a new tile of a certain type, with its entry point in the same place as the 
    void activateTile(GameObject tile, Transform exitPoint)
    {
        GameObject newTile = Instantiate(tile, exitPoint.position, exitPoint.rotation) as GameObject;
        newTile.name = tile.name;
        nextTilePool.Add(newTile);
    }



    //Find out where in our tileArray the requested tile is at.
    int getTileNumber(GameObject[] tileArray, GameObject targetTile)
    {
        for(int e = 0; e < tileArray.Length; e++)
        {
            if(tileArray[e].name == targetTile.name)
            {
                return e;
            }
        }
        return 0;
    }
}
