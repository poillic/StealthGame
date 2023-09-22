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

    [SerializeField]
    private AgentMovement _agentMovement;

    private EnemyState _currentState = EnemyState.PATROL;

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
                break;
            case EnemyState.CHASE:
                // Transition pour aller dans RESET
                // Perdre le joueur de vue pendant 5 secondes
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

    }

    public void PlayerLost()
    {

    }
}
