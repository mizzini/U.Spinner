using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerAim : MonoBehaviour
{
    public event Action OnPlayerAim;
    public event Action OnPlayerAimComplete;

    private PlayerInput _playerInput;
    private InputAction _aimAction;

    private bool _isAiming;

    private TargetService _targetService;
    private PlayerVision _playerVision;

    public HashSet<Transform> targetsInRange = new HashSet<Transform>();

    // Throw
    private InputAction _throwAction;

    [SerializeField]
    private Boomerang _boomerang;

    private bool _isThrowing;

    private void Awake()
    {
        // Locate the target service for information on available targets.
        if (GameObject.Find("Target Service") != null)
        {
            _targetService = GameObject.Find("Target Service").GetComponent<TargetService>();
        }
        else
        {
            Debug.LogWarning(this.gameObject.name + " Cannot locate Target Service.");
        }

        // Locate the PlayerInput controller for the Aim action.
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            _aimAction = _playerInput.actions["Aim"];
            _throwAction = _playerInput.actions["Throw"];
        }

        // Locate Boomerang in scene
        GameObject boomerangObject = GameObject.Find("Boomerang(Clone)");
        if (boomerangObject != null)
        {
            _boomerang = GameObject.Find("Boomerang(Clone)").GetComponent<Boomerang>();
        }
        else
        {
            Debug.LogWarning("Unable to find Boomerang in scene.");
        }

        // Locate the PlayerVision component for information on visible targets.
        _playerVision = GetComponent<PlayerVision>();
    }

    private void OnEnable()
    {
        if (_boomerang != null)
        {
            _boomerang.OnRevolutionComplete += _boomerang_OnRevolutionComplete;
        }
    }

    private void _boomerang_OnRevolutionComplete()
    {
        _isThrowing = false;
        // Clear targets in range
        targetsInRange.Clear();
    }

    private void OnDisable()
    {
        if (_boomerang != null)
        {
            _boomerang.OnRevolutionComplete -= _boomerang_OnRevolutionComplete;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Handle user input.
        _aimAction.performed += _ => AimHandler(true);
        // TODO: other cancellation criteria
        _aimAction.canceled += _ => AimHandler(false);

        if (_isAiming) { Aim(); };

        _throwAction.started += _ => ThrowBoomerang();
    }

    private void ThrowBoomerang()
    {
        if (_isThrowing) { return; }
        //Debug.Log("Throw triggered!");
        _isThrowing = true;
        if (_boomerang != null)
        {
            _boomerang.Throw(targetsInRange.ToArray());
        }

        CancelAim();
    }

    private void CancelAim()
    {
        Debug.Log("Aim cancelled.");
        _playerVision.RemoveVisionCone();
        _isAiming = false;
        targetsInRange.Clear();
        OnPlayerAimComplete?.Invoke();
    }

    // Player input handler function.
    private void AimHandler(bool b)
    {
        if (_aimAction == null) { return; }
        if (_isThrowing) { return; }
        _isAiming = b;
        OnPlayerAim?.Invoke();

        // When player cancels aim
        if (!b)
        {
            CancelAim();
        }
    }

    // Aim behavior.
    private void Aim()
    {
        // Can't aim if can't see.
        if (_playerVision == null) { return; }
        //Debug.Log("Player is aiming...");
        // Draw aiming vision cone
        _playerVision.DrawVisionCone();

        // Send targets to boomerang...
        HashSet<Transform> marks = _targetService.GetTargets();
        foreach (Transform t in marks)
        {
            if (t.TryGetComponent<ITargetable>(out ITargetable target))
            {
                if (_playerVision.CheckVisibility(t))
                {
                    // Add to targets in range
                    targetsInRange.Add(t);
                    target.Target();
                }
                //else
                //{
                //    // Remove from targets in range
                //    targetsInRange.Remove(t);
                //    target.UnTarget();
                //}
            }
        }
    }
}
