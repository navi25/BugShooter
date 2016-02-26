using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Custom Menu Items to create the prefabs directly from the Unity menu
/// </summary>
public static class GamestrapUIPrefabs
{
    private const string dir = "GameObject/Gamestrap UI/";
    private const int priority = 10;

    [MenuItem(dir + "Button",false,priority)]
    static void CreateButton(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "Button");
    }

    [MenuItem(dir + "Button with Icon",false,priority)]
    static void CreateButtonIcon(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "ButtonIcon");
    }

    [MenuItem(dir + "Icon Button",false,priority)]
    static void CreateIconButton(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "IconButton");
    }

    [MenuItem(dir + "InputField",false,priority)]
    static void CreateInputField(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "InputField");
    }

    [MenuItem(dir + "Scrollbar",false,priority)]
    static void CreateScrollbar(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "Scrollbar");
    }

    [MenuItem(dir + "Scroll List", false, priority)]
    static void CreateScrollList(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "Scroll List");
    }

    [MenuItem(dir + "Slider",false,priority)]
    static void CreateSlider(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "Slider");
    }

    [MenuItem(dir + "Toggle Slider",false,priority)]
    static void CreateSliderToggle(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "SliderToggle");
    }

    [MenuItem(dir + "Toggle Check",false,priority)]
    static void CreateToggleCheck(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "ToggleCheck");
    }

    [MenuItem(dir + "Toggle Radio Button",false,priority)]
    static void CreateToggleRadio(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "ToggleRadio");
    }

#if UNITY_5_2
        [MenuItem(dir + "Dropdown", false, priority)]
    static void CreateDropdown(MenuCommand menuCommand)
    {
        InstantiateGameObject(menuCommand, "Dropdown");
    }
#endif

    static void InstantiateGameObject(MenuCommand menuCommand, string name)
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(GamestrapHelper.gamestrapRoute + "Prefabs/" + name + ".prefab", typeof(GameObject));
        if (!prefab)
        {
            Debug.LogError("Prefab '" + name + ".prefab' missing from '" + GamestrapHelper.gamestrapRoute + "Prefabs/', make sure you have placed the prefab where it should be at.");
            return;
        }
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(prefab);


        if (menuCommand.context)
        {
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        }
        else if (Selection.activeObject is GameObject) {
            GameObjectUtility.SetParentAndAlign(go, Selection.activeObject as GameObject);
        }

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
