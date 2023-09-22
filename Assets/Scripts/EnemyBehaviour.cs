using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState
    {
        PATROL,
        CHASE,
        RESET
    }

    public float m_stopChaseRange = 8f;

    [SerializeField]
    private AgentMovement _agentMovement;

    private EnemyState _currentState = EnemyState.PATROL;

    private bool _playerInSight = false;
    private Transform _playerTransform;

    [Header( "Feedback" )]
    public Light _light;
    public Color _green;
    public Color _red;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch ( _currentState )
        {
            case EnemyState.PATROL:
                _agentMovement.Move();

                // Transition pour aller dans CHASE
                // Voir le joueur

                if( _playerInSight )
                {
                    _currentState = EnemyState.CHASE;
                    _light.color = _red;
                }

                break;
            case EnemyState.CHASE:
                // Transition pour aller dans RESET
                // Perdre le joueur de vue pendant 5 secondes

                _agentMovement.Chase( _playerTransform );

                if ( !_playerInSight  && Vector3.Distance( _playerTransform.position, _agentMovement.GetCurrentWaypoint() ) > m_stopChaseRange )
                {
                    _currentState = EnemyState.PATROL;
                    _light.color = _green;
                }

                break;
            case EnemyState.RESET:
                // Transition pour aller dans PATROL
                // Revenu au waypoint de départ 
                break;
            default:
                break;
        }
    }

    public void PlayerDetected()
    {
        if( _playerTransform == null )
        {
            _playerTransform = GameObject.FindWithTag( "Player" ).transform;
        }

        Ray ray = new Ray( transform.position, _playerTransform.position - transform.position );
        RaycastHit hit;

        if( Physics.Raycast( ray, out hit ) )
        {
            _playerInSight = hit.collider.CompareTag( "Player" );
        }
    }

    public void PlayerLost()
    {
        _playerInSight = false;
    }
}
