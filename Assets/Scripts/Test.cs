using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    #region Variables

    public GameObject _sphere;
    public Vector3 _offset;
    #endregion

    #region Unity LifeCycle

    private void Reset()
    {        
        
    }
    void Awake()
    {

            GameObject go = new GameObject();
            go.name = "GroundCheck";
            go.transform.parent = transform;

            _sphere = go;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 origin = transform.up * _offset.y + transform.right * _offset.x + transform.forward * _offset.z;
        Debug.DrawRay( transform.position + origin, -transform.up );
    }

#endregion

#region Main Methods
#endregion
}
