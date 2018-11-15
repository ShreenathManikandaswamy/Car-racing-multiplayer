using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public GameObject FullMap;

    public void fullmap()
    {
        FullMap.SetActive(true);
    }

    public void close()
    {
        FullMap.SetActive(false);
    }
}
