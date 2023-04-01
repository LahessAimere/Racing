using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotorcycleControlsSimple : MonoBehaviour { //MS Motorcycle test - Marcos Schultz (www.schultzgames.com)
 
	public WheelCollider frontWheel;
	public WheelCollider rearWheel;
	public GameObject meshFront;
	public GameObject meshRear;
 
	float rbVelocityMagnitude;
	float horizontalInput;
	float verticalInput;
	float medRPM;
	
 
	void OnEnable(){
		WheelCollider WheelColliders = GetComponentInChildren<WheelCollider>();
		WheelColliders.ConfigureVehicleSubsteps(1000, 30, 30);
	}
 
	void FixedUpdate () {
		horizontalInput = Input.GetAxis ("Horizontal");
		verticalInput = Input.GetAxis ("Vertical");
		
		// motorTorque
		rearWheel.motorTorque = verticalInput * 10f;
		
		//steerAngle
		float nextAngle = horizontalInput * 35.0f;
		frontWheel.steerAngle = Mathf.Lerp (frontWheel.steerAngle, nextAngle, 0.125f);
		
		// Brake
		if (Mathf.Abs (verticalInput) < 0.1f) 
		{
			rearWheel.brakeTorque = frontWheel.brakeTorque =10f;
			rearWheel.motorTorque = 0.0f;
		} 
		else 
		{
			rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
		}
	}
 
	void Update()
	{
		//update wheel meshes
		Vector3 temporaryVector;
		Quaternion temporaryQuaternion;
		
		frontWheel.GetWorldPose(out temporaryVector, out temporaryQuaternion);
		meshFront.transform.rotation = temporaryQuaternion;
		
		rearWheel.GetWorldPose(out temporaryVector, out temporaryQuaternion);
		meshRear.transform.rotation = temporaryQuaternion;
		
		// Mock up mouvement
		float rotationAngle = Input.GetAxis ("Horizontal");
		verticalInput = Input.GetAxis ("Vertical");
		// Influence accelerations sur la rotation
		rotationAngle = rotationAngle*10 *Time.deltaTime;
        
		transform.Rotate(0,rotationAngle,0);
		transform.position = transform.position +
		                     transform.forward * (verticalInput*10 * Time.deltaTime);
	}
}