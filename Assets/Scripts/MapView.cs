using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour {

    int[,] mapIndices;

    private void SetMapIndices(int width, int hieght)
    {
        mapIndices = new int[width, hieght];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < hieght; j++)
            {
                mapIndices[i, j] = 0;
            }
        }
    }

	// Use this for initialization
	void Start () {
        SetMapIndices(50, 30);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
