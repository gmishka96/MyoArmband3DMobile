using UnityEngine;

/// <summary>
/// Provides functionalities for extraction of Myo data from Android plugin.
/// and updating of objects' positions.
/// Myo positioning: on left forearm, led toward elbow and
/// led facing top during hand extension (palm facing ground).
/// Make sure that smartphone position is adjusted beforehand so that
/// Myo's coordinate plane matches that of Unity project.
/// Extends <see cref="MonoBehaviour"/>.
/// </summary>
public class MyoBleControl : MonoBehaviour
{
    private const bool IsLoggingActivated = false;
    private const bool IsXrObjectActivated = false;
    private const bool IsEthanActivated = true;
    private const byte TOAST_MAX_FRAMES = 100;

    private Object dataLock;
    private AndroidJavaObject pluginActivity;
    private byte toastFramesCounter;

    private string orientationStr = string.Empty;
    private string accelerationStr = string.Empty;
    private string gyroscopeStr = string.Empty;
    private string averageAbsEmgStr = string.Empty;

    private GameObject rightHandController;
    private GameObject leftHandController;

    private GameObject ethanRightForeArm;
    private GameObject ethanLeftForeArm;

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes variables and objects.
    /// </summary>
    void Start()
    {
        this.dataLock = new Object();
        this.pluginActivity = null;
        this.toastFramesCounter = 0;

        this.orientationStr = string.Empty;
        this.accelerationStr = string.Empty;
        this.gyroscopeStr = string.Empty;
        this.averageAbsEmgStr = string.Empty;

        try
        {
            if (IsXrObjectActivated == true)
            {
                this.rightHandController = GameObject.FindGameObjectWithTag("RightHand");
                this.leftHandController = GameObject.FindGameObjectWithTag("LeftHand");
            }
            else if (IsEthanActivated == true)
            {
                this.ethanRightForeArm = GameObject.FindGameObjectWithTag("EthanRightForeArm");
                this.ethanLeftForeArm = GameObject.FindGameObjectWithTag("EthanLeftForeArm");
            }

            this.InitializePlugin();
        }
        catch { }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Extracts data from Android plugin.
    /// Updates transforms' rotations.
    /// </summary>
    void Update()
    {
#if PLATFORM_ANDROID
        if (this.pluginActivity != null)
        {
            float xOrientation = 0;
            float yOrientation = 0;
            float zOrientation = 0;
            float wOrientation = 0;
            float xAcceleration = 0;
            float yAcceleration = 0;
            float zAcceleration = 0;
            float xGyroscope = 0;
            float yGyroscope = 0;
            float zGyroscope = 0;
            float averageAbsEmg = 0;

            lock (dataLock)
            {
                xOrientation = this.pluginActivity.Call<float>("getXOrientation");
                yOrientation = this.pluginActivity.Call<float>("getYOrientation");
                zOrientation = this.pluginActivity.Call<float>("getZOrientation");
                wOrientation = this.pluginActivity.Call<float>("getWOrientation");
                xAcceleration = this.pluginActivity.Call<float>("getXAcceleration");
                yAcceleration = this.pluginActivity.Call<float>("getYAcceleration");
                zAcceleration = this.pluginActivity.Call<float>("getZAcceleration");
                xGyroscope = this.pluginActivity.Call<float>("getXGyroscope");
                yGyroscope = this.pluginActivity.Call<float>("getYGyroscope");
                zGyroscope = this.pluginActivity.Call<float>("getZGyroscope");
                averageAbsEmg = this.pluginActivity.Call<float>("getAverageAbsEmg");
            }

            if (IsXrObjectActivated == true)
            {
                if (this.rightHandController != null)
                {
                    Quaternion myoQuaternion = new Quaternion(yOrientation, zOrientation, xOrientation, wOrientation);
                    this.rightHandController.transform.localRotation = myoQuaternion;
                }

                if (this.leftHandController != null)
                {
                    Quaternion myoQuaternion = new Quaternion(yOrientation, zOrientation, xOrientation, wOrientation);
                    this.leftHandController.transform.localRotation = new Quaternion(
                        myoQuaternion.x,
                        -myoQuaternion.y,
                        -myoQuaternion.z,
                        myoQuaternion.w);
                }
            }
            else if (IsEthanActivated == true)
            {
                if (this.ethanRightForeArm != null)
                {
                    float xRight = -yOrientation;
                    float yRight = xOrientation;
                    float zRight = -zOrientation;
                    float wRight = -wOrientation;

                    Quaternion rightQuaternion = new Quaternion(xRight, yRight, zRight, wRight);

                    /* axis: x = -yOrientation; w = -wOrientation
                     * euler: x = -180; y = 0; z = 180
                     * result: same x; same y; same z
                     */
                    // this.ethanRightForeArm.transform.rotation = Quaternion.Euler(-180, 0, 180) * rightQuaternion;

                    /* axis: x = -yOrientation; y = xOrientation; z = -zOrientation; w = -wOrientation
                     * euler: x = -180; y = 0; z = -180
                     * result: same x; mirrored y; same z
                     */
                    //this.ethanRightForeArm.transform.rotation = Quaternion.Euler(-180, 0, -180) * rightQuaternion;

                    /* axis: x = -yOrientation; y = xOrientation; z = -zOrientation; w = -wOrientation
                     * euler: x = -180; y = 0; z = -180
                     * result: same x; mirrored y; same z
                     */
                    Quaternion q = Quaternion.Euler(-180, 0, -180) * rightQuaternion;
                    q.z *= -1;
                    this.ethanRightForeArm.transform.rotation = q;
                }

                if (this.ethanLeftForeArm != null)
                {
                    float xLeft = yOrientation;
                    float yLeft = xOrientation;
                    float zLeft = zOrientation;
                    float wLeft = wOrientation;

                    Quaternion leftQuaternion = new Quaternion(xLeft, yLeft, zLeft, wLeft);
                    this.ethanLeftForeArm.transform.rotation = leftQuaternion;
                }
            }

            if (IsLoggingActivated == true)
            {
                this.orientationStr = $"Orientation x: {xOrientation}, y: {yOrientation}, z: {zOrientation}, w: {wOrientation}";
                this.accelerationStr = $"Acceleration x: {xAcceleration}, y: {yAcceleration}, z: {zAcceleration}";
                this.gyroscopeStr = $"Gyroscope x: {xGyroscope}, y: {yGyroscope}, z: {zGyroscope}";
                this.averageAbsEmgStr = $"Average EMG: {averageAbsEmg}";

                string logMessage = $"Plugin extract:" +
                    $"\n{this.orientationStr}" +
                    $"\n{this.accelerationStr}" +
                    $"\n{this.gyroscopeStr}" +
                    $"\n{this.averageAbsEmgStr}";

                Debug.Log(logMessage);

                this.toastFramesCounter++;
                if (this.toastFramesCounter > TOAST_MAX_FRAMES)
                {
                    this.toastFramesCounter = 0;
                    this.pluginActivity.Call("showToast", logMessage);
                }
            }
        }
#endif
    }

    void InitializePlugin()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        this.pluginActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }
}
