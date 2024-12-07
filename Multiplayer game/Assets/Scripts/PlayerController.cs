using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public Button raiseHandButton;
    public float movementSpeed = 3f;
    public float turnSpeed = 100f;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        if (photonView.IsMine)
        {
            raiseHandButton.onClick.AddListener(() => RaiseHandRPC());
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
        }
        else
        {
            InterpolateMovement();
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;
        transform.Translate(horizontal, 0, vertical);

        float turn = Input.GetAxis("Mouse X") * Time.deltaTime * turnSpeed;
        transform.Rotate(0, turn, 0);

        photonView.RPC("UpdatePlayerPosition", RpcTarget.Others, transform.position, transform.rotation);
    }

    private void InterpolateMovement()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void RaiseHandRPC()
    {
        Debug.Log("Player Raised Hand!");
    }

    [PunRPC]
    public void UpdatePlayerPosition(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
    }
}
