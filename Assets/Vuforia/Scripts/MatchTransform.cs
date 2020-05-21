using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTransform : MonoBehaviour
{
    [Header("Leave empty! ")]
    public GameObject GO_to_match;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GO_to_match != null)
        {
            transform.position = GO_to_match.transform.position;
            transform.rotation = GO_to_match.transform.rotation;
        }
    }
}
