using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CameraController : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Vector3 LookTarget;
    [HideInInspector]
    public Vector3 NewLookTarget;
    private Vector3 _velocity;
    private Vector3 _targetVelocity;
    [SerializeField]
    private float _damping = 0.95f;
    [SerializeField]
    private float _stiffness = 0.001f;
    [SerializeField]
    private float _lissajous_a = 0.03f;
    [SerializeField]
    private float _lissajous_b = 0.04f;
    [SerializeField]
    private float _lissajous_delta = 0.25f;
    [SerializeField]
    private float _lissajous_amount = 0.01f;

    public Volume m_Volume;
    private VolumeProfile profile;
    private DepthOfField dof;

    // Start is called before the first frame update
    void Start()
    {
        //initialise the newlooktarget to the current target so we don't suddenly swerve the camera
        NewLookTarget = LookTarget;
        //cameraData = GetComponent<HDAdditionalCameraData>();
        profile = m_Volume.sharedProfile;
        profile.TryGet<DepthOfField>(out dof);
    }

    Vector3 LissajousOffset(float time) {
        return new Vector3(0.0f, Mathf.Sin(_lissajous_a * time + _lissajous_delta), Mathf.Cos(_lissajous_b * time))*_lissajous_amount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //exponential motion
        // gameObject.transform.Translate((TargetPosition - gameObject.transform.position)*0.02f,Space.World);
        //LookTarget += (NewLookTarget - LookTarget)*0.02f;

        //damped harmonic motion, Euler integration
        _velocity = (_velocity + (TargetPosition - gameObject.transform.position)*_stiffness) * _damping;
        _targetVelocity = (_targetVelocity + (NewLookTarget + LissajousOffset(Time.time) - LookTarget)*_stiffness) * _damping;
        gameObject.transform.position += _velocity;
        LookTarget += _targetVelocity;
        gameObject.transform.LookAt(LookTarget);

        //cameraData.physicalParameters.focusDistance = (gameObject.transform.position - LookTarget).magnitude-1;
        dof.farFocusStart.value = (gameObject.transform.position - LookTarget).magnitude + 0.5f;
    }
}
