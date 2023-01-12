using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsData : ScriptableObject
{
    [Serializable]
    public class ColorsData
    {
        public int maxColors = 6;
        public List<Material> ColorsList = new List<Material>();
        public Material DefaultBoardColor;
        public Material SelectedLineColor;
        public Material WhiteColor;
        public Material BlackColor;

    }
    [SerializeField]
    ColorsData colors = new ColorsData();
    public ColorsData Colors { get { return colors; } }

    [Serializable]
    public class PrefabsData
    {
        public GameObject slotPrefab;
        public GameObject slotX4Prefab;
        public GameObject colorChoicePrefab;
        public GameObject OkButtonPrefab;
    }
    [SerializeField]
    PrefabsData prefabs = new PrefabsData();
    public PrefabsData Prefabs { get { return prefabs; } }

    [Serializable]
    public class RulesData
    {
        public int slotsByLine = 4;
        public int linesByBoard = 12;

        public bool sameColorByLine = true;
    }
    [SerializeField]
    RulesData rules = new RulesData();
    public RulesData Rules { get { return rules; } }
}
