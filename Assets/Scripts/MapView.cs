using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour {

    //int[,] mapIndices;

    //private void SetMapIndices(int width, int hieght)
    //{
    //    mapIndices = new int[width, hieght];
    //    for (int i = 0; i < width; i++)
    //    {
    //        for (int j = 0; j < hieght; j++)
    //        {
    //            mapIndices[i, j] = 0;
    //        }
    //    }
    //}

    float drawCounter = 0;
    float counterTimeOut = 5.0f;

    // Use this for initialization
    void Start () {
        //SetMapIndices(50, 30);

	}
	
	// Update is called once per frame
	void Update ()
    {
        drawCounter += Time.deltaTime;
        if (drawCounter >= counterTimeOut)
        {
            Debug.Log("Tick!");
            drawCounter = 0;
        }
    }
}
