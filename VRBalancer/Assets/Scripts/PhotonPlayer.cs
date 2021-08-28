using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PhotonPlayer : MonoBehaviour,IPunObservable
{
    public float lastNetworkUpdateTime = 0; //needed to calculate velocity based upon rate of positional updates
    public Vector3 networkPos;
    public Vector3 networkVelocity;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
        {
            //transform.position = networkPos;
            //transform.position = Vector3.Lerp(transform.position, networkPos, 0.05f); // --> lerping
            transform.position = transform.position + networkVelocity * Time.deltaTime;
            Vector3 error = networkPos - transform.position;
            if (error.magnitude > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, networkPos, 0.1f); // --> lerping
            }
            
        }
        else
        {
            networkPos = transform.position;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // this is the owner object
            stream.Serialize(ref networkPos);
        }
        else
        {
            //float sentTime = (float)info.SentServerTime;
            float sentTime = Time.time;
            Vector3 newNetworkPos = (Vector3)stream.ReceiveNext();
            networkVelocity = (newNetworkPos - networkPos) / (sentTime - lastNetworkUpdateTime);
            networkPos = newNetworkPos;
            lastNetworkUpdateTime = sentTime;
        }
    }
}
