using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomInfoPanelUpdating : MonoBehaviour
{
    public Text RoomName;
    public Text RoomCurNumber;
    public Text State;
    bool IsStart;
    public RoomUpdate roomUpdate;
    private void Awake()
    {
        roomUpdate = GetComponentInParent<RoomUpdate>();
    }

    public void Set(string _RoomName, string _RoomCurNumber,bool _IsStart)
    {
        RoomName.text = _RoomName;
        RoomCurNumber.text = _RoomCurNumber;
        IsStart = _IsStart;
        //Debug.Log("IsStart" + IsStart);
        //Debug.Log("State" + State.name);
        State.text = (IsStart) ? "Gaming" : "Wait";
    }
    public bool IsSameName(string name)
    {
      return  (name == RoomName.text) ? true : false;
    }
    public void Button()
    {
        if (IsStart)
            return;
        else
            roomUpdate.ToJoin(RoomName.text);
    }
}
