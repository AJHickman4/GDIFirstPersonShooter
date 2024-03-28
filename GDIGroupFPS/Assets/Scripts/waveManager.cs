using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManager : MonoBehaviour
{
    public static waveManager instance;
    public waveSpawner[] spawners;
    [SerializeField] int timeBetweenSpawns;

    public int waveCurrent;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        StartCoroutine(startWave());
    }

    public IEnumerator startWave()
    {
            waveCurrent++;
        
        if (waveCurrent <= spawners.Length)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            spawners[waveCurrent - 1].startWave();
        }
    }
}
