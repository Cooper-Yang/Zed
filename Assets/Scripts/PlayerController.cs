using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera mainCamera;
    private Transform selectedObject;
    private bool isDragging = false;
    private Vector3 offset;
    private Transform lastSelection;
    
   
    void Awake()
    {
        playerInput = new PlayerInput();
        mainCamera = Camera.main;
    }


    void Update()
    {
        if (isDragging && selectedObject != null)
        {
            MoveSelectedObject();
        }

        RaycastOnMouseMove();
    }

    /// <summary>
    /// Enables player input actions.
    /// </summary>
    private void OnEnable()
    {
        playerInput.Enable();
    }

    /// <summary>
    /// Disables player input actions.
    /// </summary>
    private void OnDisable()
    {
        playerInput.Disable();
    }

    /// <summary>
    /// Handles interaction events, allowing the player to grab and release objects.
    /// </summary>
    /// <param name="context">Input action callback context.</param>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TrySelectObject();
        }
        else if (context.canceled)
        {
            isDragging = false;
            selectedObject = null;
        }
    }

    #region MouseActionHelperFunction

    /// <summary>
    /// Moves the selected object in the camera's XY plane based on mouse position.
    /// </summary>
    private void MoveSelectedObject()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(selectedObject.position);
        mouseScreenPosition.z = objectScreenPosition.z;
        Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition) - offset;
        newWorldPosition.z = selectedObject.position.z;

        selectedObject.position = newWorldPosition;
    }

    /// <summary>
    /// Attempts to select an interactable object under the mouse cursor.
    /// </summary>
    private void TrySelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                selectedObject = hit.transform;
                isDragging = true;
                Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(selectedObject.position);
                Vector3 mouseScreenPosition = Input.mousePosition;
                offset = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, objectScreenPosition.z)) - selectedObject.position;
            }
        }
    }

    /// <summary>
    /// Handles raycasting on mouse movement and manages object highlighting.
    /// </summary>
    private void RaycastOnMouseMove()
    {
        if (isDragging && selectedObject != null && lastSelection == selectedObject)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Interactable"))
        {
            Transform selection = hit.transform;
            Renderer renderer = selection.GetComponent<Renderer>();

            if (renderer != null && renderer.material.HasProperty("_RimTrigger"))
            {
                SetRimTrigger(renderer, true);
                lastSelection = selection;
            }
        }
        else
        {
            ResetLastSelection();
        }
    }

    /// <summary>
    /// Sets the "_RimTrigger" property on the object's material.
    /// </summary>
    /// <param name="renderer">Renderer of the target object.</param>
    /// <param name="state">Boolean value indicating whether to enable or disable the property.</param>
    private void SetRimTrigger(Renderer renderer, bool state)
    {
        if (renderer != null && renderer.material.HasProperty("_RimTrigger"))
        {
            renderer.material.SetFloat("_RimTrigger", state ? 1f : 0f);
        }
    }

    /// <summary>
    /// Resets the highlighting on the last selected object.
    /// </summary>
    private void ResetLastSelection()
    {
        if (isDragging && selectedObject != null && lastSelection == selectedObject)
        {
            return;
        }
        if (lastSelection != null)
        {
            Renderer renderer = lastSelection.GetComponent<Renderer>();
            SetRimTrigger(renderer, false);
            lastSelection = null;
        }
    }
    #endregion
}