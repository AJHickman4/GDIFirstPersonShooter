using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class boardManager : MonoBehaviour
{
    [SerializeField] List<GameObject> boards;
    [SerializeField] int boardFixSpeed;
    [SerializeField] int boardDestroySpeed;
    [SerializeField] GameObject player;
    public bool fixing;
    public bool destroy;

    bool enemyTrigger;
    bool playerTrigger;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //private void OnTriggerStay(Collider other)
    //{

    //    if (other.CompareTag("Player"))
    //        playerTrigger = true;
    //   else 
    //        enemyTrigger = true;


    //            if (Input.GetKey(KeyCode.E))
    //                fixing = true;
    //            else if (other.CompareTag("Player"))
    //                fixing = false;
    //            if (fixing)
    //                StartCoroutine(fixBoards());


    //    if (enemyTrigger)
    //    {
    //        destroy = true;
    //        StartCoroutine(destroyBoards());
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            playerTrigger = true;
        if (other.tag == "Enemy")
            enemyTrigger = true;

        if (playerTrigger)
        {
            fixing = true;
            if (fixing)
                StartCoroutine(fixBoards());
        }
        if (enemyTrigger)
        {
            destroy = true;
            if (destroy)
                StartCoroutine(destroyBoards());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            playerTrigger = false;
            fixing = false;
        }
        if (other.tag == "Enemy")
        { 
            enemyTrigger = false;
            destroy = false;
        }
    }

    IEnumerator fixBoards()
    {
        for (int i = 0; i < boards.Count; i++)
        {
            if (fixing)
            {
                if (boards[i].activeInHierarchy)
                {
                    yield return null;
                }
                else if (!boards[i].activeInHierarchy)
                {
                    yield return new WaitForSeconds(boardFixSpeed);
                    boards[i].SetActive(true);
                }
            }
            else yield return null;
        }
    }
    IEnumerator destroyBoards()
    {
        for (int i = 0; i < boards.Count; i++)
        {
            yield return new WaitForSeconds(boardDestroySpeed);
            boards[i].SetActive(false);
        }
    }
}
