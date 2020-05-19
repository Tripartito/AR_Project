using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Target_Link : MonoBehaviour
{

    public GameObject original_target;
    // Start is called before the first frame update
    private void Awake()
    {
        this.gameObject.SetActive(true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = original_target.transform.position;
    }
}
