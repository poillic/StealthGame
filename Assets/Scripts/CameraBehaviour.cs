using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    #region Variables

    public Transform m_StartTarget;
    public Transform m_EndTarget;

    public enum CameraState
    {
        PATROL, PURSUIT, RESET
    }

    public CameraState _currentState;

    public float m_travelDuration = 3f;
    public float m_playerEvasionTime = 5f;
    private float _chrono = 0f;

    //Quand ping est TRUE on va du début vers la fin, quand il est FALSE c'est de la fin vers le début
    private bool _ping = true;

    private bool _playerSpotted = false;
    private Transform _playerTransform;
    
#endregion

#region Unity LifeCycle

    void Awake()
    {
        
    }

    void Start()
    {
        transform.LookAt( m_StartTarget );
    }

    void Update()
    {
        switch ( _currentState )
        {
            case CameraState.PATROL:
                Patrol();

                if( _playerSpotted )
                {
                    _chrono = 0f;
                    _currentState = CameraState.PURSUIT;
                }
                break;
            case CameraState.PURSUIT:

                if( !_playerSpotted )
                {
                    _chrono += Time.deltaTime;

                    if( _chrono >= m_playerEvasionTime )
                    {
                        _currentState = CameraState.RESET;
                    }
                }
                else
                {
                    transform.LookAt( _playerTransform.position );
                    _chrono = 0f;
                }
                break;

            case CameraState.RESET:
                Vector3 dir = transform.position - m_StartTarget.position;
                Quaternion targetRotation = Quaternion.Euler( dir );
                transform.rotation = Quaternion.RotateTowards( transform.rotation, targetRotation, 0.1f );

                if( transform.rotation == targetRotation )
                {
                    _chrono = 0f;
                    _currentState = CameraState.PATROL;
                }
                break;
            default:
                break;
        }
    }

    public void FollowPlayer()
    {
        _playerSpotted = true;

        if( _playerTransform == null )
        {
            _playerTransform = GameObject.FindGameObjectWithTag( "Player" ).transform;
        }
    }

    public void PlayerLost()
    {
        _playerSpotted = false;
    }

    #endregion

    #region Main Methods

    private void Patrol()
    {
        // Interpolation linéaire entre la position de début et la position de fin, le denier paramètre est le pourcentage de progression de
        // l'interpolation. A 0 on est au début, à 1 on est à la fin, à 0.5 on est au milieu
        Vector3 lookAtPosition = Vector3.Lerp( m_StartTarget.position, m_EndTarget.position, _chrono / m_travelDuration );

        if ( _ping )
        {
            _chrono += Time.deltaTime;

            // Si la valeur du chrono est supérieure à la durée du trajet c'est que le trajet est terminé, donc on fait demi-tour
            if ( _chrono >= m_travelDuration )
            {
                _ping = false;
            }
        }
        else
        {
            _chrono -= Time.deltaTime;

            // Si chrono est plus petit que 0 c'est qu'on a fini de revenir
            if ( _chrono < 0f )
            {
                _ping = true;
            }
        }

        transform.LookAt( lookAtPosition );
    }

#endregion
}
