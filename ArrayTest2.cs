using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayTest2 : MonoBehaviour
{

    [SerializeField]private int[][] categoriesList;
    // Start is called before the first frame update
    void Start()
    {
        categoriesList = new int[4][];
        //categoriesList[3] = new int[4];
        for (int i = 0,dad = 1; i < categoriesList.Length; i++,dad*=2)
        {
            categoriesList[i] = new int[dad];
            Debug.Log("dad:"+i+"child:"+dad+"TotalParts:"+ categoriesList.Length);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
