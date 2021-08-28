
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WiiCalibrator : MonoBehaviour
{
    public Transform copPosition;
    public Transform cop_sprite;
    //for CoP calculations
    public Transform stickTip;
    //public Transform WiiBalanceBoard;
    public BalanceBoardModel BalanceBoardModel;
    public bool test;
    //public TMP_Text totalWeightText;
    //public TMP_Text xBalanceText;
    //public TMP_Text yBalanceText;
    //public TMP_Text theRemoteNumber;
    //public TMP_Text totalRemotesText;
    //public TMP_Text stickValueText;
    //public TMP_Text stickWeightText;
    //public Transform balanceBoard;
    public int whichRemote = 0; //current highlited remote
    public int balanceBoardIdx = 3;
    public float calibrationVal;
    public float stickWeight;
    public Wii Wii;
    public Stick stick;

    Vector4 theBalanceBoard;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        //stickValueText.text = stick.stick_value.ToString();
        if (test)
        {
            test = false;
            testRemotes();
        }
        int totalRemotes = Wii.GetRemoteCount();
        //Debug.Log("Total Remotes: " + totalRemotes.ToString());
        //totalRemotesText.text = totalRemotes.ToString();
        //stickWeightText.text = stick.weight_on_stick.ToString();
        //Debug.Log("Which remote: " + whichRemote.ToString());
        //theRemoteNumber.text = "Remote on Display:" + (whichRemote + 1).ToString();

        if (Wii.GetExpType(whichRemote) == balanceBoardIdx)//balance board is in
        {
            //balanceBoard.gameObject.SetActive(true);
            //wiimote.gameObject.SetActive(false);
            Vector4 rawBalanceBoard = Wii.GetRawBalanceBoard(whichRemote);
            theBalanceBoard = Wii.GetBalanceBoard(whichRemote);
            Vector2 theCenter = Wii.GetCenterOfBalance(whichRemote);
            float calibratedWeight = (Wii.GetTotalWeight(whichRemote) - calibrationVal);
            //Debug.Log("total Weight: " + (Wii.GetTotalWeight(whichRemote) - calibrationVal) + "kg");
            //Debug.Log("Calibrated Weight = " + calibratedWeight);
            //Debug.Log("Raw sensor values: " + rawBalanceBoard);

            //totalWeightText.text = calibratedWeight + " kg";
            //xBalanceText.text = "x: " + theCenter.x.ToString();
            //yBalanceText.text = "y: " + theCenter.y.ToString();
            //Debug.Log("Top Right " + theBalanceBoard.x + "kg" + " Top Left " + theBalanceBoard.y + "kg" + " Bottom right: " + theBalanceBoard.z + "kg" + " Bottom Left: " + theBalanceBoard.w + "kg");
            //Debug.Log("Top Left " + theBalanceBoard.y + "kg");
            //Debug.Log("Bottom right: " + theBalanceBoard.z + "kg");
            //Debug.Log("Bottom Left: " + theBalanceBoard.w + "kg");
            //BalanceBoardModel.RB.text = "RB: " + (theBalanceBoard.z + BalanceBoardModel.bottomRightCalibrationValue).ToString();
            //BalanceBoardModel.RT.text = "RT: " + (theBalanceBoard.x + BalanceBoardModel.topRightCalibrationValue).ToString();
            //BalanceBoardModel.LT.text = "LT: " + (theBalanceBoard.y + BalanceBoardModel.topLeftCalibrationValue).ToString();
            //BalanceBoardModel.LB.text = "LB: " + (theBalanceBoard.w + BalanceBoardModel.bottomLeftCalibrationValue).ToString();

            //Vector3 copPos = COP();
            //Debug.Log("COP: " + copPos);
            //cop_sprite.transform.position = new Vector3 (copPos.x, copPos.y, copPosition.transform.position.z);
            //cop_sprite.transform.position = new Vector3(copPos.y, copPosition.transform.position.y, copPos.x);

            stickWeight = (float)stick.weight_on_stick;


            List<string> data = new List<string> {
                stickTip.position.ToString(),
                stickWeight.ToString(),
                transform.position.ToString(),
                transform.rotation.ToString(),
                theBalanceBoard.z.ToString(),
                theBalanceBoard.x.ToString(),
                theBalanceBoard.y.ToString(),
                theBalanceBoard.w.ToString()
            };

            unityutilities.Logger.LogRow("Balance_Measurements", data);


        }
        else
        {

            Debug.Log("WiiBoard is Inactive" + " Wii.GetExpType(whichRemote)=  " + Wii.GetExpType(whichRemote));
        }


    }

    void testRemotes()
    {

        for (int i = 0; i < 15; i++)
        {
            Debug.Log("Remote value " + i + " = " + Wii.GetExpType(i));
        }
    }

    Vector2 GetCenterOfPressure(float[] weights, List<Vector3> points)
    {
        Vector2 cop = new Vector2();

        if (weights.Length != points.Count)
        {
            Debug.Log("Length of weights and points shoild be equal!!");
        }
        else
        {

            float weightSum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                weightSum += weights[i];
            }

            float weightX = 0;
            float weightY = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                weightX += points[i].x;
                weightY += points[i].y;
            }
            cop.x = weightX / weightSum;
            cop.y = weightY / weightSum;
        }
        return cop;
    }
    public Vector3 COP ()
    {
        Vector3 cop = Vector3.zero;
        float totalWeight = theBalanceBoard.w + theBalanceBoard.x + theBalanceBoard.y + theBalanceBoard.z;

        float x_weight = BalanceBoardModel.bottomLeft.position.x * theBalanceBoard.w + BalanceBoardModel.bottomRight.position.x * theBalanceBoard.z
            + BalanceBoardModel.topLeft.position.x * theBalanceBoard.y + BalanceBoardModel.topRight.position.x * theBalanceBoard.x;  
             cop.x = x_weight / totalWeight;

        float y_weight = BalanceBoardModel.bottomLeft.position.y * theBalanceBoard.w + BalanceBoardModel.bottomRight.position.y * theBalanceBoard.z
            + BalanceBoardModel.topLeft.position.y * theBalanceBoard.y + BalanceBoardModel.topRight.position.y * theBalanceBoard.x;
        cop.y = y_weight / totalWeight;

        float z_weight = BalanceBoardModel.bottomLeft.position.z * theBalanceBoard.w + BalanceBoardModel.bottomRight.position.z * theBalanceBoard.z
    + BalanceBoardModel.topLeft.position.z * theBalanceBoard.y + BalanceBoardModel.topRight.position.z * theBalanceBoard.x;
        cop.z = z_weight / totalWeight;

        //Debug.Log("total x weight: " + x_weight + "total y weight: " + y_weight);
        return cop;
    }
}
