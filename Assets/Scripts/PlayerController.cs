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

    private Rigidbody selectedRigidbody;
    private Vector3 lastPosition;
    private Vector3 objectVelocity;

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
            if (selectedRigidbody != null)
            {
                objectVelocity = (selectedObject.position - lastPosition) / Time.deltaTime;
                lastPosition = selectedObject.position;
            }
        }

        RaycastOnMouseMove();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TrySelectObject();
        }
        else if (context.canceled)
        {
            isDragging = false;

            if (selectedRigidbody != null)
            {
                selectedRigidbody.linearVelocity = objectVelocity;
            }

            selectedObject = null;
            selectedRigidbody = null;
        }
    }

    #region MouseActionHelperFunction
    private void MoveSelectedObject()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(selectedObject.position);
        mouseScreenPosition.z = objectScreenPosition.z;
        Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition) - offset;
        newWorldPosition.z = selectedObject.position.z;

        selectedObject.position = newWorldPosition;
    }

    private void TrySelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                selectedObject = hit.transform;
                isDragging = true;

                selectedRigidbody = selectedObject.GetComponent<Rigidbody>();
                lastPosition = selectedObject.position;

                Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(selectedObject.position);
                Vector3 mouseScreenPosition = Input.mousePosition;
                offset = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, objectScreenPosition.z)) - selectedObject.position;
            }
        }
    }

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

    private void SetRimTrigger(Renderer renderer, bool state)
    {
        if (renderer != null && renderer.material.HasProperty("_RimTrigger"))
        {
            renderer.material.SetFloat("_RimTrigger", state ? 1f : 0f);
        }
    }

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