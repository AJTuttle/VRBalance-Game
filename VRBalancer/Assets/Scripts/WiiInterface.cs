using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiiInterface : MonoBehaviour
{
    //public Transform balanceBoard;
    public int whichRemote = 0; //current highlited remote
    public int balanceBoardIdx = 3;
    public Wii Wii;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Wii.GetExpType(whichRemote) == balanceBoardIdx)//balance board is in
        {
            //balanceBoard.gameObject.SetActive(true);
            //wiimote.gameObject.SetActive(false);
            Vector4 rawBalanceBoard = Wii.GetRawBalanceBoard(whichRemote);
            Vector4 theBalanceBoard = Wii.GetBalanceBoard(whichRemote);
            Vector2 theCenter = Wii.GetCenterOfBalance(whichRemote);

            Debug.Log("total Weight: " + Wii.GetTotalWeight(whichRemote) + "kg");
            Debug.Log("Raw sensor values: " + rawBalanceBoard);
            //Debug.Log("Top Right " + theBalanceBoard.x + "kg");
            //Debug.Log("Top Left " + theBalanceBoard.y + "kg");
            //Debug.Log("Bottom right: " + theBalanceBoard.z + "kg");
            //Debug.Log("Bottom Left: " + theBalanceBoard.w + "kg");
        } else
        {
            Debug.Log("WiiBoard is Inactive");
        }
       
    }
}
