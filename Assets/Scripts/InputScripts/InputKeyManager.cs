using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyInputs", menuName = "Create KeyInputs")]
public class InputKeyManager : ScriptableObject
{
    [Header("Player Control Input")]
    public KeyCode WalkForwardKey = KeyCode.W;
    public KeyCode WalkBackwardKey = KeyCode.S;
    public KeyCode WalkLeftKey = KeyCode.A;
    public KeyCode WalkRightKey = KeyCode.D;
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    [Space]
    public KeyCode AttackKey = KeyCode.Mouse0;
    public KeyCode LookDownSightsKey = KeyCode.Mouse1;
    public KeyCode InteractableKey = KeyCode.E;
}
