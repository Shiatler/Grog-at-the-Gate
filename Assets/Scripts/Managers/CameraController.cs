
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Pan Settings")]
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;
    public float dragPanSpeed = 20f;
    public float movementSmoothing = 10f;

    [Header("Zoom Settings")]
    public float scrollSpeed = 5f;
    public float minY = 10f;
    public float maxY = 80f;
    public float zoomSmoothing = 10f;

    [Header("Border Limits")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 200f;
    public float maxRotationX = 80f;
    public float minRotationX = 10f;

    [Header("Human POV Settings")]
    public float humanPOVTransitionHeight = 12f;
    public float humanPOVFOV = 60f;
    public float rtsFOV = 50f;
    public float povTransitionSmoothing = 5f;
    public float zoomRestoreSmoothing = 8f;

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private bool isRotating = false;
    private Camera cam;
    private float currentRotationX = 45f;
    private float currentRotationY = 0f;
    private bool isHumanPOV = false;
    private Vector3 targetPosition;
    private float targetY;
    private float targetFOV;
    private float targetRotationX;
    private Vector3 groundLookAtPoint;
    private Vector3 savedPositionBeforeZoomIn;
    private float savedRotationXBeforeZoomIn;
    private float savedRotationYBeforeZoomIn;
    private bool hasSavedPosition = false;
    private bool isRestoringPosition = false;
    private float restoreTimer = 0f;
    private const float maxRestoreTime = 2f; // Maximum time to restore before forcing completion

    // START #########################################################

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
        
        lastMousePosition = Input.mousePosition;
        currentRotationX = transform.eulerAngles.x;
        currentRotationY = transform.eulerAngles.y;
        targetPosition = transform.position;
        targetY = transform.position.y;
        targetFOV = cam != null ? cam.fieldOfView : rtsFOV;
        targetRotationX = currentRotationX;
    }

    // UPDATE #########################################################

    void Update ()
    {
        if (GameManager.GameIsOver)
        {
            this.enabled = false;
            return;
        }
        

        HandleMouseDrag();
        HandleEdgePanning();
        HandleZoom();
        HandleRotation();
        HandleRestoreTransition();
        UpdatePOVTransition();
        ApplyBorders();
    }

    // HANDLE MOUSE DRAG #########################################################

    void HandleMouseDrag()
    {
        // Reset flags if mouse button is not held (safety check)
        if (!Input.GetMouseButton(2))
        {
            if (isDragging || isRotating)
            {
                isDragging = false;
                isRotating = false;
            }
        }
        
        if (Input.GetMouseButtonDown(2))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
                // Cancel restoration when user starts dragging
                if (isRestoringPosition)
                {
                    isRestoringPosition = false;
                    restoreTimer = 0f;
                }
            }
            else
            {
                isRotating = true;
                // Cancel restoration when user starts rotating
                if (isRestoringPosition)
                {
                    isRestoringPosition = false;
                    restoreTimer = 0f;
                }
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
            isRotating = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x * dragPanSpeed * Time.deltaTime, 0, -delta.y * dragPanSpeed * Time.deltaTime);
            
            Vector3 newPos = targetPosition;
            newPos += transform.right * move.x;
            newPos += transform.forward * move.z;
            
            // Enforce borders
            targetPosition = EnforceBorders(newPos);
            lastMousePosition = Input.mousePosition;
        }
    }

    // HANDLE EDGE PANNING #########################################################

    void HandleEdgePanning()
    {
        // Disable edge panning when rotating
        if (isRotating)
            return;
        
        // Cancel restoration if user is actively panning
        if (isRestoringPosition)
        {
            isRestoringPosition = false;
            restoreTimer = 0f;
        }

        float moveAmount = panSpeed * Time.deltaTime;
        Vector3 moveDirection = Vector3.zero;

        // Get camera's forward and right directions (relative to camera rotation)
        Vector3 cameraForward = transform.forward;
        Vector3 cameraRight = transform.right;
        
        // Flatten to horizontal plane (remove Y component)
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // W = forward relative to camera
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            moveDirection += cameraForward;
        }
        // S = back relative to camera
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            moveDirection -= cameraForward;
        }
        // A = left relative to camera
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            moveDirection -= cameraRight;
        }
        // D = right relative to camera
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            moveDirection += cameraRight;
        }

        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            Vector3 newPos = targetPosition + moveDirection * moveAmount;
            
            // Enforce borders AFTER movement - this allows smooth movement up to the border
            targetPosition = EnforceBorders(newPos);
        }
    }

    // HANDLE ZOOM #########################################################

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0)
        {
            // Cancel restoration if user is actively zooming (they're in control)
            if (isRestoringPosition && Mathf.Abs(scroll) > 0.01f)
            {
                isRestoringPosition = false;
                restoreTimer = 0f;
            }
            
            float oldY = targetY;
            bool wasInHumanPOV = oldY <= humanPOVTransitionHeight;
            
            targetY -= scroll * 1000 * scrollSpeed * Time.deltaTime;
            targetY = Mathf.Clamp(targetY, minY, maxY);
            
            bool isNowInHumanPOV = targetY <= humanPOVTransitionHeight;
            
            // If zooming OUT from human POV (crossing threshold upward), start smooth restoration
            if (wasInHumanPOV && !isNowInHumanPOV && hasSavedPosition && !isDragging && !isRotating)
            {
                isRestoringPosition = true;
                restoreTimer = 0f; // Reset timer when starting restoration
            }
            // When zooming in or out (but not crossing threshold), maintain X/Z position
            // X/Z should stay the same during zoom - borders will handle clamping
        }
    }
    
    // CALCULATE GROUND LOOK AT POINT #########################################################

    void CalculateGroundLookAtPoint()
    {
        // Calculate the point on the ground plane (Y=0) that the camera is looking at
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = new Ray(transform.position, transform.forward);
        
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            groundLookAtPoint = ray.GetPoint(distance);
        }
        else
        {
            // Fallback: calculate based on camera angle
            float height = transform.position.y;
            float angle = currentRotationX * Mathf.Deg2Rad;
            float dist = height / Mathf.Tan(angle);
            Vector3 forwardDir = Quaternion.Euler(0, currentRotationY, 0) * Vector3.forward;
            groundLookAtPoint = transform.position + forwardDir * dist;
            groundLookAtPoint.y = 0f;
        }
    }
    
    // MAINTAIN GROUND POSITION #########################################################

    void MaintainGroundPosition()
    {
        // Calculate new camera position to maintain the same ground look-at point
        float height = targetY;
        float angle = currentRotationX * Mathf.Deg2Rad;
        
        if (angle > 0.01f)
        {
            float distance = height / Mathf.Tan(angle);
            Vector3 forwardDir = Quaternion.Euler(0, currentRotationY, 0) * Vector3.forward;
            Vector3 newPos = groundLookAtPoint - forwardDir * distance;
            newPos.y = height;
            
            // Only update if within borders
            if (newPos.x >= minX && newPos.x <= maxX && newPos.z >= minZ && newPos.z <= maxZ)
            {
                targetPosition = newPos;
            }
        }
    }

    // HANDLE ROTATION #########################################################

    void HandleRotation()
    {
        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentRotationY += mouseX;
            currentRotationX -= mouseY;
            currentRotationX = Mathf.Clamp(currentRotationX, minRotationX, maxRotationX);
            targetRotationX = currentRotationX;

            transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
            
            // Don't maintain ground position during rotation - let user move freely
            // This allows full range of movement within borders
        }
    }

    // HANDLE RESTORE TRANSITION #########################################################

    void HandleRestoreTransition()
    {
        if (isRestoringPosition && hasSavedPosition)
        {
            restoreTimer += Time.deltaTime;
            
            // Smoothly lerp position back to saved position
            targetPosition.x = Mathf.Lerp(targetPosition.x, savedPositionBeforeZoomIn.x, zoomRestoreSmoothing * Time.deltaTime);
            targetPosition.z = Mathf.Lerp(targetPosition.z, savedPositionBeforeZoomIn.z, zoomRestoreSmoothing * Time.deltaTime);
            
            // Smoothly lerp rotation back to saved rotation
            currentRotationX = Mathf.Lerp(currentRotationX, savedRotationXBeforeZoomIn, zoomRestoreSmoothing * Time.deltaTime);
            currentRotationY = Mathf.Lerp(currentRotationY, savedRotationYBeforeZoomIn, zoomRestoreSmoothing * Time.deltaTime);
            targetRotationX = Mathf.Lerp(targetRotationX, savedRotationXBeforeZoomIn, zoomRestoreSmoothing * Time.deltaTime);
            
            // Check if we're close enough to stop restoring
            float posDistance = Vector3.Distance(new Vector3(targetPosition.x, 0, targetPosition.z), new Vector3(savedPositionBeforeZoomIn.x, 0, savedPositionBeforeZoomIn.z));
            float rotDistance = Mathf.Abs(currentRotationX - savedRotationXBeforeZoomIn) + Mathf.Abs(currentRotationY - savedRotationYBeforeZoomIn);
            
            // Force completion if close enough OR if timeout reached (prevents getting stuck)
            if (posDistance < 0.01f && rotDistance < 0.1f || restoreTimer >= maxRestoreTime)
            {
                // Snap to exact values and stop restoring
                targetPosition.x = savedPositionBeforeZoomIn.x;
                targetPosition.z = savedPositionBeforeZoomIn.z;
                currentRotationX = savedRotationXBeforeZoomIn;
                currentRotationY = savedRotationYBeforeZoomIn;
                targetRotationX = savedRotationXBeforeZoomIn;
                isRestoringPosition = false;
                restoreTimer = 0f;
            }
        }
        else
        {
            // Reset timer when not restoring
            restoreTimer = 0f;
        }
    }

    // UPDATE POV TRANSITION #########################################################

    void UpdatePOVTransition()
    {
        float currentHeight = targetY;
        bool shouldBeHumanPOV = currentHeight <= humanPOVTransitionHeight;

        if (shouldBeHumanPOV)
        {
            if (!isHumanPOV)
            {
                // Store position and rotation when entering human POV (zooming in)
                savedPositionBeforeZoomIn = targetPosition;
                savedRotationXBeforeZoomIn = currentRotationX;
                savedRotationYBeforeZoomIn = currentRotationY;
                hasSavedPosition = true;
                isHumanPOV = true;
            }
            targetFOV = humanPOVFOV;
            targetRotationX = Mathf.Clamp(targetRotationX, 0f, 30f);
        }
        else
        {
            if (isHumanPOV)
            {
                // Restore position when leaving human POV (zooming out)
                if (hasSavedPosition)
                {
                    targetPosition.x = savedPositionBeforeZoomIn.x;
                    targetPosition.z = savedPositionBeforeZoomIn.z;
                }
                isHumanPOV = false;
            }
            targetFOV = rtsFOV;
        }

        // Smooth FOV transition
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, povTransitionSmoothing * Time.deltaTime);
        }

        // Smooth rotation transition
        currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, povTransitionSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
    }

    Vector3 EnforceBorders(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        return pos;
    }

    // APPLY BORDERS #########################################################

    void ApplyBorders()
    {
        // Ensure targetPosition is always valid (safety check)
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.z))
        {
            targetPosition = transform.position;
        }
        
        // Set Y first
        targetPosition.y = targetY;
        
        // Ensure targetY is valid
        if (float.IsNaN(targetY))
        {
            targetY = transform.position.y;
        }
        
        // ENFORCE BORDERS ON TARGET POSITION
        targetPosition = EnforceBorders(targetPosition);

        // Get current position and apply X/Z immediately (no smoothing)
        Vector3 currentPos = transform.position;
        
        // Safety check for current position
        if (float.IsNaN(currentPos.x) || float.IsNaN(currentPos.z))
        {
            currentPos = targetPosition;
        }
        
        currentPos.x = targetPosition.x;
        currentPos.z = targetPosition.z;
        
        // Smooth Y only
        currentPos.y = Mathf.Lerp(currentPos.y, targetPosition.y, movementSmoothing * Time.deltaTime);
        
        // Ensure Y is valid before final border enforcement
        if (float.IsNaN(currentPos.y))
        {
            currentPos.y = targetPosition.y;
        }
        
        // FINAL BORDER ENFORCEMENT before setting position
        currentPos = EnforceBorders(currentPos);
        
        // SET POSITION - this is the ONLY place transform.position is set
        transform.position = currentPos;
    }
    
    // ON APPLICATION FOCUS #########################################################
    
    void OnApplicationFocus(bool hasFocus)
    {
        // Reset input states when application loses/gains focus to prevent stuck states
        if (!hasFocus)
        {
            isDragging = false;
            isRotating = false;
        }
    }
}
