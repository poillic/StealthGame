using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    #region Variables

    //Chercher le rigidbody
    [SerializeField] Rigidbody _rigidbody;
    //Chercher InputActionReference
    [SerializeField] InputActionReference PlayerMove;
    //Ajouter un champ Speed
    [SerializeField] float moveSpeed = 2f;

    private Transform CameraTransform;

    #endregion

    #region Unity LifeCycle

    void Awake()
    {
    }

    void Start()
    {
        CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Fonction de d�placement du Rigidbody
        // La direction dans laquelle on va, autrement dit la touche sur laquelle on appuie
        Vector2 moveDirection = PlayerMove.action.ReadValue<Vector2>();

        // On cr�er un nouveau Vector3 pour r�gler le changement de longueur du Vecteur3
        Vector3 CamForward = new Vector3( CameraTransform.forward.x, 0f, CameraTransform.forward.z ).normalized;

        Vector3 ForwardBackward = CamForward * moveDirection.y;
        Vector3 RightLeft = CameraTransform.right * moveDirection.x;
        Vector3 direction = ForwardBackward + RightLeft;

        // La vitesse de d�placement du Player
        //Vector3 playerVelocity = new Vector3( moveDirection.x, 0f, moveDirection.y ) * moveSpeed;
        Vector3 playerVelocity = direction * moveSpeed;

        playerVelocity.y = _rigidbody.velocity.y;
        // La fonction du Rigidbody pour lui appliquer le vecteur de d�placement
        _rigidbody.velocity = playerVelocity;

        if( playerVelocity.magnitude > 0f )
        {
            // On oriente le player dans la direction o� l'on va mais on ne peut plus faire de pas de c�t� ( starfing )
            //transform.forward = playerVelocity;

            // Le player regarde dans la m�me direction que la cam�ra QUE quand il se d�place
            transform.forward = CamForward;
        }

        // Rotation : G�rer par Cin�machine, on le fait plus tard
    }

    #endregion

    #region Main Methods
    #endregion

}
