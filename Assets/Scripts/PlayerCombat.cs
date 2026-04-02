using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private PlayerInput playerInput;

    void Start()
    {
        playerInput.actions["Block"].started += ctx => OnBlockStarted();
        playerInput.actions["Block"].canceled += ctx => OnBlockCanceled();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAttack()
    {

    }

    void OnBlockStarted()
    {

    }

    void OnBlockCanceled()
    {

    }

    void OnBoomerang()
    {

    }

}
