using NaughtyAttributes;
using UnityEngine;

using Action = System.Action;

public class Player : MonoBehaviour {
	public Action onRespawn;

	[Header("Balance")]
	[Space]
	[SerializeField] [MinMaxSlider(0, 5, false)] Vector2 rageBuffs = new Vector2(1, 2);
	[SerializeField] float dashStaminaPrice = 50;

	[Header("Anims")]
	[Space]
	[SerializeField] int dieAnimationCount = 10;
	[SerializeField] int cameraAnimsCount = 6;
	[SerializeField] AudioClip dieClip = null;
	[SerializeField] AudioClip spawnClip = null;
	[SerializeField] AudioClip analogNoice = null;

	[Header("Refs")]
	[Space]
	public PlayerMover mover = null;
	[SerializeField] Health health = null;
	[SerializeField] PlayerTimer timer = null;
	[SerializeField] PlayerRageBar rageBar = null;
	[SerializeField] PlayerStaminaBar staminaBar = null;
	[SerializeField] Animator anim = null;
	[SerializeField] Animator cameraAnim = null;

	private void Awake() {
		GameManager.Instance.player = this;

		rageBar.onRageValueChange += OnRageValueChange;

		GameManager.Instance.player.mover.onRespawnEnd += OnRespawnEnd;
	}

	private void OnDestroy() {
		if(GameManager.Instance.player == this)
			GameManager.Instance.player = null;
	}

	public int IncreaseComboCounter() {
		return rageBar.IncreaseComboCounter();
	}

	public bool TryDash() {
		if (staminaBar.IsEnoughStamina(dashStaminaPrice)) {
			staminaBar.DecreaseStamina(dashStaminaPrice);

			return true;
		}
		else {
			//TODO: floating text: not enaugh stamina
			return false;
		}
	}

	void OnRageValueChange() {
		float currBuff = Mathf.Lerp(rageBuffs.x, rageBuffs.y, rageBar.CurrRagePersent);
		mover.SetRageBuff(currBuff);
		staminaBar.regenerationMultiplier = currBuff;
		mover.dashForceMultiplier = currBuff;
	}

	public void OnEnterNewLevel(Transform newSpawnPos) {
		mover.OnEnterNewLevel(newSpawnPos);
	}

	void Die() {
		GameManager.Instance.isPlaying = false;
		mover.OnDie();

		Destroy(GameManager.Instance.ambient.gameObject);
		AudioManager.Instance.Play(dieClip, channel: AudioManager.AudioChannel.Sound);
		AudioSource analogNoiceas = AudioManager.Instance.PlayFaded(analogNoice, channel: AudioManager.AudioChannel.Sound);
		analogNoiceas.volume = 0.7f;

		anim.SetInteger("DieAnimation", Random.Range(1, dieAnimationCount));
		cameraAnim.SetInteger("DieAnimation", Random.Range(1, cameraAnimsCount));

		LeanTween.value(analogNoiceas.volume, 1.0f, 0.1f)
			.setDelay(2.2f)
			.setOnUpdate((float v)=> { 
				analogNoiceas.volume = v;
			});

		LeanTween.delayedCall(2.5f, () => {
			anim.SetInteger("DieAnimation", 0);
			cameraAnim.SetInteger("DieAnimation", 0);

			timer.Init();
			staminaBar.Init();
			rageBar.Init();
			mover.Respawn();
			onRespawn?.Invoke();
		});

		LeanTween.value(analogNoiceas.volume, 0.7f, 0.1f)
			.setDelay(2.6f)
			.setOnUpdate((float v) => {
				analogNoiceas.volume = v;
			});

		LeanTween.delayedCall(4.5f, () => {
			anim.SetTrigger("IsSpawning");
		});

		LeanTween.delayedCall(5.0f, () => {
			if(analogNoiceas != null)
				Destroy(analogNoiceas.gameObject);
			AudioManager.Instance.Play(spawnClip, channel: AudioManager.AudioChannel.Sound);
		});
	}

	void OnRespawnEnd() {
		
	}
}
