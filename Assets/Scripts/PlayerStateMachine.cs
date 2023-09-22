using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

#region Variables

    enum PlayerState
    {
        IDLE,
        WALK,
        RUN,
        CROUCH,
        FALL,
        JUMP,
        DEATH
    };

    private PlayerState _currentState = PlayerState.IDLE;
    private Rigidbody _rigidbody;
    private Vector2 _moveDirection = Vector2.zero;
    private float _currentSpeed = 0f;
    private Transform cameraTransform;
    private bool _isGrounded = false;
    public LayerMask m_GroundLayer;
    private bool _mustJump = false;
    private CapsuleCollider _currentCollider;

    [Header( "Colliders" )]
    public CapsuleCollider m_walkCollider;
    public CapsuleCollider m_crouchCollider;

    [Header( "Speeds" )]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public float crouchSpeed = 2f;

    [Header( "Input Actions" )]
    public InputActionReference MoveAction;
    public InputActionReference JumpAction;
    public InputActionReference CrouchAction;
    public InputActionReference RunAction;

    [Header( "Rigibody Parameters" )]
    public float m_distanceFromGround = 1f;
    public float m_raycastGroundPositionner = 1f;
    public Vector3 m_groundCheckerDimension = new Vector3( .2f, .2f, .2f );
    public float m_distanceGroundCheckerToTransform = 0.8f;

    [Header( "Debug Mode" )]
    public bool m_activateGizmos = true;

#endregion

