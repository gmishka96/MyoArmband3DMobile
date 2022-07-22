# MyoArmband3DMobile
Project represents a Unity application for Android that connects to a Myo Armband named My Myo.
The connection is established using a custom Android plugin that provides a BLE foreground service.

The 3D application contains:
- a prefab character model to which head a camera is attached
- script that reads the phone gyro to rotate the camera along with the phone
- script that rotates the left hand according to the band orientation and mirros the right left hand.

It is assumed that the band is on the left forearm with LED on top and pointing to elbow.

There is an initial position that has to be taken - try and error.

Project was tested with Android 9 (API 28).
Version of Unity used: 2020.3.30f1

P.S.: the import to Unity 2020.3.37f1 was terrible.
