using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioCam : MonoBehaviour {

	public Transform target; // The target object to follow
	private GameObject player; // The Player object to follow
	public float cameraDistance = 5f; // The distance between the camera and the target
	public float wallOffset = 0.1f; // The offset to move the camera back when it collides with a wall
	public float cursorSensitivity = 10f; // The sensitivity of the cursor movement
	public float CStickSensitivity = 10f; // The sensitivity of the C-Stick movement
	public bool invertCursorY = false; // Whether to invert the Y axis of the cursor movement
	public bool isThirdPerson = true; // Whether the camera is in third-person view
	public float smoothTime = 0.3f; // The duration of the camera smoothing
	public float camYOffset = 4;

	private float cursorX, cursorY; // The current cursor position
	private float cStickX, cStickY; // The current cursor position
	private Vector3 velocity = Vector3.zero; // The current velocity of the camera
	private float cameraRotation = 0; // Holds marios current cam rotation.
	public float cameraRotationOffset = 3f; // Camera speed?
	private Vector3 cameraControl; // Variable for controlled camera offset(cstick)
	private float controllerYoffset; //for camera rotation calculation
	public bool isLocked = false; //if cam is locked.
	private Vector3 cameraPosition;

	public float confStickXmax = 1;
	public float confStickYmax = 1;
	private bool confRotate = false;
	private bool confSmooth = true;
	private bool confWalk = true;
	private int confMode = 0;

	public static MarioCam marioCamera;

	public void setState(int state, Vector3 tpos, Quaternion trot){
		confMode = state;
		transform.rotation = trot;
		transform.localPosition = tpos;
		switch(confMode){
		case 0://OW FREE
			confRotate = true;
			confWalk = true;
			confSmooth = true;
			confStickXmax = 1;
			confStickYmax = 1;
			break;
		case 1://STATIC
			confRotate = false;
			confWalk = false;
			confSmooth = false;
			confStickXmax = 0;
			confStickYmax = 0;
			break;
		case 2://OW FREE+STATIC PATH
			confRotate = false;
			confWalk = true;
			confSmooth = true;
			confStickXmax = 0;
			confStickYmax = 0;
			break;
		case 3://OW 2D
			break;
		case 4://OW HEAD
			break;

		}
	}

	void Start()

	{
		cameraPosition = target.position - transform.forward *cameraDistance;
		cStickX = 0.0f; cStickY = 0.0f;
		Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
		Cursor.visible = false; // Hide the cursor
		player = MarioController.marioObject.gameObject;
		marioCamera = this;
		//scr_gameInit.globalValues.gameObject.transform.position = transform.position;
	}


	void Update()
	{
		// Smoothly interpolate the camera's position towards the desired position
		if(confSmooth) transform.position = Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, smoothTime);

		if(!isLocked){
			//if (confMode != 1)
				//scr_gameInit.globalValues.gameObject.transform.position = transform.position;
			
				#if UNITY_EDITOR
			cursorX += (Input.GetAxis("Mouse X") * cursorSensitivity * Time.deltaTime * -1)*confStickXmax;
			cursorY += (Input.GetAxis("Mouse Y") * cursorSensitivity * Time.deltaTime * -1)*confStickYmax;
				cursorY = Mathf.Clamp(cursorY, -20, 70f); // Clamp the Y axis to prevent camera flipping
				#else
			cStickX += (UnityEngine.N3DS.GamePad.CirclePadPro.x * cursorSensitivity * Time.deltaTime * -1)*confStickXmax;
			cStickY += (UnityEngine.N3DS.GamePad.CirclePadPro.y * cursorSensitivity * Time.deltaTime * -1)*confStickYmax;
			cStickY = Mathf.Clamp(cStickY, -20, 70f); // Clamp the Y axis to prevent camera flipping
				#endif

			// Rotate the player and camera around the target based on the cursor and C-Stick movement
			cameraControl = new Vector3((cursorY + cStickY)/2.2f, cursorX + cStickX, 0);
			if(confRotate) if(MarioController.marioObject.isMoving){
				float mAngleY = MarioController.marioObject.transform.eulerAngles.y;
				if((mAngleY-transform.eulerAngles.y)-cameraControl.y!=0) cameraRotation += ((mAngleY-transform.eulerAngles.y)/(90/3));
			}
			transform.localRotation = Quaternion.Euler(transform.rotation.x+cameraControl.x, cameraRotation+cameraControl.y, transform.rotation.z + cameraControl.y*2);

			if(confWalk){

				float targetY = MarioController.marioObject.groundedPosition + MarioController.marioObject.camYOffset + (cursorY + cStickY) / 100;
				float distance = Mathf.Abs(targetY - target.position.y);
				float smoothSpeed = distance / 0.5f; // ensure it takes half a second to move
				target.position = new Vector3(MarioController.marioObject.transform.position.x, Mathf.Lerp(target.position.y, targetY, Time.deltaTime * smoothSpeed), MarioController.marioObject.transform.position.z);

				// Calculate camera position based on target position and camera distance
				cameraPosition = target.position - transform.forward *cameraDistance;

				RaycastHit hit;

				if (Physics.Raycast(target.position, cameraPosition - target.position, out hit, cameraDistance) && hit.collider.tag=="camBlock")
				{
					cameraPosition = hit.point + hit.normal *0.2f; //dont peek into walls!
				}
				transform.LookAt(target); //look at the camera target
			}
		}
	}
}