using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardManager : MonoBehaviour
{
    [SerializeField] List<GameObject> boards;
    [SerializeField] int boardFixSpeed;
    [SerializeField] int boardDestroySpeed;
    public bool fixing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
                fixing = true;
            else if (other.CompareTag("Player"))
                fixing = false;
            if (fixing)
                StartCoroutine(fixBoards());
        }
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(fixBoards());
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
