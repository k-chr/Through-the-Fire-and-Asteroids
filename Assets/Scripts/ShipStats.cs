using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

public enum ShipType {
	Heavy = 0,
	Fast,
	Balanced
}

public enum UpgradeType {
	damage = 0,
	speed, 
	damageReduction,
	health
}

public struct Stats {
	public float damage;
	public float speed;
	public float turboMultiplier;
	public float damageReduction;
	public float shotSpeed;
	public float maxHealth;
	public float curHealth;
	public float fireRate;
}

public struct Upgrade {
	public UpgradeType upgrType;
	public uint possibleUpgradeLevels;
	public uint currentUpgradeLevel;

	public Upgrade(Upgrade other) {
		this.upgrType = other.upgrType;
		this.possibleUpgradeLevels = other.possibleUpgradeLevels;
		this.currentUpgradeLevel = other.currentUpgradeLevel;
	}

	public static Upgrade operator++(Upgrade other) {
		if (!CheckUpgradeLevel(other)) throw new System.InvalidOperationException();
		Upgrade x = new Upgrade(other);
		++x.currentUpgradeLevel;
		return x;
	}

	public static bool CheckUpgradeLevel(Upgrade upgr) {
		if (upgr.currentUpgradeLevel >= upgr.possibleUpgradeLevels) return false;
		return true;
	} 
}

public struct UpgradeData {
	public uint currentExperience;
	public uint targetExperience;
	public uint[] upgradeExpValues;
	public uint upgradePoints;
	public uint baseExp;

	public UpgradeData(object obj) {
		upgradeExpValues = new uint[20];
		upgradeExpValues[0] = baseExp = 250;
		for (int i = 1; i < upgradeExpValues.Length; ++i) {
			upgradeExpValues[i] = (uint)(baseExp + Mathf.Pow(i, 2) * Mathf.Sqrt(baseExp) * Mathf.Pow(Mathf.PI, Mathf.PI));
			
		}
		upgradePoints = currentExperience = 0;
		targetExperience = baseExp;
	}
}

public struct MatchStatistics {
	public uint kills;
	public uint deaths;
	public uint points;

	public void UpdatePoints() {
		points = kills * 2;
	}
}

public class ShipStats : MonoBehaviour {

	public static Dictionary<ShipType, Stats> statsInitializer = new Dictionary<ShipType, Stats>();
	public Dictionary<string, Upgrade> playerUpgrades = new Dictionary<string, Upgrade>();
	public Dictionary<string, float> upgradeValues = new Dictionary<string, float>();
	public Stats _shipStats = new Stats();

	public string[] upgradeNames = new string[] {"damage", "speed", "damageReduction", "health"};

	public ShipType type;
	
	void Start () {
		//StatsInit(this.type);
	}

	void StatsInit(ShipType pChoice) {
		if (!System.Enum.IsDefined(typeof(ShipType), pChoice)) throw new System.InvalidOperationException();
		using (StreamReader reader = new StreamReader("JsonFiles/shipStats.json")) {
			statsInitializer = JsonConvert.DeserializeObject<Dictionary<ShipType, Stats>>(reader.ReadToEnd());
		}
		_shipStats = statsInitializer[pChoice];
	}

	void UpgradeValuesInit() {
		upgradeValues["damage"] = 5f;
		upgradeValues["speed"] = 1.5f;
		upgradeValues["damageReduction"] = 5f;
		upgradeValues["health"] = 15f;
	}

	void StatsDump() {
		FileDump.SerializeToFile(
			JsonConvert.SerializeObject(statsInitializer, Formatting.Indented),
			@"JsonFiles/shipStats.json"
		);
	}

	void AssignType(ShipType type) {
		this.type = type;
	}

	void UpgradesDump() {
		Upgrade[] structs = new Upgrade[upgradeNames.Length];//0 - damage, 1 - speed, 2 - damage reduction, 3 - more health
		structs[0].upgrType = UpgradeType.damage;
		structs[1].upgrType = UpgradeType.speed;
		structs[2].upgrType = UpgradeType.damageReduction;
		structs[3].upgrType = UpgradeType.health;
		structs[0].currentUpgradeLevel = structs[1].currentUpgradeLevel = structs[2].currentUpgradeLevel = structs[3].currentUpgradeLevel = 0;
		structs[0].possibleUpgradeLevels = 6;
		structs[1].possibleUpgradeLevels = 6;
		structs[2].possibleUpgradeLevels = 3;
		structs[3].possibleUpgradeLevels = 3;
		FileDump.SerializeToFile(
			JsonConvert.SerializeObject(structs, Formatting.Indented),
			@"JsonFiles/upgrades.json"
		);
	}

	public void UpgradeShip(string upgradeName, string fieldName) {
		try {
			++playerUpgrades[upgradeName];
			CalculateNewStats(upgradeName, fieldName);
		} catch (System.InvalidOperationException) {
			Debug.Log("Maximum upgrade level already");
		}
	}

	void ShipDataDump() {
		ShipType[] types = System.Enum.GetValues(typeof(ShipType)) as ShipType[];
		Stats temp = new Stats();
		//heavy ship
		temp.damage = 15f;
		temp.speed = 2f;
		temp.turboMultiplier = 1.2f;
		temp.damageReduction = 30f;
		temp.shotSpeed = 3f;
		temp.curHealth = temp.maxHealth = 250f;
		temp.fireRate = 0.5f;
		statsInitializer[types[0]] = temp;
		//fast ship
		temp.damage = 20f;
		temp.speed = 7f;
		temp.turboMultiplier = 1.8f;
		temp.damageReduction = 15f;
		temp.shotSpeed = 3f;
		temp.curHealth = temp.maxHealth = 125f;
		temp.fireRate = 0.35f;
		statsInitializer[types[1]] = temp;
		//balanced ship
		temp.damage = 12f;
		temp.speed = 4f;
		temp.turboMultiplier = 1.5f;
		temp.damageReduction = 20f;
		temp.shotSpeed = 5f;
		temp.curHealth = temp.maxHealth = 150f;
		temp.fireRate = 0.4f;
		statsInitializer[types[2]] = temp;
		StatsDump();
	}

	void CalculateNewStats(string upgradeName, string fieldName) {
		FieldInfo field = _shipStats.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
		FieldInfo f = _shipStats.GetType().GetField("maxHealth", BindingFlags.Instance | BindingFlags.Public);
		if (upgradeName == "health") {
			float maxHealthVal = (float)f.GetValue(_shipStats);
			maxHealthVal += upgradeValues[upgradeName];
			object temp1 = _shipStats;
			f.SetValue(temp1, maxHealthVal);
			_shipStats = (Stats)temp1;
		}
		object temp = _shipStats;
		float val = (float)field.GetValue(_shipStats);
		val += upgradeValues[upgradeName];
		field.SetValue(temp, val);
		_shipStats = (Stats)temp;
	}
}
