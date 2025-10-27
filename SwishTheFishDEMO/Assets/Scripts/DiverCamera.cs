using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Microsoft.Surface.NativeWrappers.NativeMethods;
using static UnityEngine.GraphicsBuffer;
using Object = System.Object;

public class DiverCamera : MonoBehaviour
{
    private Camera camera;

    public Camera diverCamera;             // diver's camera
    public Color fallbackColor = Color.blue;
    private Color originalColor;

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



    // Start is called before the first frame update
    void Start()
    {
        TableManager.Instance.OnTouch += OnTouchReceive;

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

        //PrintControllerInputs();
    }

    void PrintControllerInputs()
    {
        for (int i = 1; i <= 4; i++)
        {
            float axis = Input.GetAxis("Axis " + i);
            if (Mathf.Abs(axis) > 0.1f) Debug.Log("Axis " + i + ": " + axis);
        }
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
            Vector3 diverPosition = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, prop.position));
            diverPosition.y = transform.position.y;
            //if (presentDiver) diverPosition.y = transform.position.y;
            //else diverPosition.y -= 2f;

            if ((diverPosition - originalPos).magnitude > 1e-2f)
            {
                diverPosition.y = transform.position.y;
                originalPos = diverPosition;
                transform.position = Vector3.Lerp(transform.position, originalPos, smoothing * Time.deltaTime);
            }
            presentDiver = true;
        }
        else
        {
            presentDiver = false;
        }
    }

    // directional input
    public void DiverInput()
    {
        horizontalInput = Input.GetAxisRaw("4th Axis");
        verticalInput = Input.GetAxisRaw("Vertical");

        float triggerUp = Input.GetAxis("Advance");
        float triggerDown = Input.GetAxis("Return");

        goUp = Input.GetKey(upKey) || triggerUp > 0.1f;
        goDown = Input.GetKey(downKey) || triggerDown > 0.1f;

        // ONLY FOR TESTING
        //presentDiver = Input.GetKey(KeyCode.M);
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

        float xBounded = Mathf.Clamp(targetCamPos.x, -80, 80);
        float yBounded = Mathf.Clamp(targetCamPos.y, -25, 10);
        float zBounded = Mathf.Clamp(targetCamPos.z, -45, 45);
        targetCamPos = new Vector3(xBounded, yBounded, zBounded);

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}