using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTransform : MonoBehaviour
{
    [Header("Leave empty! ")]
    public GameObject GO_to_match;

    private Vector3 last_pos;
    private Quaternion last_rotation;

    private FOW_Unit detection_info;

    // Start is called before the first frame update
    void Start()
    {
        detection_info = transform.parent.GetComponent<FOW_Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GO_to_match != null)
        {
            if (detection_info.is_visible == true && detection_info.player_target_found == true)
            {
                transform.position = GO_to_match.transform.position;
                transform.rotation = GO_to_match.transform.rotation;
            } 
            else
            {
                transform.position = last_pos;
                transform.rotation = last_rotation;
            }

            last_pos = transform.position;
            last_rotation = transform.rotation;
        }
    }
}
