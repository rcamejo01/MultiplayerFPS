using UnityEngine;
using System.Collections;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class RigidBodyFPSWalker : MonoBehaviour
{

	public float speed = 30f;
	public float gravity = 30f;
	public float maxVelocityChange = 30f;
	public bool canJump = true;
	public float jumpHeight = 5f;
	private bool grounded = false;

	public int health = 100;
	public int kills = 0;
	public int deaths = 0;

	public GameObject[] activePlayers;

	public GameObject FPSCam;
	public PhotonView photonView;

	void Awake()
	{
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
		//photonView.RPC("updateName", PhotonTargets.AllBuffered, PhotonNetwork.playerName, health);
		gameObject.name = PhotonNetwork.playerName;
	}

	void FixedUpdate()
	{
		kills = PlayerPrefs.GetInt("kills");
		deaths = PlayerPrefs.GetInt("deaths");

		if (grounded)
		{
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

			// Jump
			if (canJump && Input.GetButton("Jump"))
			{
				GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}

		// We apply gravity manually for more tuning control
		GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));

		grounded = false;
	}

	void OnCollisionStay()
	{
		grounded = true;
	}

	float CalculateJumpVerticalSpeed()
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10, 10, 100, 30), "HP | " + health);
	}
}