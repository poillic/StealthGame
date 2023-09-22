using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{

    #region Variables

    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;

#endregion

#region Unity LifeCycle

    private void OnTriggerEnter( Collider other )
    {
        if( other.CompareTag("Player") )
        {
            OnPlayerEnter.Invoke();
        }   
    }

    private void OnTriggerExit( Collider other )
    {
        if( other.CompareTag("Player") )
        {
            OnPlayerExit.Invoke();
        }
    }
    #endregion

    #region Main Methods
    #endregion
}
