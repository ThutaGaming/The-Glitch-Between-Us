using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Presses "R" for the player automatically the instant the equipped weapon's magazine hits
/// empty. Character.OnTryPlayReload needs a real InputAction.CallbackContext, which can't be
/// constructed directly from outside code, so this feeds an actual Keyboard state event through
/// the Input System - the same pipeline a physical key press uses - rather than reaching into
/// Infima's private reload methods.
/// </summary>
public class AutoReload : MonoBehaviour
{
    [Tooltip("Small delay after the magazine empties before the reload key is pressed, so the empty-magazine feedback (sound/animation) is felt first.")]
    [SerializeField] private float delayAfterEmpty = 0.15f;

    private IGameModeService gameModeService;
    private CharacterBehaviour playerCharacter;
    private InventoryBehaviour playerCharacterInventory;

    private bool triggered;
    private float emptyTimer;

    private void Awake()
    {
        gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        playerCharacter = gameModeService != null ? gameModeService.GetPlayerCharacter() : null;
        playerCharacterInventory = playerCharacter != null ? playerCharacter.GetInventory() : null;
    }

    private void Update()
    {
        if (playerCharacterInventory == null) return;

        WeaponBehaviour equipped = playerCharacterInventory.GetEquipped();
        if (equipped == null || equipped.GetAmmunitionCurrent() > 0)
        {
            triggered = false;
            emptyTimer = 0f;
            return;
        }

        if (triggered) return;

        emptyTimer += Time.deltaTime;
        if (emptyTimer < delayAfterEmpty) return;

        triggered = true;
        StartCoroutine(PressReloadKey());
    }

    private IEnumerator PressReloadKey()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) yield break;

        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.R));
        InputSystem.Update();

        yield return null;

        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        InputSystem.Update();
    }
}
