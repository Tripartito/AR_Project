using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public enum dice_types
    {
        d4 = 0,
        d6,
        d8,
        d10,
        d12,
        d20
    }
    public enum dice_modes
    {
        add = 0,
        separated,
        take_highest,
        take_lowest
    }

    public dice_types currDiceType = dice_types.d20;
    public dice_modes currDiceMode = dice_modes.add;
    public uint numDice = 1;
    public int diceModifier = 0;

    private List<DiceScript> currentDice = new List<DiceScript>();
    public GameObject[] dicePrefabs;

    [Header("For Dice Spawn Limits (Width / Dice Drop Point / Length)")]
    public Vector3 bowlParameters;
    private Vector3 diceSize;

    private GameObject gravityGround;

    private bool rolling = false;

    // Start is called before the first frame update
    void Awake()
    {
        gravityGround = GameObject.Find("Bowl_Wall_0");

        currentDice.Add(Instantiate(dicePrefabs[(int)currDiceType], gameObject.transform.position + new Vector3(0, bowlParameters.y, 0), Quaternion.identity).GetComponent<DiceScript>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (DiceScript dice in currentDice)
                dice.ThrowMe();

            rolling = true;
        }
        
        if (rolling)
        {
            Physics.gravity = gravityGround.transform.rotation * new Vector3(0f, -9.81f, 0f);

            bool finished = true;
            foreach (DiceScript dice in currentDice)
                if (!dice.gameObject.GetComponent<Rigidbody>().isKinematic)
                {
                    finished = false;
                    break;
                }

            if (finished)
            {
                int finalResult = 0;

                foreach (DiceScript dice in currentDice)
                    finalResult += dice.rollResult;

                GameObject.Find("Roll_Result").GetComponent<Text>().text = "You rolled a " + finalResult.ToString();
                rolling = false;
            }
        }
    }

    public void ChangeDiceType()
    {

    }

    public void ChangeDiceNum()
    {

    }

    public void ChangeDiceMode()
    {

    }

    public void ChangeModifier()
    {

    }
}
