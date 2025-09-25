using System;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public GameObject[] fighters;
    public GameObject spawnPoint;
    
    private int fighterIndex;
    private void Awake()
    {
        fighterIndex = PlayerPrefs.GetInt("Character");
        Instantiate(fighters[fighterIndex], spawnPoint.transform.position, Quaternion.identity);
    }
}
