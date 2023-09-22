using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{

    #region Variables

    public Transform m_target;

    public Transform[] m_waypoints;

    public enum AgentMode
    {
        START_TO_END,
        END_TO_START,
        PING_PONG
    }

    public AgentMode m_currentMode;

    private int _currentWaypoint = 0;
    private NavMeshAgent _navMeshAgent;

    private bool _ping = true;
    #endregion

#region Unity LifeCycle

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();    
    }

    void Start()
    {
        

        switch ( m_currentMode )
        {
            case AgentMode.START_TO_END:
                _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );
                break;
            case AgentMode.END_TO_START:
                _navMeshAgent.SetDestination( m_waypoints[ m_waypoints.Length - 1 ].position );
                break;
            case AgentMode.PING_PONG:
                _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );
                break;
            default:
                Debug.LogError( "Valeur du Waypoint mode inconnue" );
                break;
        }
    }

    void Update()
    {

    }

#endregion

#region Main Methods

    private void Increment()
    {
        _currentWaypoint++;

        if ( _currentWaypoint > m_waypoints.Length - 1 )
        {
            _currentWaypoint = 0;
        }

        _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );
    }

    private void Decrement()
    {
        _currentWaypoint--;

        if ( _currentWaypoint < 0 )
        {
            _currentWaypoint = m_waypoints.Length - 1;
        }

        _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );
    }

    public void Move()
    {
        //CloseGuard
        if ( _navMeshAgent.remainingDistance >= 1f )
            return;

        switch ( m_currentMode )
        {
            case AgentMode.START_TO_END:
                Increment();

                break;
            case AgentMode.END_TO_START:
                Decrement();

                break;
            case AgentMode.PING_PONG:
                if ( _ping )
                {
                    _currentWaypoint++;

                    if ( _currentWaypoint >= m_waypoints.Length - 1 )
                    {
                        _ping = false;
                    }

                    _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );

                }
                else
                {
                    _currentWaypoint--;

                    if ( _currentWaypoint <= 0 )
                    {
                        _ping = true;
                    }

                    _navMeshAgent.SetDestination( m_waypoints[ _currentWaypoint ].position );
                }
                break;
            default:
                break;
        }
    }

#endregion
}
