using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCS : MonoBehaviour {

    [PunRPC]
    public void ChangeColor(float[] color)
    {
        GetComponent<Renderer>().material.color = new Color(color[0], color[1], color[2]);

    }

    [PunRPC]
    public void ChildrenRendererToggle()
    {
        //자식 객체의 렌더러를 토글한다.

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = !r.enabled;
    }
}
