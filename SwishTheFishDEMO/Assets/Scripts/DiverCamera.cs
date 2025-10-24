using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Microsoft.Surface.NativeWrappers.NativeMethods;
using static UnityEngine.GraphicsBuffer;

public class DiverCamera : MonoBehaviour
{
    private Camera camera;

    public Camera diverCamera;             // diver's camera
    public Camera placeholderCamera;          // in worst case, have another camera
    public GameObject placeholderImage;    // Canvas image, plane or anything

    public float moveSpeed = 7f;
    public float rotationSpeed = 11f;
    public float smoothing = 5f;        // The speed with which the camera will be following.
    public Vector3 movement = new Vector3(0, 0, 0);
    public float yRotation = 0;

    public KeyCode upKey = KeyCode.C;
    public KeyCode downKey = KeyCode.LeftShift;
    public float horizontalInput;
    public float verticalInput;
    public bool goUp = false;
    public bool goDown = false;

    public Vector3 originalPos;
    public float maxDistance = 20f;

    ObjectInput diverProp;
    public bool presentDiver = false;

    public Color fallbackColor = Color.blue;
    private Color originalColor;


    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        originalPos = transform.position;
        originalColor = diverCamera.backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {
        // to test, change how we detect diver, like any button at a static location
        ShowImage();
        DiverInput();
        MoveCamera();
    }
    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
        AddDiver(diverProp);
    }

    // we receive the information of the fish here
    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            diverProp = objectInputs.TryGetValue(TableManager.DiverId, out diverProp) ? diverProp : null;
        }
    }

    // we show either the inner camera footage, either the other placeholder image
    public void ShowImage()
    {
        // we receive no signal, we show the placeholder image
        if (diverProp == null && !presentDiver)
        {
            diverCamera.clearFlags = CameraClearFlags.SolidColor;
            diverCamera.backgroundColor = fallbackColor;
            diverCamera.cullingMask = 0;

        }
        // we're receiving a signal, we enable the camera and turn off the placeholder
        else
        {
            diverCamera.clearFlags = CameraClearFlags.Skybox;
            diverCamera.backgroundColor = originalColor;
            diverCamera.cullingMask = ~0;
        }
    }

    // we check if we add the diver at every frame
    public void AddDiver(ObjectInput prop)
    {
        if (prop != null && prop.tagValue == TableManager.DiverId)
        {
            Debug.Log("we received the signal for the diver, we add it");
            Vector3 diverPosition = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, prop.position));

            if (presentDiver) diverPosition.y = transform.position.y;
            else diverPosition.y -= 2f;

            originalPos = diverPosition;
            presentDiver = true;
            Debug.Log("the diver's original position is " + originalPos);
        }
        else
        {
            //presentDiver = false;
        }
    }

    // directional input
    public void DiverInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        goUp = Input.GetKey(upKey);
        goDown = Input.GetKey(downKey);

        // ONLY FOR TESTING
        presentDiver = Input.GetKey(KeyCode.M);
        //if (presentDiver) diverProp = new ObjectInput();
    }

    // we apply the input's movement to the camera
    public void MoveCamera()
    {
        Vector3 movementXZ = transform.forward * verticalInput;
        movementXZ = movementXZ.normalized * moveSpeed;

        Vector3 movementY = Vector3.zero;
        if (goUp != goDown)
        {
            if (goUp)
            {
                movementY = transform.up.normalized * moveSpeed;
            }
            else
            {
                movementY = transform.up.normalized * -1f * moveSpeed;
            }
        }

        Vector3 targetCamPos = transform.position + movementXZ + movementY;
        Vector3 centerToTarget = targetCamPos - originalPos;
        float distToTarget = centerToTarget.magnitude;

        if (distToTarget > maxDistance)
        {
            Vector3 centerToCam = transform.position - originalPos;
            float distToEdge = maxDistance - centerToCam.magnitude;
            targetCamPos = originalPos + centerToTarget.normalized * maxDistance;
        }

        float turnY = horizontalInput * rotationSpeed * Time.deltaTime;
        yRotation += turnY;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        float xBounded = Mathf.Clamp(targetCamPos.x, -60, 60);
        float yBounded = Mathf.Clamp(targetCamPos.y, -25, 10);
        float zBounded = Mathf.Clamp(targetCamPos.z, -35, 35);
        targetCamPos = new Vector3(xBounded, yBounded, zBounded);

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}