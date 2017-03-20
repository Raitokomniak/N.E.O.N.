using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Image healthFill;
	public Image characterHealthFill;
	public Text healthText;
	IEnumerator BlinkHealth;
	bool blinkHealth;
	bool blinkRunning;

	public Image adrenalineRushFill;

	public Image characterNeonOverlay;
	Color helmetMagenta = new Color32(251, 0, 78, 198);
	Color helmetGreen = new Color32(50, 251, 0, 198);

	//FOR DEBUGGING ONLY!!!!!!!!!!!!!!!!
	PlayerHealth health;

	// Use this for initialization
	void Awake () {
		BlinkHealth = BlinkHealthStatus ();
		UpdateHealth (100);
		health = GameObject.Find("Player").GetComponent<PlayerHealth>();
		ChangeHelmetColor (helmetMagenta);
		StartCoroutine (ForceStatusAfterDelay ());
	}

	IEnumerator ForceStatusAfterDelay() {
		yield return new WaitForSeconds (1f);
		//health.takeDamage (50);
		StartCoroutine(BurnDownAbility ());
	}

	void Update(){

	}

	public void UpdateHealth(int amount){
		if (amount <= 0) amount = 0;

		healthFill.fillAmount = amount * 0.01f;
		healthText.text = amount.ToString () + "%";

		UpdateCharacterHealthFill (amount);

		if (amount < 100 && !blinkRunning) {
			blinkHealth = true;
			StartCoroutine (BlinkHealthStatus());
		} else if(amount == 100) {
			blinkHealth = false;
		}


	}

	public void UpdateCharacterHealthFill(int amount){
		int newAmount = 100 - amount;
		characterHealthFill.fillAmount = newAmount * 0.01f;
	}


	IEnumerator BlinkHealthStatus(){
		while (blinkHealth) {
			blinkRunning = true;
			for (float a = .1f; a < .8f; a += 0.05f) {
				characterHealthFill.color = new Color (1, 1, 1, a);
				yield return new WaitForSecondsRealtime (.03f);
			}
			for (float a = .8f; a > 0f; a -= 0.05f) {
				characterHealthFill.color = new Color (1, 1, 1, a);
				yield return new WaitForSecondsRealtime (.04f);
			}
		}
		blinkRunning = false;
	}


	void ChangeHelmetColor(Color color){
		characterNeonOverlay.color = color;
	}

	//FOR DEBUGGING ONLYYYYYYYYY
	//CHANGE WHEN ABILITY SCRIPT CAN BE REFERENCED 
	IEnumerator BurnDownAbility(){
		ChangeHelmetColor (helmetGreen);
		for (int t = 100; t > 0; t--) {
			adrenalineRushFill.fillAmount = t * 0.01f;
			yield return new WaitForSecondsRealtime (.1f);
		}
		ChangeHelmetColor (helmetMagenta);
		ReCharge ();
	}


	void ReCharge(){
		StartCoroutine (ReChargeAbility ());
	}

	IEnumerator ReChargeAbility(){
		for (int t = 0; t < 100; t++) {
			adrenalineRushFill.fillAmount = t * 0.01f;
			yield return new WaitForSecondsRealtime (.5f);
		}
	}	
}
