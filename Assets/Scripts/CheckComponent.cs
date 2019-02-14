using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EnterMark[] gates = FindObjectsOfType<EnterMark>();
        foreach (var single in gates)
        {
            Debug.Log(single.gameObject.name);
        }
    }
}
