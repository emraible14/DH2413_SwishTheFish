using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static Microsoft.Surface.NativeWrappers.NativeMethods;
using static UnityEngine.GraphicsBuffer;
using Object = System.Object;

public class DiverCamera : MonoBehaviour
{
    [Header("Cameras Data")]
    public Camera diverCamera;
    public Camera sceneCamera;
    private Camera camera;

    [Header("Movement Data")]
    public float moveSpeed = 7f;
    public float rotationSpeed = 30f;
    public float smoothing = 5f;
    public float maxDistance = 20f;
    private float yRotation = 0;

    [Header("Input Data")]
    public KeyCode upKey = KeyCode.C;
    public KeyCode downKey = KeyCode.LeftShift;
    private float horizontalInput;
    private float verticalInput;
    private bool goUp = false;
    private bool goDown = false;

    [Header("Two Cameras Data")]
    private Vector3 phPos;
    private Vector3 phRot;
    private float phFOV;
    private Vector3 diverPos;
    private Vector3 diverRot;
    private float diverFOV;

    [Header("Transition Data")]
    private bool transitioning;
    private bool startedTransition;
    public float transitionDuration;
    public float transitionDelay;
    private bool sceneToDiver;

    [Header("Diver Data")]
    private ObjectInput diverProp;
    private bool presentDiver = false;

    private PostProcessVolume diverCameraPPV;
    private ColorGrading colorGrading;
    private LensDistortion lensDistortion;
    private Color initialColor;


    // Start is called before the first frame update
    void Start()
    {
        TableManager.Instance.OnTouch += OnTouchReceive;

        camera = Camera.main;
        diverPos = transform.position;
        diverCamera.enabled = true;
        diverCameraPPV = GetComponent<PostProcessVolume>();
        diverCameraPPV.profile.TryGetSettings(out colorGrading);
        diverCameraPPV.profile.TryGetSettings(out lensDistortion);
        if (colorGrading != null)
        {
            initialColor = colorGrading.colorFilter.value;
        }

        phPos = sceneCamera.transform.position;
        phRot = sceneCamera.transform.eulerAngles;
        phFOV = sceneCamera.fieldOfView;
        diverFOV = diverCamera.fieldOfView;

        diverCamera.transform.position = phPos;
        diverCamera.transform.eulerAngles = phRot;
        diverCamera.fieldOfView = phFOV;

        presentDiver = false;

        startedTransition = false;
        transitioning = false;

    }

