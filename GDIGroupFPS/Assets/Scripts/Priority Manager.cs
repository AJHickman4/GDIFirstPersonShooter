using UnityEngine;
using System.Collections.Generic;

public class PriorityManager : MonoBehaviour
{
    private static List<int> availablePriorities = new List<int>();
    private static HashSet<int> assignedPriorities = new HashSet<int>();
    private static PriorityManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePriorities();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePriorities()
    {
        for (int i = 0; i < 100; i++)
        {
            availablePriorities.Add(i);
        }
        Shuffle(availablePriorities);
    }

    public static int GetUniquePriority()
    {
        if (availablePriorities.Count > 0)
        {
            int priority = availablePriorities[0];
            availablePriorities.RemoveAt(0);
            assignedPriorities.Add(priority);
            return priority;
        }
        else
        {
            return -1;
        }
    }


    public static void ReleasePriority(int priority)
    {
        if (assignedPriorities.Contains(priority))
        {
            assignedPriorities.Remove(priority);
            availablePriorities.Add(priority); 
            Shuffle(availablePriorities); 
        }
        else
        {
            return;
        }
    }

    private static void Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}