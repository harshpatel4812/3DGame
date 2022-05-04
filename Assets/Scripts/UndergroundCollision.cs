using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UndergroundCollision : MonoBehaviour
{

	void OnTriggerEnter (Collider other)
	{
		//in last bottom coliision trigger object is

		if (!Game.isGameover) {
			string tag = other.tag;
			
			if (tag.Equals ("Object")) { 
				Level.Instance.objectsInScene--;
				UIManager.Instance.UpdateLevelProgress ();

				
				Magnet.Instance.RemoveFromMagnetField (other.attachedRigidbody);

				Destroy (other.gameObject);

				// if win
				if (Level.Instance.objectsInScene == 0) {
					//win
					UIManager.Instance.ShowLevelCompletedUI ();
					Level.Instance.PlayWinFx ();

					//Next level 2 seconds
					Invoke ("NextLevel", 2f);
				}
			}
			if (tag.Equals ("Obstacle")) {
				Game.isGameover = true;
				Destroy (other.gameObject);

				//camera animation vibrating
				Camera.main.transform
					.DOShakePosition (1f, .2f, 20, 90f)
					.OnComplete (() => {
					//restart
					Level.Instance.RestartLevel ();
				});
			}
		}
	}

	void NextLevel ()
	{
		Level.Instance.LoadNextLevel ();
	}
		
}
