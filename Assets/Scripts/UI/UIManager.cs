//#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject owningPlayer = null;
	public ShipStats stats = null;
	public Text pointsAvailable = null;
	public string[] inputStrings = null;
	public RectTransform panel = null;
	public Dictionary<string, Pair<Slider, Text>> upgradeUI = new Dictionary<string, Pair<Slider, Text>>();
	public List<Slider> forSlidersDict = new List<Slider>();
	public List<Text> forTextsDict = new List<Text>();
	public Gradient gradient;
	public Color gradientToColor;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
	void Start () {
		stats = owningPlayer.GetComponent<ShipStats>();
		pointsAvailable.text = "Available upgrade points: " + stats.upgradeInfo.upgradePoints.ToString();
		inputStrings = new string[] {"1", "2", "3", "4"};
		panel.gameObject.SetActive(false);
		int i = 0;
		foreach (var s in ShipStats.upgradeNames) {
			Debug.Log(forTextsDict[i].name);
			Debug.Log(forSlidersDict[i].name);
			upgradeUI.Add(s, new Pair<Slider, Text>(forSlidersDict[i], forTextsDict[i++]));
		}
		foreach (var key in upgradeUI.Keys) {
			Slider temp = upgradeUI[key].GetFirst();
			temp.maxValue = stats.playerUpgrades[key].possibleUpgradeLevels;
			temp.minValue = 0;
			temp.wholeNumbers = !false;
			Text temp1 = upgradeUI[key].GetSecond();
			temp1.text = "Current Level: " + stats.playerUpgrades[key].currentUpgradeLevel.ToString();
		}
	}
	
	void Update () {
		string temp = Input.inputString;
		if (temp == "b") {
			panel.gameObject.SetActive(!panel.gameObject.activeSelf);
			pointsAvailable.text = "Available upgrade points: " + stats.upgradeInfo.upgradePoints.ToString();
		}
		#if (DEBUG)
			if (Input.GetKeyDown(KeyCode.D)) {
				Debug.Log("KEYS FROM PLAYER UPGRADES");
				foreach (var s in stats.playerUpgrades.Keys) Debug.Log(s);
				Debug.Log("KEYS FROM UI");
				foreach (var s in upgradeUI.Keys) {
					Debug.Log(upgradeUI[s].GetFirst().name);
					Debug.Log(upgradeUI[s].GetSecond().name);
				}

			}
			if (Input.GetKeyDown(KeyCode.H)) {
				stats.upgradeInfo++;
			}
		#endif
		if (panel.gameObject.activeSelf && stats.upgradeInfo.upgradePoints > 0 && System.Array.Exists(inputStrings, str => str == temp)) {
			switch (System.Int32.Parse(temp)) {
				case 1:
					stats.UpgradeShip("damage", "damage");
					UpdateUI("damage", upgradeUI["damage"]);
					break;
				case 2:
					stats.UpgradeShip("speed", "speed");
					UpdateUI("speed", upgradeUI["speed"]);
					break;
				case 3:
					stats.UpgradeShip("damageReduction", "damageReduction");
					UpdateUI("damageReduction", upgradeUI["damageReduction"]);
					break;
				case 4:
					stats.UpgradeShip("health", "curHealth");
					UpdateUI("health", upgradeUI["health"]);
					break;
			}
		}
	}

	void UpdateUI(string upgradeName, Pair<Slider, Text> pair) {
		Slider temp = pair.GetFirst();
		Text temp1 = pair.GetSecond();
		temp.value++;
		Image img = temp.gameObject.GetComponent<Image>();
		img.fillAmount = temp.value / temp.maxValue;
		//img.color = Color.Lerp(Color.green, Color.red, 1f);
		temp1.text = "Current Level: " + (stats.playerUpgrades[upgradeName].currentUpgradeLevel >= stats.playerUpgrades[upgradeName].possibleUpgradeLevels ? "Maximum!" : stats.playerUpgrades[upgradeName].currentUpgradeLevel.ToString());
		pointsAvailable.text = "Available upgrade points: " + stats.upgradeInfo.upgradePoints.ToString();
	}
}
