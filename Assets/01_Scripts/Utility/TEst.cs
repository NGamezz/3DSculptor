using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEst : MonoBehaviour
{
    private List<int> ints = new();

    void Start()
    {
        for(int i =0; i < 100; i++ )
        {
            ints.Add( i );
        }

        int index = 0;

        Debug.Log(index++);
        Debug.Log(index);
        Debug.Log(++index);
        Debug.Log(index);
    }
}
