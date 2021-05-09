using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public Camera FPSCam;
	public GameObject hitPar;
	public int damage = 30;
	public int range = 10000;

	public Animation am;
	public AnimationClip shoot;
	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			FireShot();
		}
	}

	public void FireShot()
	{
		if (!am.IsPlaying(shoot.name))
		{
			am.CrossFade(shoot.name);
			RaycastHit hit;
			Ray ray = FPSCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

			if (Physics.Raycast(ray, out hit, range))
			{
				if (hit.transform.tag == "Player")
				{
					hit.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.AllBuffered, damage);
				}
				GameObject particleClone;
				particleClone = PhotonNetwork.Instantiate(hitPar.name, hit.point, Quaternion.LookRotation(hit.normal), 0) as GameObject;
				Destroy(particleClone, 2);
				Debug.Log(hit.transform.name);

			}

		}
	}
}

