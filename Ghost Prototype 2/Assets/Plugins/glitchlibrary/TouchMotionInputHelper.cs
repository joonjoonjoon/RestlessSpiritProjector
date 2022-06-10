using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionInputHelper : MonoBehaviour {

    private static float circularAxis = 0;
    private static float straightAxis = 0;
    private static float straightAxisAngle = 0;

    public static float GetCircularAxis() { return circularAxis; }
    public static float GetStraightAxis() { return straightAxis; }
    public static float GetStraightAxisAngle() { return straightAxisAngle; }

    public static float GetStraightAxisRelativeTo(float angle, float tolerance)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(angle, straightAxisAngle)) > tolerance &&
            Mathf.Abs(Mathf.DeltaAngle(angle + 180, straightAxisAngle)) > tolerance) return 0;
        var v = Quaternion.Euler(0, 0, straightAxisAngle) * Vector3.right * straightAxis;
        v = Quaternion.Euler(0, 0, -angle) * v;
        return v.x;
    }

    [InspectorNote("Swipe Motion Settings", NoteColor.pink)]
    public bool swipe;

    public float swipeClamp = 5f;   // this mostly prevents jitter when switching directions from CW to CCW or viceversa

    [InspectorNote("Circular Motion Settings", NoteColor.green)]
    public bool circle;

    public float minDistanceBetweenPoints = 8;
    public float maxDistanceBetweenPoitns = 8;
    public float minAngleToDetectTurn = 1.8f;
    public float angleToDetectTurnClamp = 5f;   // this mostly prevents jitter when switching directions from CW to CCW or viceversa
    public float minPointSampleSize = 6;
    public float maxPointSampleSize = 6;
    public bool debugCircularEnabled;
    public bool debugStraightEnabled;

    private List<Vector3> touchPositions;
    private DebugCameraLines cam;
    void Start()
    {
        touchPositions = new List<Vector3>();
    }

    void Update()
    {
        

        // reset axis
        circularAxis = 0;
        straightAxis = 0;
        straightAxisAngle = 0;

        if(Input.GetMouseButtonDown(0))
        {
            touchPositions = new List<Vector3>();
            touchPositions.Add(Input.mousePosition);
        }

        // Continuous circular detection
        if (Input.GetMouseButton(0) && touchPositions.Count >= 1)
        {
            // calculate distance between last 2 points
            float distance = Vector3.Distance(Input.mousePosition, touchPositions[touchPositions.Count - 1]);

            // calculate whether it's a relatively short or a long distance, so we can include more/less points
            float distanceFactor = 1-Mathf.Clamp01((Mathf.Abs(distance) - minDistanceBetweenPoints) / (maxDistanceBetweenPoitns - minDistanceBetweenPoints));
            int pointSampleSize = (int)(distanceFactor * (maxPointSampleSize - minPointSampleSize) + minPointSampleSize);

            Debug.Log(distanceFactor + " " + pointSampleSize);
            // check if minimum distance is met
            if (distance > minDistanceBetweenPoints)
            {
                // add current touch
                touchPositions.Add(Input.mousePosition);

                // make sure we have enough points to do detection
                if (touchPositions.Count > 2)
                {
                    // measure current and last angles
					var currentAngle = Vector3Extension.Angle2D(touchPositions[touchPositions.Count - Mathf.Min(touchPositions.Count, (int)pointSampleSize)], touchPositions[touchPositions.Count - 1]);
					var lastAngle = Vector3Extension.Angle2D(touchPositions[touchPositions.Count - Mathf.Min(touchPositions.Count, (int)pointSampleSize)], touchPositions[touchPositions.Count - 2]);
                    var anglediff = Mathf.DeltaAngle(currentAngle, lastAngle);

                    // if the difference is over the threshold
                    if (Mathf.Abs(anglediff) > minAngleToDetectTurn)
                    {
                        // subtract threshold !!! not sure if needed -joon
                        if (anglediff > 0) anglediff -= minAngleToDetectTurn;
                        if (anglediff < 0) anglediff += minAngleToDetectTurn;
                        
                        // set axis, so it can be read
                        circularAxis = Mathf.Clamp(anglediff,-angleToDetectTurnClamp ,angleToDetectTurnClamp );

                        // add debug GL lines
                        if (debugCircularEnabled)
                        {
                            if (circularAxis > 0)
                            {
                                DebugCameraLines.AddLine(touchPositions[touchPositions.Count - (int)pointSampleSize], touchPositions[touchPositions.Count - 1], Color.red);
                            }
                            else
                            {
                                DebugCameraLines.AddLine(touchPositions[touchPositions.Count - (int)pointSampleSize], touchPositions[touchPositions.Count - 1], Color.blue);
                            }
                        }
                    }
                    else
                    {
                        // set straight axis
                        straightAxis = distance;
                        straightAxisAngle = lastAngle;

                        //Debug.Log(straightAxis + " " + straightAxisAngle);

                        if (debugCircularEnabled)
                        {
                            DebugCameraLines.AddLine(touchPositions[touchPositions.Count - (int)pointSampleSize], touchPositions[touchPositions.Count - 1], Color.yellow);
                        }
                    }
                    if (debugCircularEnabled)
                    {
                        DebugCameraLines.AddLine(touchPositions[touchPositions.Count - (int)pointSampleSize], touchPositions[touchPositions.Count - 2], Color.white.SetA(0.4f));
                    }
                }
            }
        }

        
    }
}
