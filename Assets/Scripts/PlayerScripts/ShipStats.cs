using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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
    public float health;
    public float damage;
	public float speed;
	public float turboMultiplier;
	public float damageReduction;
	public float shotSpeed;
    public float fireRate;

    public Stats(float _health, float _damage, float _speed, float _turboMultiplier, float _damageReduction, float _shotSpeed, float _fireRate)
    {
        health = _health;
        damage = _damage;
        speed = _speed;
        turboMultiplier = _turboMultiplier;
        damageReduction = _damageReduction;
        shotSpeed = _shotSpeed;
        fireRate = _fireRate;
    }
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

	public static bool CheckUpgradeLevel(Upgrade upgr){
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

    public MatchStatistics(uint _kills, uint _deaths, uint _points)
    {
        kills = _kills;
        deaths = _deaths;
        points = _points;
    }

    public void UpdateKills()
    {
        ++kills;
    }

    public void UpdateDeaths()
    {
        ++deaths;
    }

	public void UpdatePoints() {
		points = kills * 2;
	}
}

public class ShipStats : MonoBehaviour {

	public static Dictionary<ShipType, Stats> statsInitializer = new Dictionary<ShipType, Stats>();
	public Dictionary<string, Upgrade> playerUpgrades = new Dictionary<string, Upgrade>();

    [SerializeField]
	public Stats _shipStats = new Stats(100f, 100f, 5f, 2f, 0.5f, 40f, 4f);
    public MatchStatistics _matchStats = new MatchStatistics(0, 0, 0);

	public string[] upgradeNames = new string[] {"damage", "speed", "damageReduction", "health"};

	public ShipType type;
	
	void Start () {}

	void StatsInit(ShipType pChoice) {
		if (!System.Enum.IsDefined(typeof(ShipType), pChoice)) throw new System.InvalidOperationException();
		using (StreamReader reader = new StreamReader("JsonFiles/shipStats.json")) {
			statsInitializer = JsonConvert.DeserializeObject<Dictionary<ShipType, Stats>>(reader.ReadToEnd());
		}
		_shipStats = statsInitializer[pChoice];
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

}
