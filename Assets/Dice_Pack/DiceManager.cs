using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

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
    public uint diceNum = 0;
    private InputField diceNumUI;
    public int diceModifier = 0;
    private InputField diceModifierUI;

    private GameObject camera;
    private string roll_string;
    private Text rollTextComp;
    private TextMesh rollTextMeshComp;

    private List<DiceScript> currentDice = new List<DiceScript>();
    public GameObject[] dicePrefabs;

    [Header("For Dice Spawn Data (Width / Height / Length)")]
    public Vector3 areaParameters;
    private List<Vector3> spawnLocations = new List<Vector3>();

    private uint maxLineElements = 5;
    private float spawnSeparation = 0.4f;
    //Size = 5x5

    private GameObject gravityGround;

    private bool rolling = false;

    // Start is called before the first frame update
    void Awake()
    {
        gravityGround = GameObject.Find("Bowl_Wall_0");

        camera = GameObject.Find("ARCamera");
        diceTypeUI = GameObject.Find("Dice_Type_Dropdown").GetComponent<Dropdown>();
        diceModeUI = GameObject.Find("Dice_Mode_Dropdown").GetComponent<Dropdown>();
        diceNumUI = GameObject.Find("Dice_Amount").GetComponent<InputField>();
        diceModifierUI = GameObject.Find("Dice_Modifier").GetComponent<InputField>();

        rollTextComp = GameObject.Find("Roll_Result").GetComponent<Text>();
        rollTextMeshComp = GetComponent<TextMesh>();

        if (diceTypeUI != null)
        {
            diceType = (dice_types)diceTypeUI.value;
        }
        else
            Debug.LogError("Dice Type UI not recognized in DiceManager.cs!");

        if (diceModeUI != null)
            diceMode = (dice_modes)diceModeUI.value;
        else
            Debug.LogError("Dice Mode UI not recognized in DiceManager.cs!");

        int parseResult = 0;
        if (int.TryParse(diceModifierUI.text, out parseResult))
        {
            diceModifier = parseResult;

            if (int.TryParse(diceNumUI.text, out parseResult))
            {
                diceNum = (uint)parseResult;
                RecalculateDicePositions();
                RecalculatePresentDice();
            }
            else
                Debug.LogError("Fucked Up Dice Number Parse!");
        }
        else
            Debug.LogError("Fucked Up Dice Modifier Parse!");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            MakeRoll();

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
                CalculateThrow();
                rolling = false;
            }
        }
        else
        {
            rollTextMeshComp.gameObject.transform.LookAt(camera.transform);
            rollTextMeshComp.gameObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        }
    }

    public void MakeRoll()
    {
        foreach (DiceScript dice in currentDice)
            dice.ThrowMe();

        roll_string = "";
        //rollTextComp.text = "";
        rollTextMeshComp.text = "";
        rolling = true;
    }

    private void CalculateThrow()
    {
        int finalResult = 0;

        switch (diceMode)
        {
            case dice_modes.add:
                foreach (DiceScript dice in currentDice)
                    finalResult += dice.rollResult;

                roll_string = finalResult.ToString();

                break;
            case dice_modes.separated:

                int i = 0;
                foreach (DiceScript dice in currentDice)
                {
                    roll_string += dice.rollResult;

                    if (++i > 3)
                    {
                        roll_string += "\n";
                        i = 0;
                    }
                    else
                        roll_string += " | ";
                }

                if (i > 0)
                {
                    roll_string = roll_string.Remove(roll_string.Length - 3, 3);
                    roll_string += "\n";
                }

                break;
            case dice_modes.take_highest:
                foreach (DiceScript dice in currentDice)
                    if (dice.rollResult > finalResult)
                        finalResult = dice.rollResult;

                roll_string = finalResult.ToString();

                break;
            case dice_modes.take_lowest:
                finalResult = 20;

                foreach (DiceScript dice in currentDice)
                    if (dice.rollResult < finalResult)
                        finalResult = dice.rollResult;

                roll_string = finalResult.ToString();

                break;
        }

        string end_str = "";

        if (diceMode != dice_modes.separated)
            end_str = " = " + (finalResult + diceModifier).ToString();

        if (diceModifier > 0)
            roll_string += " + " + Math.Abs(diceModifier).ToString() + end_str;
        else if (diceModifier < 0)
            roll_string += " - " + Math.Abs(diceModifier).ToString() + end_str;

        //rollTextComp.text = roll_string;
        rollTextMeshComp.text = roll_string;
    }

    private void RecalculateDicePositions()
    {
        spawnLocations.Clear();

        uint rows = (uint)Math.Ceiling(diceNum / (double)maxLineElements);
        uint columns = diceNum;
        if (diceNum > 5)
            columns = 5;

        float spawn_pos_X = -(float)Math.Floor(columns / 2.0) * spawnSeparation;
        float spawn_pos_Z = -(float)Math.Floor(rows / 2.0) * spawnSeparation;

        for (int i = 0; i < rows; ++i)
            for (int j = 0; j < maxLineElements; ++j)
                spawnLocations.Add(new Vector3(spawn_pos_X + spawnSeparation * j, areaParameters.y, spawn_pos_Z + spawnSeparation * i));
    }

    private void RecalculatePresentDice()
    {
        foreach (DiceScript dice in currentDice)
            Destroy(dice.gameObject);

        currentDice.Clear();

        for (int i = 0; i < diceNum; ++i)
        {
            GameObject diceGO = Instantiate(dicePrefabs[(int)diceType], gravityGround.transform.position + gravityGround.transform.rotation * spawnLocations[i], Quaternion.identity);
            diceGO.transform.SetParent(gravityGround.transform.parent, true);

            DiceScript diceScript = diceGO.GetComponent<DiceScript>();
            diceScript.spawnPos = spawnLocations[i];
            currentDice.Add(diceScript);
        }
    }

    // UI -------------------------
    public void ChangeDiceType()
    {
        if (diceType != (dice_types)diceTypeUI.value)
        {
            diceType = (dice_types)diceTypeUI.value;
            RecalculatePresentDice();
        }
    }

    public void ChangeDiceNum()
    {
        int parseResult = 0;
        if (int.TryParse(diceNumUI.text, out parseResult))
        {
            if (diceNum != parseResult)
            {
                if (parseResult > 25)
                {
                    diceNum = 25;
                    diceNumUI.text = "25";
                }
                else
                    diceNum = (uint)parseResult;

                RecalculateDicePositions();
                RecalculatePresentDice();
            }
        }
    }

    public void ChangeDiceMode()
    {
        diceMode = (dice_modes)diceModeUI.value;
    }

    public void ChangeModifier()
    {
        int parseResult = 0;
        if (int.TryParse(diceModifierUI.text, out parseResult))
            diceModifier = parseResult;
    }
}
