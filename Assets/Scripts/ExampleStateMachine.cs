using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExampleStateMachine : MonoBehaviour
{

#region Variables

    enum CubeState
    {
        WHITE,
        GREEN,
        RED,
        PINK
    }

    private CubeState _currentState = CubeState.WHITE;
    private Renderer _renderer;

    public InputAction A_button;
    public InputAction Z_button;
    public InputAction ShiftA;

#endregion

#region Unity LifeCycle

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        A_button.Enable();
        Z_button.Enable();
        ShiftA.Enable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        OnStateUpdate();
    }

#endregion

#region Main Methods

    void OnStateEnter()
    {
        switch ( _currentState )
        {
            case CubeState.WHITE:
                _renderer.material.color = Color.white;
                break;
            case CubeState.GREEN:
                _renderer.material.color = Color.green;
                break;
            case CubeState.RED:
                _renderer.material.color = Color.red;
                break;
            case CubeState.PINK:
                _renderer.material.color = new Color(1f, .4f, .4f);
                break;
            default:
                break;
        }
    }

    void OnStateUpdate()
    {
        switch ( _currentState )
        {
            case CubeState.WHITE:

                if( A_button.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.GREEN );
                }
                else if ( Z_button.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.RED );
                }

                break;
            case CubeState.GREEN:

                if ( A_button.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.WHITE );
                }

                break;
            case CubeState.RED:

                if ( Z_button.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.PINK );
                }

                break;
            case CubeState.PINK:

                if ( Z_button.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.WHITE );
                }
                else if( ShiftA.WasPerformedThisFrame() )
                {
                    TransitionToState( CubeState.RED );
                }

                break;
            default:
                break;
        }
    }

    void OnStateExit()
    {
        switch ( _currentState )
        {
            case CubeState.WHITE:
                break;
            case CubeState.GREEN:
                break;
            case CubeState.RED:
                break;
            case CubeState.PINK:
                break;
            default:
                break;
        }
    }

    void TransitionToState( CubeState nextState )
    {
        OnStateExit();
        _currentState = nextState;
        OnStateEnter();
    }

#endregion
}
