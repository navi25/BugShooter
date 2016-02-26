using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Gamestrap
{
    /// <summary>
    /// Scriptable Object incharge of saving all of the UI settings of Gamestrap UI Toolkit
    /// </summary>
    public class GamestrapTheme : ScriptableObject
    {
        public List<ColorSet> palette = new List<ColorSet>();
        public List<EffectSet> effectSets = new List<EffectSet>();
    }
    /// <summary>
    /// ColorSet is a color in the pallete of the Gamestrap UI Toolkit which handles the colors to be set to any UI in the scene
    /// </summary>
    [System.Serializable]
    public class ColorSet
    {
        public string name = "";
        public string tag = "";
        public Color normal;
        public Color highlighted;
        public Color pressed;
        public Color disabled;
        public Color detail;

        public ColorSet()
        {

        }

        public ColorSet(string name, string tag, Color normal, Color highlighted, Color pressed, Color disabled, Color detail){
            this.name = name;
            this.tag = tag;
            this.normal = normal;
            this.highlighted = highlighted;
            this.pressed = pressed;
            this.disabled = disabled;
            this.detail = detail;
        }

        public ColorSet(Color[] colors)
        {
            if (colors.Length >= 5)
            {
                normal = colors[0];
                highlighted = colors[1];
                pressed = colors[2];
                disabled = colors[3];
                detail = colors[4];
            }
        }
        
        public ColorSet Clone()
        {
            return (ColorSet)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// Effect is a set of effects in the Gamestrap UI Toolkit which handles the effects to be set to any UI in the scene
    /// </summary>
    [System.Serializable]
    public class EffectSet
    {
        public string name = "";
        public string tag = "";

        public bool gradient;
        public Color gradientTop = Color.white;
        public Color gradientBottom = Color.black;

        public bool shadow;
        public Color shadowColor = Color.black;
        public Vector2 shadowOffset = new Vector2(2,-2);

        public EffectSet Clone()
        {
            return (EffectSet)this.MemberwiseClone();
        }
    }
}