    // Update is called once per frame
    void Update()
    {
        DiverInput();
        MoveCamera();
    }

    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
        AddDiver(diverProp);
        StartTransition();
    }

    void OnTouchReceive(Dictionary<int, FingerInput> surfaceFingers, Dictionary<int, ObjectInput> objectInputs)
    {
        if (objectInputs.Count > 0)
        {
            diverProp = objectInputs.TryGetValue(TableManager.DiverId, out diverProp) ? diverProp : null;
        }
        else
        {
            diverProp = null;
        }
    }

    public void ShowImage()
    {
        // we receive no signal, we show the placeholder image
        if (diverProp == null && !presentDiver)
        {
            if (diverCamera.enabled) diverCamera.enabled = false;
            if (sceneCamera != null) sceneCamera.enabled = true;
        }
        // we're receiving a signal, we enable the camera and turn off the placeholder
        else
        {
            //Debug.Log("we have a signal");
            if (sceneCamera.enabled) sceneCamera.enabled = false;
            if (diverCamera != null) diverCamera.enabled = true;
        }
    }

    public void StartTransition()
    {
        if (transitioning)
        {
            StartCoroutine(TransitionImage());
        }
    }

    IEnumerator TransitionImage()
    {
        yield return new WaitForSeconds(transitionDelay);

        if (transitioning && !startedTransition)
        {
            startedTransition = true;
            Vector3 startPos = sceneToDiver ? phPos : diverPos;
            Vector3 startRot = sceneToDiver ? phRot : diverRot;
            float startFOV = sceneToDiver ? phFOV : diverFOV;
            Vector3 endPos = sceneToDiver ? diverPos : phPos;
            Vector3 endRot = sceneToDiver ? diverRot : phRot;
            float endFOV = sceneToDiver ? diverFOV : phFOV;

            //Debug.Log("Let's start the transition. Here are our variables:\n" +
            //    startPos + " -> " + endPos + "\n" +
            //    startRot + " -> " + endRot + "\n" +
            //    startFOV + " -> " + endFOV);

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
                diverCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
                diverCamera.transform.eulerAngles = Vector3.Lerp(startRot, endRot, t);
                diverCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);

                if (diverCamera.transform.position.y < 35)
                {
                    if (colorGrading != null)
                    {
                        colorGrading.colorFilter.value = new Color(0.59f, 1.81f, 2.11f, 1f);
                    }

                    if (lensDistortion != null)
                    {
                        lensDistortion.intensity.value = -59;
                    }
                    
                }
                else
                {
                    if (colorGrading != null)
                    {
                        colorGrading.colorFilter.value = initialColor;
                    }

                    if (lensDistortion != null)
                    {
                        lensDistortion.intensity.value = 0;
                    }
                }


                yield return null;
            }

            transitioning = false;
            startedTransition = false;
        }
    }

    public void AddDiver(ObjectInput prop)
    {
        if (prop != null && prop.tagValue == TableManager.DiverId)
        // if (Input.GetKey(KeyCode.M))
        {
            //Debug.Log("we received the signal for the diver, we add it");
            Vector3 propPos = Helpers.ReverseZIndex(Helpers.GetWorldPositionOnPlane(camera, prop.position));
            // Vector3 propPos = new Vector3(-31.4f, -10.8f, -13.9f);
            //Vector3 propRot = new Vector3(90, Helpers.GetPropOrientationDeg(prop.orientation), 0);
            Vector3 propRot = new Vector3(0f, 0f, 0f);


            if (presentDiver)
            {
                //Debug.Log("we already have a diver, we just move it");
                if (transitioning) return;
                propPos.y = diverCamera.transform.position.y;
                if (!startedTransition && !transitioning)
                {
                    //Debug.Log("we didn't start a transition and don't have to either");
                    diverCamera.transform.position = Vector3.Lerp(diverCamera.transform.position, propPos, Time.fixedDeltaTime * 10);
                }
                //transitioning = false;
            }
            else
            {
                //Debug.Log("we didn't have a diver, now we do");
                propPos.y -= 2f;
                transitioning = true;
                sceneToDiver = true;
                presentDiver = true;
            }

            diverPos = propPos;
            diverRot = propRot;
            //Debug.Log("the diver's original position is " + diverPos);
        }
        else
        {
            if (presentDiver)
            {
                //Debug.Log("first frame since we don't have a diver anymore");
                transitioning = true;
                sceneToDiver = false;
            }
            presentDiver = false;
        }
    }

    public void DiverInput()
    {
        horizontalInput = Input.GetAxisRaw("4th Axis");
        verticalInput = Input.GetAxisRaw("Vertical");

        float triggerUp = Input.GetAxis("Advance");
        float triggerDown = Input.GetAxis("Return");

        goUp = Input.GetKey(upKey) || triggerUp > 0.1f;
        goDown = Input.GetKey(downKey) || triggerDown > 0.1f;
    }

    public void MoveCamera()
    {
        if (!presentDiver || transitioning) return;

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
        Vector3 centerToTarget = targetCamPos - diverPos;
        float distToTarget = centerToTarget.magnitude;

        if (distToTarget > maxDistance)
        {
            Vector3 centerToCam = transform.position - diverPos;
            float distToEdge = maxDistance - centerToCam.magnitude;
            targetCamPos = diverPos + centerToTarget.normalized * maxDistance;
        }

        float turnY = horizontalInput * rotationSpeed * Time.deltaTime;
        yRotation += turnY;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        float xBounded = Mathf.Clamp(targetCamPos.x, -80, 80);
        float yBounded = Mathf.Clamp(targetCamPos.y, -25, 10);
        float zBounded = Mathf.Clamp(targetCamPos.z, -45, 45);
        targetCamPos = new Vector3(xBounded, yBounded, zBounded);

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        diverPos = transform.position;
        diverRot = transform.eulerAngles;
    }
}