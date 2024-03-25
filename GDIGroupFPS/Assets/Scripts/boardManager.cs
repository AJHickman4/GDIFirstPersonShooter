using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class boardManager : MonoBehaviour
{
    [SerializeField] List<GameObject> boards;
    [SerializeField] int boardFixSpeed;
    [SerializeField] int boardDecaySpeed;
    [SerializeField] GameObject player;
    public bool fixing;
    public bool decay;
    public bool isEmpty;

    // Start is called before the first frame update
    void Start()
    {
        isEmpty = true;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isEmpty)
        StartCoroutine(decayBoards());
    }
    private void OnTriggerStay(Collider other)
    {
        isEmpty = false;
        if (Input.GetKey(KeyCode.E))
        {
            fixing = true;
        }
        else
            fixing = false;
            if (fixing)
                StartCoroutine(fixBoards());
        
    }
    void OnTriggerExit(Collider other)
    {
        isEmpty = true;
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
                if (!boards[i].activeInHierarchy)
                {
                    yield return new WaitForSeconds(boardFixSpeed);
                    boards[i].SetActive(true);
                }
            }
            else yield return null;
        }
    }

    IEnumerator decayBoards()
    {
        for (int i = 0; i < boards.Count; i++)
        {
            if (isEmpty)
            {
                if (!boards[i].activeInHierarchy)
                {
                    yield return null;
                }
                if (boards[i].activeInHierarchy)
                {
                    yield return new WaitForSeconds(boardDecaySpeed);
                    boards[i].SetActive(false);
                }
            }
            else yield return null;
        }

    }
    
}
