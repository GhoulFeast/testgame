using System;
using UnityEngine;

public enum ItemType
{
    Material,//����
    Consumables,//����Ʒ
    ExtEquip,
    Armed_A_Equip,
    Armed_B_Equip,
    Exoskeleton_Equip,//�����
    Reactor_Equip,//��Ӧ��
    EnergyStorageCore_Equip,//���ܵ�о
    auxiliaryChip_Equip,
}

public class ItemInfo
{
  

    public string id;
    public string name="";
    public ItemType type;
    public string note="";
    public int num=0;
    public float weight=0;
    public float cost;
    public bool isUse=true;
    public Sprite sprite;
    public int maxNum;
}