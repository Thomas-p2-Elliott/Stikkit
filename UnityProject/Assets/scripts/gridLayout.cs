using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridLayout : MonoBehaviour {

    public GameObject square;
    public int gridX=10, gridY=20;
    public GameObject[,] squareArray;
    public float tileSpacing = 10.1f;

	void Start () {
        squareArray = new GameObject[gridX,gridY];
        for (int i = 0; i < gridX; i++)
        {
                for (int n = 0; n < gridY; n++)
                {
                GameObject gridTemp = Instantiate(square, new Vector3(transform.position.x+i*tileSpacing,transform.position.y+n*tileSpacing), square.transform.rotation) as GameObject;
                gridTemp.name = "grid:" + i + "," + n;
                gridTemp.transform.parent = transform;
                squareArray[i,n] = gridTemp;
            }
        }
    }
	
	void Update () {
		
	}
}