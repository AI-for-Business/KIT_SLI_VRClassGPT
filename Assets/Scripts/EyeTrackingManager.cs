using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EyeTrackingManager : MonoBehaviour
{
    public OVREyeGaze eyeGaze;
    //public GameObject SphereEyeGazeHelper;
    private Vector3 boxSize = new Vector3(1, 1, 1);
    public float boxShrinkFactor = 20.0f;
    private Vector3 raycastDirection;

    [SerializeField]
    private float rayDistance = 10f;
    [SerializeField]
    private LayerMask raycastLayerMask;


    //If the eye Gaze is on the Teacher or on the Slides call the coresponding Event
    public UnityEvent onTeacherFocus;
    public UnityEvent onSlidesFocus;
    public UnityEvent onNoFocus;

   
    void Start()
    {
        eyeGaze = GetComponent<OVREyeGaze>();
    }

    int count = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        
        RaycastHit hit;

        raycastDirection = transform.TransformDirection(Vector3.forward);
        Vector3 normalizedDirection = Vector3.Normalize(raycastDirection);

        //SphereEyeGazeHelper.transform.position = transform.position + normalizedDirection * 4f;



        if (Physics.BoxCast(transform.position,boxSize/boxShrinkFactor, raycastDirection, out hit, Quaternion.identity, rayDistance, raycastLayerMask))
        {
            count++;

            if (count % 50 == 0)
            {
                Debug.Log(count);
                if (hit.collider.CompareTag("Teacher"))
                {
                    onTeacherFocus.Invoke();
                    Debug.Log("Teacher hit");
                }
                else if (hit.collider.CompareTag("Blackboard"))
                {
                    onSlidesFocus.Invoke();
                    Debug.Log("Blackboard in Focus");
                }
            }
           


        }
        else
        {
            onNoFocus.Invoke();
            // Debug.Log("NO Focus");
        }
    }

    // Visual aid in Scene View
    private void OnDrawGizmos()
    {
        // Start position of the box cast
        Vector3 boxStartPos = transform.position;

        // Calculate the end position of the box cast
        Vector3 boxEndPos = boxStartPos + raycastDirection.normalized * rayDistance;

        // Draw the box at the start and end positions
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxStartPos, boxSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxEndPos, boxSize);

        // Draw a line between the start and end positions
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(boxStartPos, boxEndPos);
    }
}
