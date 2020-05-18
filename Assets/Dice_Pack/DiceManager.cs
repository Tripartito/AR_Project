using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
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

    public dice_types diceType = dice_types.d20;
    private Dropdown diceTypeUI;
    public dice_modes diceMode = dice_modes.add;
    private Dropdown diceModeUI;
    public uint numDice = 1;
    private InputField numDiceUI;
    public int diceModifier = 0;
    private InputField diceModifierUI;

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

        diceTypeUI = GameObject.Find("Dice_Type_Dropdown").GetComponent<Dropdown>();
        diceModeUI = GameObject.Find("Dice_Mode_Dropdown").GetComponent<Dropdown>();
        numDiceUI = GameObject.Find("Dice_Amount").GetComponent<InputField>();
        diceModifierUI = GameObject.Find("Dice_Modifier").GetComponent<InputField>();

        if (diceTypeUI != null)
        {
            diceType = (dice_types)diceTypeUI.value;
            currentDice.Add(Instantiate(dicePrefabs[(int)diceType], gameObject.transform.position + new Vector3(0, bowlParameters.y, 0), Quaternion.identity).GetComponent<DiceScript>());
        }
        else
            Debug.LogError("Dice Type UI not recognized in DiceManager.cs!");

        if (diceModeUI != null)
            diceMode = (dice_modes)diceModeUI.value;
        else
            Debug.LogError("Dice Mode UI not recognized in DiceManager.cs!");
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

            bool rollingFinished = true;
            foreach (DiceScript dice in currentDice)
                if (!dice.rollFinished)
                {
                    rollingFinished = false;
                    break;
                }

            if (rollingFinished)
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
