using UnityEngine;
using UnityEngine.InputSystem;

public class GyroControl : MonoBehaviour
{
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    float lowPassKernelWidthInSeconds = 1.0f;
    private float lowPassFilterFactor;
    private Vector3 lowPassValue = Vector3.zero;
    GameObject headRig;

    void Start()
    {
        Debug.Log($"Is gyro supported: {SystemInfo.supportsGyroscope}");

        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            Debug.Log($"Is gyro enabled on start: {UnityEngine.InputSystem.Gyroscope.current.enabled}");
        }
        else
        {
            Debug.Log("Gyroscope.current is null!");
        }

        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.AttitudeSensor.current);
            Debug.Log($"Is gyro enabled on start: {UnityEngine.InputSystem.AttitudeSensor.current.enabled}");
        }
        else
        {
            Debug.Log("AttitudeSensor.current is null!");
        }

        try
        {
            this.headRig = GameObject.FindGameObjectWithTag("HeadRig");
        }
        catch { }

        //Filter Accelerometer
        this.lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        this.lowPassValue = Input.acceleration;
    }

    void Update()
    {
        try
        {
            Debug.Log($"Is gyroscope enabled on update: {UnityEngine.InputSystem.Gyroscope.current.enabled}");
            Debug.Log($"Is attitude sensor enabled on update: {UnityEngine.InputSystem.AttitudeSensor.current.enabled}");

            Quaternion attitude = UnityEngine.InputSystem.AttitudeSensor.current.attitude.ReadValue();
            Debug.Log($"Attitude sensor value: {attitude}");
            
            // The Device uses a 'left-hand' orientation, we need to transform it to 'right-hand'
            Quaternion tGyro = new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);

            // the gyro attitude is tilted towards the floor and upside-down relative to what we want in unity.
            // First Rotate the orientation up 90deg on the X Axis, then 180Deg on the Z to flip it right-side up. 
            Quaternion tRotation = Quaternion.Euler(-90f, 0f, 0f) * tGyro;
            tRotation = Quaternion.Euler(0f, 0f, 180f) * tRotation;
            tRotation = Quaternion.Euler(0f, -90f, 0f) * tRotation;

            // You can now apply this rotation to any unity camera!
            this.transform.localRotation = tRotation;

            if (this.headRig != null)
            {
                Vector3 lowPassValue = Vector3.Lerp(this.lowPassValue, Input.acceleration, this.lowPassFilterFactor);
                //this.headRig.transform.Translate(lowPassValue);
                //this.headRig.transform.position += lowPassValue;
            }
        }
        catch
        { }
    }
}
