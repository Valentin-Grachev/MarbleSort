using System;
using System.Collections.Generic;
using UnityEngine;
using UNodes.Extensions;


public enum ColorType
{
    Red = 0,
    Orange = 1,
    Green = 2,
    Blue = 3,
    Yellow = 4,
}


[CreateAssetMenu(menuName = "Configs/Colors", fileName = "Colors")]
public class ColorsConfig : ScriptableObject
{
    [Serializable] private struct ColorSetting
    {
        public ColorType colorType;
        public Color color;
    }

    [SerializeField] private List<ColorSetting> _colorSettings;


    public Color GetColor(ColorType colorType) => 
        _colorSettings.FindOrExeption(setting => setting.colorType == colorType).color;


}
