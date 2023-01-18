using System.Collections.Generic;
using UnityEngine;


public enum DessertType
{
    None,
    Truffle,
    Chocoteja
}

[System.Serializable]
public class AppData
{
    public Dictionary<DessertKey, DessertData> TrufflesTasteDic_Data = new Dictionary<DessertKey, DessertData>();

    public Dictionary<string, RequestData> RequestsDic_Data = new Dictionary<string, RequestData>();

    public AppData(Dictionary<DessertKey, DessertData> trufflesdic, Dictionary<string, RequestData> requestDic)
    {
        TrufflesTasteDic_Data = trufflesdic;
        RequestsDic_Data = requestDic;
    }
}

[System.Serializable]
public class RequestData
{
    public string date;

    public string time;

    public List<TasteSettingInfo> tasteSettingsInfoList = new List<TasteSettingInfo>();

    public int totalAmount;

    public float totalCost;

    public float deliveryCost;

    public int notificationID;

    public RequestData(string date, string time, int totalCount, float deliveryCost, float totalCost, List<TasteSettingInfo> tasteSettingsInfoList, int notificationID)
    {
        this.date = date;
        this.time = time;
        this.totalAmount = totalCount;
        this.tasteSettingsInfoList = tasteSettingsInfoList;
        this.deliveryCost = deliveryCost;
        this.totalCost = totalCost;
        this.notificationID = notificationID;
    }
}

[System.Serializable]
public class DessertKey
{
    public string name;
    public DessertType dessertType;

    public DessertKey(string name, DessertType dessertType)
    {
        this.name = name;
        this.dessertType = dessertType;
    }

    public bool Equals(DessertKey other)
    {
        if (name != other.name) return false;
        if (dessertType != other.dessertType) return false;

        return true;
    }


    public static DessertKey Create(string name, DessertType dessertType)
    {
        return new DessertKey(name, dessertType);
    }
}

[System.Serializable]
public class DessertData
{
    public ColorInfo colorInfo;

    public float cost;

    public DessertData(float cost, Color unityColor)
    {
        this.cost = cost;
        colorInfo = new ColorInfo(unityColor);
    }
}

[System.Serializable]
public class TasteSettingInfo
{
    public DessertKey key;
    public DessertData dessertData;
    public int amount;

    public TasteSettingInfo(DessertKey key, DessertData dessertData, int amount)
    {
        this.key = key;
        this.dessertData = dessertData;
        this.amount = amount;
    }
}





[System.Serializable]
public class ColorInfo
{
    public float R;
    public float G;
    public float B;
    public float A;

    public ColorInfo(Color unityColor)
    {
        R = unityColor.r;
        G = unityColor.g;
        B = unityColor.b;
        A = unityColor.a;
    }

    public Color ToUnityColor()
    {
        return new Color(R, G, B, A);
    }
}
