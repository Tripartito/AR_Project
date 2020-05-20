using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenEntity : MonoBehaviour
{
    public bool is_visible = false;

    [Header("Detection bools (Vuforia changes this values)")]
    public bool player_target_found = false;
    public bool GM_target_found = false;

    [Header("Actual Prefab or GameObject we will render")]
    public GameObject GameObject_to_show;
    private MatchTransform GO_to_show_trans;


    private GameObject player_visible;
    private GameObject GM_visible;

    private DefaultTrackableEventHandler player_tracker;
    private DefaultTrackableEventHandler GM_tracker;

    private GameManagement manager;



    // Start is called before the first frame update
    void Start()
    {
        player_visible = this.transform.Find("Player_Target").gameObject;
        GM_visible = this.transform.Find("GM_Target").gameObject;

        player_tracker = player_visible.GetComponent<DefaultTrackableEventHandler>();
        GM_tracker = GM_visible.GetComponent<DefaultTrackableEventHandler>();

        manager = GameObject.Find("GameManager").GetComponent<GameManagement>();


        GO_to_show_trans = GameObject_to_show.GetComponent<MatchTransform>();

        GameObject_to_show.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        player_target_found = player_tracker.is_found;
        GM_target_found = GM_tracker.is_found;

        if (player_tracker.is_found == true)
        {
            GameObject_to_show.SetActive(true);
            GO_to_show_trans.GO_to_match = player_visible;
        }
        else if (GM_tracker.is_found == true)
        {
            if (manager.user_is_GM == true)
            {
                GameObject_to_show.SetActive(true);
                GO_to_show_trans.GO_to_match = GM_visible;
            }
            else
                GameObject_to_show.SetActive(false);
        }
        else if (player_tracker.is_found == false && GM_tracker.is_found == false)
        {

        }
    }
}
