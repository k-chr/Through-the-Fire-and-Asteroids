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

    public Stats(float _health, float _damage, float _speed, float _turboMultiplier, float _damageReduction, float _shotSpeed, float _fireRate)
    {
        maxHealth = curHealth = _health;
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

public struct UpgradeData
{
    public uint currentExperience;
    public uint targetExperience;
    public uint[] upgradeExpValues;
    public uint upgradePoints;
    public uint baseExp;
    public int index;

    public UpgradeData(object obj)
    {
        upgradeExpValues = new uint[20];
        upgradeExpValues[0] = baseExp = 250;
        for (int i = 1; i < upgradeExpValues.Length; ++i)
        {
            upgradeExpValues[i] = (uint)(baseExp + Mathf.Pow(i, 2) * Mathf.Sqrt(baseExp) * Mathf.Pow(Mathf.PI, Mathf.PI));

        }
        upgradePoints = currentExperience = 0;
        targetExperience = baseExp;
        index = 0;
    }

    public UpgradeData(UpgradeData other)
    {
        this.baseExp = other.baseExp;
        this.upgradePoints = other.upgradePoints;
        this.currentExperience = other.currentExperience;
        this.targetExperience = other.targetExperience;
        this.index = other.index;
        this.upgradeExpValues = new uint[other.upgradeExpValues.Length];
        System.Array.Copy(other.upgradeExpValues, 0, this.upgradeExpValues, 0, other.upgradeExpValues.Length);
    }

    public static UpgradeData operator ++(UpgradeData x)
    {
        if (x.index == x.upgradeExpValues.Length) return x;
        UpgradeData _x = new UpgradeData(x);
        ++_x.upgradePoints;
        try
        {
            _x.targetExperience = _x.upgradeExpValues[++_x.index];
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log("MAX LEVEL!");
        }
        _x.currentExperience = 0;
        return _x;
    }

    public static UpgradeData operator --(UpgradeData x)
    {
        UpgradeData _x = new UpgradeData(x);
        if (_x.upgradePoints > 0) --_x.upgradePoints;
        return _x;
    }

    public override string ToString()
    {
        return upgradePoints.ToString();
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
		points += kills * 2;
	}
}
//**********************STATYSTYKI***********************

public class ShipStats : MonoBehaviour
{
    public static Dictionary<ShipType, Stats> statsInitializer = new Dictionary<ShipType, Stats>();
    public Dictionary<string, Upgrade> playerUpgrades = new Dictionary<string, Upgrade>();
    public Dictionary<string, float> upgradeValues = new Dictionary<string, float>();
    public Stats _shipStats = new Stats();
    public UpgradeData upgradeInfo = new UpgradeData(null);
    public MatchStatistics _matchStats = new MatchStatistics(0,0,0);
    public static string[] upgradeNames = new string[] { "damage", "speed", "damageReduction", "health" };
    public ShipType type;

    void Awake()
    {
        UpgradeValuesInit();
        UpgradesInit();
        ShipDataDump();
        StatsDump();
        StatsInit(this.type);
    }

    void StatsInit(ShipType pChoice)
    {
        if (!System.Enum.IsDefined(typeof(ShipType), pChoice)) throw new System.InvalidOperationException();
        TextAsset reader = Resources.Load("JsonFiles/shipStats") as TextAsset;
        statsInitializer = JsonConvert.DeserializeObject<Dictionary<ShipType, Stats>>(reader.text);
        _shipStats = statsInitializer[pChoice];
    }

    void UpgradeValuesInit()
    {
        upgradeValues["damage"] = 5f;
        upgradeValues["speed"] = 1.5f;
        upgradeValues["damageReduction"] = 5f;
        upgradeValues["health"] = 15f;
    }

    void StatsDump()
    {
        FileDump.SerializeToFile(
            JsonConvert.SerializeObject(statsInitializer, Formatting.Indented),
            @"Assets/Resources/JsonFiles/shipStats.json"
        );
    }

    void AssignType(ShipType type)
    {
        this.type = type;
    }

    void UpgradesDump()
    {
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
        Dictionary<string, Upgrade> temp = new Dictionary<string, Upgrade>();
        for (int i = 0; i < upgradeNames.Length; ++i)
        {
            temp.Add(upgradeNames[i], structs[i]);
        }
        FileDump.SerializeToFile(
            JsonConvert.SerializeObject(temp, Formatting.Indented),
            @"Assets/Resources/JsonFiles/upgrades.json"
        );
    }

    void UpgradesInit()
    {
        FileStream fs = FileDump.CreateFile(@"Assets/Resources/JsonFiles/upgrades.json");
        using (StreamReader reader = new StreamReader(fs))
        {
            playerUpgrades = JsonConvert.DeserializeObject<Dictionary<string, Upgrade>>(reader.ReadToEnd());
        }
        fs.Close();
    }

    public void UpgradeShip(string upgradeName, string fieldName)
    {
#if (DEBUG)
        Debug.Log("DEBUGGING REFLECTION");
        foreach (var f in _shipStats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            Debug.Log(f.GetValue(_shipStats).ToString());
        }
#endif
        /* actual function's body */
        try
        {
            ++playerUpgrades[upgradeName];
            --upgradeInfo;
            CalculateNewStats(upgradeName, fieldName);

        }
        catch (System.InvalidOperationException)
        {
            Debug.Log("Maximum upgrade level already");
        }
        /* end of body */
#if (DEBUG)
        Debug.Log("DEBUGGING IF IT WAS SUCCESSFUL");
        foreach (var f in _shipStats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            Debug.Log(f.GetValue(_shipStats).ToString());
        }
#endif
    }

    void ShipDataDump()
    {
        ShipType[] types = System.Enum.GetValues(typeof(ShipType)) as ShipType[];
        Stats temp = new Stats();
        //heavy ship
        temp.damage = 15f;
        temp.speed = 2f;
        temp.turboMultiplier = 1.2f;
        temp.damageReduction = 30f;
        temp.shotSpeed = 60f;
        temp.curHealth = temp.maxHealth = 250f;
        temp.fireRate = 0.5f;
        statsInitializer[types[0]] = temp;
        //fast ship
        temp.damage = 20f;
        temp.speed = 7f;
        temp.turboMultiplier = 1.8f;
        temp.damageReduction = 15f;
        temp.shotSpeed = 60f;
        temp.curHealth = temp.maxHealth = 125f;
        temp.fireRate = 0.35f;
        statsInitializer[types[1]] = temp;
        //balanced ship
        temp.damage = 12f;
        temp.speed = 4f;
        temp.turboMultiplier = 1.5f;
        temp.damageReduction = 20f;
        temp.shotSpeed = 100f;
        temp.curHealth = temp.maxHealth = 150f;
        temp.fireRate = 0.4f;
        statsInitializer[types[2]] = temp;
        StatsDump();
    }

    void CalculateNewStats(string upgradeName, string fieldName)
    {
        FieldInfo field = _shipStats.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        FieldInfo f = _shipStats.GetType().GetField("maxHealth", BindingFlags.Instance | BindingFlags.Public);
        if (upgradeName == "health")
        {
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

