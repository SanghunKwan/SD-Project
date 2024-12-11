using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class EquipDataManager : JsonLoad
{
    #region dataClass
    [Serializable]
    public class EquipValueData
    {
        public int quality;
        public TypeNum type;

        public int HP;
        public int ATK;
        public int DOG;
        public int SPEED;
        public int ViewAngle;
        public int ViewRange;
        public int Accuracy;
        public float AtkSpeed;
        public int Range;
        public int Mentality;
        public int Stress;


    }
    [Serializable]
    class EquipValueDatas
    {
        public EquipValueData[] equipValueDatas = null;
    }
    EquipValueDatas equipValueData;
    #endregion

    private void Start()
    {
        Data.Instance.equipDataManager = this;

        equipValueData = LoadData<EquipValueDatas>("EquipStatus");
    }
    public EquipValueData GetEquipData(int index)
    {
        return equipValueData.equipValueDatas[index];
    }
    public override void Init()
    {
        throw new NotImplementedException();
    }
}