#region Unity LifeCycle

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
        m_crouchCollider.enabled = false;
    }

    void Update()
    {
        OnStateUpdate();

        Collider[] colliders = Physics.OverlapBox( transform.position - transform.up * m_distanceGroundCheckerToTransform, m_groundCheckerDimension, Quaternion.identity, m_GroundLayer );

        _isGrounded = colliders.Length > 0;
        _rigidbody.useGravity = !_isGrounded;
        // Si _isGrounded est TRUE la valeur de useGravity est l'inverse de useGravity
    }
    private void FixedUpdate()
    {
        Vector3 CamForward = new Vector3( cameraTransform.forward.x, 0f, cameraTransform.forward.z );
        Vector3 ForwardBackward = CamForward * _moveDirection.y;
        Vector3 RightLeft = cameraTransform.right * _moveDirection.x;

        Vector3 direction = ForwardBackward + RightLeft;

        Vector3 playerVelocity = direction * _currentSpeed;

        if ( _moveDirection != Vector2.zero )
        {
            transform.forward = playerVelocity;
        }

        if ( _isGrounded )
        {
            playerVelocity.y = 0f;
        }
        else
        {
            playerVelocity.y = _rigidbody.velocity.y;
        }

        if ( _mustJump )
        {
            playerVelocity.y = jumpForce;
            _mustJump = false;
        }

        _rigidbody.velocity = playerVelocity;

        

        RaycastHit hit;
        
        if( Physics.Raycast( transform.position, -transform.up, out hit, m_raycastGroundPositionner, m_GroundLayer ) )
        {
            Vector3 position = hit.point;
            position.y += m_distanceFromGround;
            _rigidbody.MovePosition( position );
        }
    }


    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        GUI.Label( new Rect( 10, 10, 150, 80 ), _currentState.ToString(), style );
        GUI.Label( new Rect( 10, 90, 150, 80 ), "Grounded : " + _isGrounded.ToString(), style );

    }

    private void OnDrawGizmos()
    {
        if( m_activateGizmos )
        {
            if( _isGrounded )
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube( transform.position - transform.up * m_distanceGroundCheckerToTransform, m_groundCheckerDimension );
            Gizmos.color = Color.green;
            Gizmos.DrawRay( transform.position, -transform.up * m_raycastGroundPositionner );
        }
    }

    #endregion

    #region Main Methods

    void OnStateEnter()
    {
        switch ( _currentState )
        {
            case PlayerState.IDLE:
                _currentSpeed = 0f;
                break;
            case PlayerState.WALK:
                _currentSpeed = walkSpeed;
                break;
            case PlayerState.RUN:
                _currentSpeed = runSpeed;
                break;
            case PlayerState.CROUCH:
                m_walkCollider.enabled = false;
                m_crouchCollider.enabled = true;
                _currentSpeed = crouchSpeed;
                break;
            case PlayerState.FALL:
                break;
            case PlayerState.JUMP:
                _mustJump = true;
                break;
            case PlayerState.DEATH:
                break;
            default:
                break;
        }
    }

    void OnStateUpdate()
    {
        switch ( _currentState )
        {
            case PlayerState.IDLE:

                if ( MoveAction.action.WasPerformedThisFrame() && RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.RUN );
                }
                else if ( MoveAction.action.WasPerformedThisFrame() && !RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.WALK );
                }
                else if ( JumpAction.action.WasPerformedThisFrame() && _isGrounded )
                {
                    TransitionToState( PlayerState.JUMP );
                } else if ( CrouchAction.action.WasPerformedThisFrame() )
                {
                    TransitionToState( PlayerState.CROUCH );
                }

                break;
            case PlayerState.WALK:

                _moveDirection = MoveAction.action.ReadValue<Vector2>();

                if( MoveAction.action.WasReleasedThisFrame() )
                {
                    TransitionToState( PlayerState.IDLE );
                }else if( RunAction.action.WasPerformedThisFrame() )
                {
                    TransitionToState( PlayerState.RUN );
                }
                else if ( JumpAction.action.WasPerformedThisFrame() && _isGrounded )
                {
                    TransitionToState( PlayerState.JUMP );
                }
                else if ( CrouchAction.action.WasPerformedThisFrame() )
                {
                    TransitionToState( PlayerState.CROUCH );
                }

                break;
            case PlayerState.RUN:

                _moveDirection = MoveAction.action.ReadValue<Vector2>();

                if ( RunAction.action.WasReleasedThisFrame() && MoveAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.WALK );
                }
                else if( !MoveAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.IDLE );
                }
                else if ( JumpAction.action.WasPerformedThisFrame() && _isGrounded )
                {
                    TransitionToState( PlayerState.JUMP );
                }
                else if ( CrouchAction.action.WasPerformedThisFrame() )
                {
                    TransitionToState( PlayerState.CROUCH );
                }

                break;
            case PlayerState.CROUCH:

                _moveDirection = MoveAction.action.ReadValue<Vector2>();

                if( CrouchAction.action.WasPerformedThisFrame() && MoveAction.action.IsPressed() && RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.RUN );
                }
                else if ( CrouchAction.action.WasPerformedThisFrame() && MoveAction.action.IsPressed() && !RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.WALK );
                }
                else if ( CrouchAction.action.WasPerformedThisFrame() && !MoveAction.action.IsPressed() && !RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.IDLE );
                }
                else if ( JumpAction.action.WasPerformedThisFrame() && _isGrounded )
                {
                    TransitionToState( PlayerState.JUMP );
                }

                break;
            case PlayerState.FALL:

                if( _isGrounded && MoveAction.action.IsPressed() && RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.RUN );
                }
                else if( _isGrounded && MoveAction.action.IsPressed() && !RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.WALK );
                }
                else if ( _isGrounded && !MoveAction.action.IsPressed() && !RunAction.action.IsPressed() )
                {
                    TransitionToState( PlayerState.IDLE );
                }

                break;
            case PlayerState.JUMP:

                _moveDirection = MoveAction.action.ReadValue<Vector2>();

                if ( _rigidbody.velocity.y < 0f && !_isGrounded )
                {
                    TransitionToState( PlayerState.FALL );
                }

                break;
            case PlayerState.DEATH:
                break;
            default:
                break;
        }
    }

    void OnStateExit()
    {
        switch ( _currentState )
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.WALK:
                break;
            case PlayerState.RUN:
                break;
            case PlayerState.CROUCH:
                m_walkCollider.enabled = true;
                m_crouchCollider.enabled = false;
                break;
            case PlayerState.FALL:
                break;
            case PlayerState.JUMP:
                break;
            case PlayerState.DEATH:
                break;
            default:
                break;
        }
    }

    void TransitionToState( PlayerState nextState )
    {
        OnStateExit();
        _currentState = nextState;
        OnStateEnter();
    }
    #endregion
}
