using NaughtyAttributes;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] [MinMaxSlider(0, 5, false)] Vector2 rageBuffs = new Vector2(1, 2);

	[Header("Refs")]
	[Space]
	[SerializeField] PlayerMover mover = null;
	[SerializeField] Health health = null;
	[SerializeField] PlayerTimer timer = null;
	[SerializeField] PlayerRageBar rageBar = null;


	private void Awake() {
		GameManager.Instance.player = this;

		rageBar.onRageValueChange += OnRageValueChange;
	}

	private void OnDestroy() {
		if(GameManager.Instance.player == this)
			GameManager.Instance.player = null;
	}

	public int IncreaseComboCounter() {
		return rageBar.IncreaseComboCounter();
	}

	void OnRageValueChange() {
		float currBuff = Mathf.Lerp(rageBuffs.x, rageBuffs.y, rageBar.CurrRagePersent);
		mover.SetRageBuff(currBuff);
	}
}
