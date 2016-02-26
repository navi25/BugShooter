using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Gamestrap
{
    [Serializable]
    public class GamestrapUI : EditorWindow
    {
        #region constand and static variables
        private const string prefsKey = "GamestrapUITheme";
        private static GamestrapUI window;
        private static Color deleteColor = new Color(0.75f, 0.25f, 0.25f);
        private static Color applyColor = new Color(0.25f, 0.5f, 0.25f);
        #endregion

        #region Editor variables

        private GamestrapTheme theme;
        private ColorSet selectedColorSet;
        private EffectSet selectedEffectSet;
        public int rowCount = 6; // Row count in palette

        private Font font;

        private bool showColors = false;
        private bool showSuggestions = false;
        private bool showSceneColors = false;
        private bool showGlobalAction = false;

        private bool _expanded;
        private Color selectedColor = GamestrapHelper.ColorRGBInt(31, 153, 200);
        private Color defaultBg;
        private List<Color[]> colors;
        private List<Color> sceneColors;
        private SchemeType paletteType;
        private GUIStyle btnStyle, btnSelectedStyle, btnLinestyle, bgStyle, titleStyle, subtitleStyle, btnStyleWhiteFont;
        private Vector2 scrollPos;
        private int recursiveLevel;
        #endregion

        [MenuItem("Window/Gamestrap UI Kit")]
        public static void ShowWindow()
        {
            window = (GamestrapUI)EditorWindow.GetWindow(typeof(GamestrapUI), false, "GS Theme Kit");
            window.minSize = new Vector2(300f, 305f);
        }

        void OnEnable()
        {
            //Load initila variables
            SetColors(GetColorDefault(GamestrapHelper.ColorRGBInt(141, 39, 137)));
            font = (Font)AssetDatabase.LoadAssetAtPath(GamestrapHelper.gamestrapRoute + "Fonts/Lato/Lato-Regular.ttf", typeof(Font));
            SetUpColors();
            sceneColors = new List<Color>();
            if (theme == null)
            {
                FindTheme();
            }
            selectedColorSet = null;
            selectedEffectSet = null;
            defaultBg = GUI.backgroundColor;
        }

        void OnGUI()
        {
            if (theme == null)
            {
                FindTheme();
            }

            //Checks if there is any style not initialized
            SetStyles();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(theme, "Content change in theme");

            GUILayout.BeginHorizontal();
            GamestrapTheme newTheme = (GamestrapTheme)EditorGUILayout.ObjectField(theme, typeof(GamestrapTheme), false);

            if (theme != newTheme)
            {
                string assetPathAndName = AssetDatabase.GetAssetPath(newTheme);
                EditorPrefs.SetString(prefsKey, assetPathAndName);
            }

            theme = newTheme;

            //Save as button UI Logic
            SaveAs();

            GUILayout.EndHorizontal();

            #region Palette GUI
            GUILayout.Label("Palette", titleStyle);
            GUILayout.Label("Right click to assign color to selection");
            
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            int counter = 0;
            foreach (ColorSet colorSet in theme.palette)
            {
                GUI.backgroundColor = colorSet.normal; // Sets the button color
                if (GUILayout.Button("", (colorSet == selectedColorSet) ? btnSelectedStyle : btnStyle, GUILayout.Width((position.width - 35f) / rowCount), GUILayout.Height(40)))
                {
                    // To remove focus from any input field which might lead to errors
                    GUI.FocusControl("Null");
                    if (Event.current.button == 1)
                    {
                        AssignColorsToSelection(colorSet);
                    }
                    else if (Event.current.button == 0)
                    {
                        if (colorSet == selectedColorSet)
                        {
                            selectedColorSet = null;
                        }
                        else
                        {
                            selectedColorSet = colorSet;
                        }
                    }

                }
                counter++;
                if (counter % rowCount == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Add", btnStyle , GUILayout.Width((position.width - 20f) / rowCount), GUILayout.Height(40)))
            {
                GUI.FocusControl("Null");
                Color[] baseColor = colors[UnityEngine.Random.Range(0, colors.Count - 1)];
                selectedColorSet = new ColorSet(baseColor);

                theme.palette.Add(selectedColorSet);
            }
            GUI.backgroundColor = defaultBg;

            GUILayout.EndHorizontal();

            if (selectedColorSet != null)
            {
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

                GUILayout.BeginHorizontal();
                GUILayout.Label("Color Details", subtitleStyle);
                GUI.backgroundColor = deleteColor;
                if (GUILayout.Button("Delete", btnStyleWhiteFont, GUILayout.Width(50f)))
                {
                    theme.palette.Remove(selectedColorSet);
                    selectedColorSet = null;
                }
                GUI.backgroundColor = defaultBg;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    selectedColorSet = null;

                }
                GUILayout.EndHorizontal();
                if (selectedColorSet != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    selectedColorSet.name = EditorGUILayout.TextField(selectedColorSet.name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Tag", GUILayout.Width(100));
                    selectedColorSet.tag = EditorGUILayout.TagField(selectedColorSet.tag);
                    EditorGUILayout.EndHorizontal();

                    selectedColorSet.normal = EditorGUILayout.ColorField("Normal", selectedColorSet.normal);
                    selectedColorSet.highlighted = EditorGUILayout.ColorField("Highlighted", selectedColorSet.highlighted);
                    selectedColorSet.pressed = EditorGUILayout.ColorField("Pressed", selectedColorSet.pressed);
                    selectedColorSet.disabled = EditorGUILayout.ColorField("Disabled", selectedColorSet.disabled);
                    selectedColorSet.detail = EditorGUILayout.ColorField("Detail", selectedColorSet.detail);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Apply ColorSet to:", GUILayout.Width(125));
                    GUI.backgroundColor = applyColor;

                    if (GUILayout.Button("Selected",btnStyleWhiteFont))
                    {
                        AssignColorsToSelection(selectedColorSet);
                    }
                    GUI.backgroundColor = defaultBg;

                    GUI.enabled = selectedColorSet.tag.Length > 0 && selectedColorSet.tag != "Untagged";

                    if (GUILayout.Button("Tag"))
                    {
                        ApplyColorSetTag(selectedColorSet);
                    }

                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    #region Helper color selection
                    GUILayout.BeginHorizontal();
                    bool lastShowSceneColors = showSceneColors;
                    showColors = GUILayout.Toggle(showColors, "Suggestions", "Button");
                    showSuggestions = GUILayout.Toggle(showSuggestions, "Schemes", "Button");
                    showSceneColors = GUILayout.Toggle(showSceneColors, "Scene Colors", "Button");
                    // If the toggle was activated, refresh and search for the new colors in scene
                    if (showSceneColors != lastShowSceneColors && showSceneColors)
                    {
                        SearchSceneColors();
                    }
                    GUILayout.EndHorizontal();

                    if (showSceneColors)
                    {
                        GUILayout.Label("Scene Colors", EditorStyles.boldLabel);
                        GUI.backgroundColor = Color.black;
                        GUILayout.BeginVertical(bgStyle);
                        GUILayout.BeginHorizontal();
                        counter = 0;
                        foreach (Color color in sceneColors)
                        {
                            GUI.backgroundColor = color; // Sets the button color
                            if (GUILayout.Button("", btnStyle))
                            {
                                SetColors(GetColorDefault(color));
                                selectedColor = color;
                            }
                            counter++;
                            if (counter % 5 == 0)
                            {
                                // Start a new row each 5
                                GUILayout.EndHorizontal();
                                GUI.backgroundColor = Color.black;
                                GUILayout.BeginHorizontal();
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUI.backgroundColor = defaultBg; // Resets the color background

                        GUILayout.EndVertical();
                    }

                    if (showSuggestions)
                    {
                        GUILayout.Label("Color Scheme Generator", EditorStyles.boldLabel);
                        paletteType = (SchemeType)EditorGUILayout.EnumPopup("Scheme: ", paletteType);
                        selectedColor = EditorGUILayout.ColorField("Base:", selectedColor);

                        GUI.backgroundColor = Color.black;
                        GUILayout.BeginVertical(bgStyle);
                        GUI.backgroundColor = selectedColor; // Sets the button color
                        if (GUILayout.Button("", btnStyle))
                        {
                            SetColors(GetColorDefault(selectedColor));
                        }
                        Color[] paletteColors = GamestrapHelper.GetColorPalette(selectedColor, paletteType);
                        GUILayout.BeginHorizontal();
                        for (int i = 0; i < paletteColors.Length; i++)
                        {
                            GUI.backgroundColor = paletteColors[i]; // Sets the button color
                            if (GUILayout.Button("", btnStyle))
                            {
                                SetColors(GetColorDefault(paletteColors[i]));
                            }
                        }
                        GUI.backgroundColor = defaultBg; // Resets the color background
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }

                    if (showColors)
                    {
                        GUILayout.Label("Color Suggestions", EditorStyles.boldLabel);
                        GUI.backgroundColor = Color.black;
                        GUILayout.BeginVertical(bgStyle);
                        GUILayout.BeginHorizontal();
                        counter = 0;
                        foreach (Color[] color in colors)
                        {
                            GUI.backgroundColor = color[0]; // Sets the button color
                            if (GUILayout.Button("", btnStyle) && color.Length >= 5)
                            {
                                SetColors(color);
                                selectedColor = color[0];
                            }
                            counter++;
                            if (counter % 5 == 0)
                            {
                                // Start a new row each 5
                                GUILayout.EndHorizontal();
                                GUI.backgroundColor = Color.black;
                                GUILayout.BeginHorizontal();
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUI.backgroundColor = defaultBg; // Resets the color background

                        GUILayout.EndVertical();
                    }
                    #endregion

                }
            }
            EditorGUILayout.EndVertical();

            #endregion

            #region Effects
            GUILayout.Label("Effects", titleStyle);
            EditorGUILayout.BeginVertical("Box");
            Color effectBackground = new Color(0.6f, 0.6f, 0.6f);
            for (int i = 0 ; i < theme.effectSets.Count ; i++)
            {
                EffectSet set = theme.effectSets[i];
                bool pressed = (set == selectedEffectSet);
                GUI.backgroundColor = effectBackground;
                bool newPressed = GUILayout.Toggle(pressed, set.name, "Button");
                GUI.backgroundColor = defaultBg;
                if (pressed != newPressed)
                {
                    // To remove focus from any input field which might lead to errors
                    GUI.FocusControl("Null");
                    if (newPressed)
                    {
                        if (Event.current.button == 1)
                        {
                            ActivateEffects(set);
                        }
                        else if (Event.current.button == 0)
                        {
                            selectedEffectSet = set;
                        }
                    }
                    else if (Event.current.button == 0)
                    {
                        selectedEffectSet = null;
                    }
                }
                if (selectedEffectSet != null && set == selectedEffectSet)
                {
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Effect Details", subtitleStyle);
                    GUI.backgroundColor = deleteColor;
                    if (GUILayout.Button("Delete", btnStyleWhiteFont, GUILayout.Width(50f)))
                    {
                        theme.effectSets.Remove(selectedEffectSet);
                        selectedEffectSet = null;
                        i--;
                    }
                    GUI.backgroundColor = defaultBg;
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        selectedEffectSet = null;
                        
                    }
                    GUILayout.EndHorizontal();

                    if (selectedEffectSet != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Name", GUILayout.Width(75));
                        selectedEffectSet.name = EditorGUILayout.TextField(selectedEffectSet.name);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Tag", GUILayout.Width(75));
                        selectedEffectSet.tag = EditorGUILayout.TagField(selectedEffectSet.tag);
                        EditorGUILayout.EndHorizontal();

                        selectedEffectSet.shadow = EditorGUILayout.ToggleLeft("Shadow", selectedEffectSet.shadow);
                        if (selectedEffectSet.shadow)
                        {
                            selectedEffectSet.shadowColor = EditorGUILayout.ColorField("Color", selectedEffectSet.shadowColor);
                            selectedEffectSet.shadowOffset = EditorGUILayout.Vector2Field("Effect Distance", selectedEffectSet.shadowOffset);
                        }

                        selectedEffectSet.gradient = EditorGUILayout.ToggleLeft("Gradient", selectedEffectSet.gradient);
                        if (selectedEffectSet.gradient)
                        {
                            selectedEffectSet.gradientTop = EditorGUILayout.ColorField("Color Top", selectedEffectSet.gradientTop);
                            selectedEffectSet.gradientBottom = EditorGUILayout.ColorField("Color Bottom", selectedEffectSet.gradientBottom);
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Apply EffectSet to:", GUILayout.Width(125));
                        GUI.backgroundColor = applyColor;
                        if (GUILayout.Button("Selection", btnStyleWhiteFont))
                        {
                            ActivateEffects(selectedEffectSet);
                        }
                        GUI.backgroundColor = defaultBg;
                        GUI.enabled = selectedEffectSet.tag.Length > 0 && selectedEffectSet.tag != "Untagged";
                        if (GUILayout.Button("Tag"))
                        {
                            ApplyEffectSetTag(selectedEffectSet);
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }
                }
            }

            if (GUILayout.Button("Add Effect Set", GUILayout.Height(35)))
            {
                GUI.FocusControl("Null");
                selectedEffectSet = new EffectSet();
                selectedEffectSet.name = "New Effect";
                theme.effectSets.Add(selectedEffectSet);
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Font
            GUILayout.Label("Font", titleStyle);
            GUILayout.BeginHorizontal();
            font = (Font)EditorGUILayout.ObjectField(font, typeof(Font), false);
            if (GUILayout.Button("Apply"))
            {
                AssignFontToSelection();
            }
            GUILayout.EndHorizontal();
            #endregion

            #region Additional Options
            showGlobalAction = EditorGUILayout.Foldout(showGlobalAction, "Additional Options");

            if (showGlobalAction)
            {
                GUI.backgroundColor = applyColor;
                if (GUILayout.Button("Apply theme to Tags in scene", btnStyleWhiteFont))
                {
                    foreach (ColorSet set in theme.palette)
                    {
                        ApplyColorSetTag(set);
                    }

                    foreach (EffectSet set in theme.effectSets)
                    {
                        ApplyEffectSetTag(set);
                    }
                }
                GUI.backgroundColor = defaultBg;
                if (GUILayout.Button("Generate Palette From Scene"))
                {
                    GetSceneColors();
                    foreach (Color c in sceneColors)
                    {
                        ColorSet cs = new ColorSet();
                        Color[] colors = GetColorDefault(c);
                        cs.normal = colors[0];
                        cs.highlighted = colors[1];
                        cs.pressed = colors[2];
                        cs.disabled = colors[3];
                        cs.detail = colors[4];
                        theme.palette.Add(cs);
                    }
                }
                GUI.backgroundColor = deleteColor;
                if (GUILayout.Button("Clear palette and effects", btnStyleWhiteFont))
                {
                    theme.palette.Clear();
                    theme.effectSets.Clear();
                }
                GUI.backgroundColor = defaultBg;
            }
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(theme);
            }
            EditorGUILayout.EndScrollView();
        }

        #region Theme creating/Finding methods
        private void CreateTheme()
        {
            theme = ScriptableObject.CreateInstance<GamestrapTheme>();
            theme.palette = new List<ColorSet>();
            theme.effectSets = new List<EffectSet>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Gamestrap UI/Basic Theme.asset");
            AssetDatabase.CreateAsset(theme, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorPrefs.SetString(prefsKey, assetPathAndName);
        }

        void FindTheme()
        {
            if (EditorPrefs.HasKey(prefsKey))
            {
                theme = (GamestrapTheme)AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString(prefsKey), typeof(GamestrapTheme));
                
            }
            if (theme == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:GamestrapTheme");
                if (guids.Length > 0)
                {
                    string assetPathAndName = AssetDatabase.GUIDToAssetPath(guids[0]);
                    theme = (GamestrapTheme)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(GamestrapTheme));
                    EditorPrefs.SetString(prefsKey, assetPathAndName);
                }
                else
                {
                    CreateTheme();
                }
            }
        }

        [MenuItem("Assets/Create/Gamestrap UI Theme")]
        public static void CreateAsset()
        {
            GamestrapTheme t = CreateAsset<GamestrapTheme>();
            t.palette = new List<ColorSet>();
            t.effectSets = new List<EffectSet>();
        }

        public static T CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
        #endregion

        #region Tag Methods

        void ApplyColorSetTag(ColorSet set)
        {
            if (set.tag.Length > 0 && set.tag != "Untagged")
            {
                GameObject[] list = GameObject.FindGameObjectsWithTag(set.tag);
                foreach (GameObject go in list)
                {
                    recursiveLevel = 0;
                    AssignColorsToSelection(go, set);
                }
            }
        }

        void ApplyEffectSetTag(EffectSet set)
        {
            if (set.tag.Length > 0 && set.tag != "Untagged")
            {
                GameObject[] list = GameObject.FindGameObjectsWithTag(set.tag);
                foreach (GameObject go in list)
                {
                    UIBehaviour[] effects;
                    #if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
                    effects = go.GetComponents<BaseVertexEffect>();
                    #else
                    effects = go.GetComponents<BaseMeshEffect>();
                    #endif
                    foreach (UIBehaviour behaviour in effects)
                    {
                        Undo.DestroyObjectImmediate(behaviour);
                    }

                    ActivateEffects(go, set);
                    go.SetActive(false);
                    go.SetActive(true);
                }
            }
        }


        #endregion

        void OnInspectorUpdate()
        {
            //Checks to see if an undo changed the selected Sets
            if (theme != null)
            {
                if (selectedColorSet != null && !theme.palette.Contains(selectedColorSet))
                {
                    selectedColorSet = null;
                    // To update the undos right away
                    Repaint();
                }
                if (selectedEffectSet != null && !theme.effectSets.Contains(selectedEffectSet))
                {
                    selectedEffectSet = null;
                    // To update the undos right away
                    Repaint();
                }
            }
        }

        void SaveAs()
        {
            if (theme == null)
            {
                return;
            }

            if (GUILayout.Button("Save as...", btnLinestyle))
            {
                string themePath = AssetDatabase.GetAssetPath(theme.GetInstanceID());
                themePath = themePath.Substring(0, themePath.LastIndexOf('/') + 1);

                string path = EditorUtility.SaveFilePanel("Save Theme", themePath, "New Theme.asset", "asset");
                if (path.Length != 0)
                {
                    path = path.Substring(path.IndexOf("Assets/"));
                    GamestrapTheme newTheme = ScriptableObject.CreateInstance<GamestrapTheme>();
                    foreach (ColorSet cs in theme.palette)
                    {
                        newTheme.palette.Add(cs.Clone());
                    }
                    foreach (EffectSet es in theme.effectSets)
                    {
                        newTheme.effectSets.Add(es.Clone());
                    }
                    theme = newTheme;
                    AssetDatabase.CreateAsset(theme, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorPrefs.SetString(prefsKey, path);
                }
            }
        }

        #region Scene Color Methods
        private void SearchSceneColors()
        {
            sceneColors.Clear();
            GetSceneColors();
        }

        public void GetSceneColors()
        {
            foreach (var root in GamestrapHelper.GetSceneGameObjectRoots())
            {
                SearchColorsGameObject(root);
            }
        }

        private void SearchColorsGameObject(GameObject gameObject)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Text>())
            {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Text>().color);
            }
            if (gameObject.GetComponent<UnityEngine.UI.Image>())
            {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Image>().color);
            }
            if (gameObject.GetComponent<UnityEngine.UI.Button>())
            {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Button>());
            }
            if (gameObject.GetComponent<UnityEngine.UI.Toggle>())
            {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Toggle>());
            }
            if (gameObject.GetComponent<UnityEngine.UI.Slider>())
            {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Slider>());
            }

            if (gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    SearchColorsGameObject(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        private void AddSceneColor(Color color)
        {
            if (!sceneColors.Contains(color))
            {
                sceneColors.Add(color);
            }
        }

        private void AddSceneColor(UnityEngine.UI.Selectable selectable)
        {
            UnityEngine.UI.ColorBlock colorBlock = selectable.colors;
            AddSceneColor(colorBlock.normalColor);
            // Uncomment if you want to include additional colorBlock colors
            //AddSceneColor(colorBlock.highlightedColor);
            //AddSceneColor(colorBlock.pressedColor);
            //AddSceneColor(colorBlock.disabledColor);
        }
        #endregion

        #region Set Custom GUI Styles

        void SetStyles()
        {
            if (btnStyle == null)
            {
                SetBtnStyle();
            }
            if (btnSelectedStyle == null)
            {
                SetBtnSelectedStyle();
            }
            if (btnLinestyle == null)
            {
                SetBtnLineStyle();
            }
            if (bgStyle == null)
            {
                SetBgStyle();
            }
            if (titleStyle == null)
            {
                SetTitleStyle();
            }
            if (subtitleStyle == null)
            {
                SetSubtitleStyle();
            }
            if (btnStyleWhiteFont == null)
            {
                btnStyleWhiteFont = new GUIStyle(GUI.skin.button);
                btnStyleWhiteFont.normal.textColor = Color.white;
            }
        }

        void SetBtnStyle()
        {
            btnStyle = new GUIStyle(GUI.skin.button);
            Sprite bg = (Sprite)AssetDatabase.LoadAssetAtPath(GamestrapHelper.gamestrapRoute + "Editor/UI/Button Normal.psd", typeof(Sprite));
            Sprite bgClicked = (Sprite)AssetDatabase.LoadAssetAtPath(GamestrapHelper.gamestrapRoute + "Editor/UI/Button Pressed.psd", typeof(Sprite));
            btnStyle.margin = new RectOffset(0, 0, 0, 1);
            btnStyle.padding = new RectOffset(0, 0, 4, 4);
            btnStyle.normal.background = bg.texture;
            btnStyle.active.background = bgClicked.texture;
        }

        void SetBtnLineStyle()
        {
            btnLinestyle = new GUIStyle(GUI.skin.button);
            btnLinestyle.margin = new RectOffset(0, 0, 2, 1);
            btnLinestyle.padding = new RectOffset(0, 0, 2, 3);
            btnLinestyle.fontSize = 10;
        }

        void SetBtnSelectedStyle()
        {
            btnSelectedStyle = new GUIStyle(GUI.skin.button);
            Sprite bg = (Sprite)AssetDatabase.LoadAssetAtPath(GamestrapHelper.gamestrapRoute + "Editor/UI/Button Selected.psd", typeof(Sprite));
            btnSelectedStyle.margin = new RectOffset(0, 0, 0, 1);
            btnSelectedStyle.padding = new RectOffset(0, 0, 4, 4);
            btnSelectedStyle.normal.background = bg.texture;
        }

        void SetBgStyle()
        {
            bgStyle = new GUIStyle(GUI.skin.box);
            bgStyle.margin = new RectOffset(4, 4, 0, 4);
            bgStyle.padding = new RectOffset(1, 1, 1, 2);
        }


        void SetTitleStyle()
        {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 12;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.LowerCenter;
            titleStyle.margin = new RectOffset(4, 4, 6, 2);
        }

        void SetSubtitleStyle()
        {
            subtitleStyle = new GUIStyle(GUI.skin.label);
            subtitleStyle.fontSize = 12;
            subtitleStyle.fontStyle = FontStyle.Bold;
            subtitleStyle.alignment = TextAnchor.MiddleLeft;
            subtitleStyle.margin = new RectOffset(4, 4, 4, 4);
        }
        #endregion

        #region Assign Color Section
        /// <summary>
        /// Sets the color to the UI elements based on what types of components they have
        /// </summary>
        private void AssignColorsToSelection(ColorSet colorSet)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                recursiveLevel = 0;
                AssignColorsToSelection(go, colorSet);
                // This resets the element so it updates the colors in the editor
                go.SetActive(false);
                go.SetActive(true);
            }
        }

        public void AssignColorsToSelection(GameObject gameObject, ColorSet colorSet)
        {
            recursiveLevel++;
            if (gameObject.GetComponent<UnityEngine.UI.Button>())
            {
                UnityEngine.UI.Button button = gameObject.GetComponent<UnityEngine.UI.Button>();
                SetColorBlock(button, colorSet);
                SetDetailColor(gameObject, colorSet);
                EditorUtility.SetDirty(button);
            }
            else if (gameObject.GetComponent<UnityEngine.UI.InputField>())
            {
                UnityEngine.UI.InputField input = gameObject.GetComponent<UnityEngine.UI.InputField>();
                SetColorBlock(input, colorSet);
                input.selectionColor = colorSet.highlighted;
                Undo.RecordObject(input.textComponent, "Change Text color");
                input.textComponent.color = colorSet.pressed;
                Undo.RecordObject(input.placeholder, "Change Placeholder color");
                input.placeholder.color = colorSet.highlighted;
                EditorUtility.SetDirty(input);
                EditorUtility.SetDirty(input.textComponent);
                EditorUtility.SetDirty(input.placeholder);
            }
            else if (gameObject.GetComponent<UnityEngine.UI.Scrollbar>())
            {
                UnityEngine.UI.Scrollbar sb = gameObject.GetComponent<UnityEngine.UI.Scrollbar>();
                SetColorBlock(sb, colorSet);
                Undo.RecordObject(gameObject.GetComponent<UnityEngine.UI.Image>(), "Change Image color");
                gameObject.GetComponent<UnityEngine.UI.Image>().color = colorSet.disabled;
                EditorUtility.SetDirty(sb);
                EditorUtility.SetDirty(gameObject.GetComponent<UnityEngine.UI.Image>());
            }
            else if (gameObject.GetComponent<UnityEngine.UI.Slider>())
            {
                UnityEngine.UI.Slider slider = gameObject.GetComponent<UnityEngine.UI.Slider>();
                SetColorBlock(slider, colorSet);
                Undo.RecordObject(slider.fillRect.gameObject.GetComponent<UnityEngine.UI.Image>(), "Change Image color");
                slider.fillRect.gameObject.GetComponent<UnityEngine.UI.Image>().color = colorSet.normal;
                SetTextColorRecursive(gameObject, colorSet);
                EditorUtility.SetDirty(slider);
                EditorUtility.SetDirty(slider.fillRect.gameObject.GetComponent<UnityEngine.UI.Image>());
            }
            else if (gameObject.GetComponent<UnityEngine.UI.Toggle>())
            {
                UnityEngine.UI.Toggle toggle = gameObject.GetComponent<UnityEngine.UI.Toggle>();
                SetColorBlock(toggle, colorSet);
                Undo.RecordObject(toggle.graphic, "Change Image color");
                toggle.graphic.color = colorSet.normal;
                SetTextColorRecursive(gameObject, colorSet);
                EditorUtility.SetDirty(toggle);
                EditorUtility.SetDirty(toggle.graphic);
            }
            else if (gameObject.transform.childCount > 0) // Recursive search for components
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    AssignColorsToSelection(gameObject.transform.GetChild(i).gameObject, colorSet);
                }
            }
            else if (recursiveLevel == 1)
            {
                if (gameObject.GetComponent<UnityEngine.UI.Image>())
                {
                    UnityEngine.UI.Image image = gameObject.GetComponent<UnityEngine.UI.Image>();
                    Undo.RecordObject(image, "Change color");
                    image.color = colorSet.normal;
                    EditorUtility.SetDirty(image);
                }
                else if (gameObject.GetComponent<UnityEngine.UI.Text>())
                {
                    UnityEngine.UI.Text text = gameObject.GetComponent<UnityEngine.UI.Text>();
                    Undo.RecordObject(text, "Change color");
                    text.color = colorSet.normal;
                    EditorUtility.SetDirty(text);
                }
            }
        }

        /// <summary>
        /// Sets all of the current variables to a ColorBlock and returns it
        /// </summary>
        /// <param name="cb">The color block from the UI element</param>
        /// <returns>The color block with the new values</returns>
        public void SetColorBlock(UnityEngine.UI.Selectable selectable, ColorSet colorSet)
        {
            UnityEngine.UI.Image img = selectable.GetComponent<UnityEngine.UI.Image>();
            Undo.RecordObject(selectable, "Change ColorBlock");
            if (selectable.transition == UnityEngine.UI.Selectable.Transition.ColorTint)
            {
                if (img)
                {
                    img.color = Color.white;
                }
                UnityEngine.UI.ColorBlock cb = selectable.colors;
                cb.normalColor = colorSet.normal;
                cb.highlightedColor = colorSet.highlighted;
                cb.pressedColor = colorSet.pressed;
                cb.disabledColor = colorSet.disabled;
                selectable.colors = cb;
            }
            else if (selectable.GetComponent<UnityEngine.UI.Image>())
            {
                img.color = colorSet.normal;
            }
        }

        public void SetColors(Color[] color)
        {
            if (color.Length < 5)
            {
                Debug.LogError("Array too short, the color Array needs to be of length count 5");
                return;
            }
            if (selectedColorSet == null)
            {
                return;
            }
            selectedColorSet.normal = color[0];
            selectedColorSet.highlighted = color[1];
            selectedColorSet.pressed = color[2];
            selectedColorSet.disabled = color[3];
            selectedColorSet.detail = color[4];
        }

        /// <summary>
        /// Searches if the GameObject has children and if the children have components type Image or Text.
        /// If they do then it will assign the variable detail to the Color of the image or text.
        /// </summary>
        /// <param name="go">UI GameObject with children</param>
        private void SetDetailColor(GameObject go, ColorSet colorSet)
        {
            int children = go.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                if (child.GetComponent<UnityEngine.UI.Image>())
                {
                    Undo.RecordObject(child.GetComponent<UnityEngine.UI.Image>(), "Change Image color");
                    child.GetComponent<UnityEngine.UI.Image>().color = colorSet.detail;
                    EditorUtility.SetDirty(child.GetComponent<UnityEngine.UI.Image>());
                }
                if (child.GetComponent<UnityEngine.UI.Text>())
                {
                    UnityEngine.UI.Text t = child.GetComponent<UnityEngine.UI.Text>();
                    Undo.RecordObject(t, "Change Text color");
                    t.color = colorSet.detail;
                    EditorUtility.SetDirty(t);
                }
            }
        }

        /// <summary>
        /// Looks recursively for component Text in the GameObjects children 
        /// and also changes the color to the Detail variable if it finds any.
        /// </summary>
        /// <param name="go">UI GameObject with children</param>
        private void SetTextColorRecursive(GameObject go, ColorSet colorSet)
        {
            int children = go.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                if (child.GetComponent<UnityEngine.UI.Text>())
                {
                    UnityEngine.UI.Text t = child.GetComponent<UnityEngine.UI.Text>();
                    Undo.RecordObject(t, "Change Text color");
                    t.color = colorSet.normal;
                    EditorUtility.SetDirty(t);
                }
                SetTextColorRecursive(child, colorSet);
            }
        }
        #endregion

        #region Set Font
        private void AssignFontToSelection()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                AssignFontToSelection(go);
                // This resets the element so it updates the colors in the editor
                go.SetActive(false);
                go.SetActive(true);
            }
        }

        private void AssignFontToSelection(GameObject gameObject)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Text>())
            {
                UnityEngine.UI.Text text = gameObject.GetComponent<UnityEngine.UI.Text>();
                if (font)
                {
                    Undo.RecordObject(text, "Change Font");
                    text.font = font;
                    EditorUtility.SetDirty(text);
                }
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                AssignFontToSelection(gameObject.transform.GetChild(i).gameObject);
            }
        }

        #endregion

        #region Activate/Deactivate Effects
        /// <summary>
        /// Sets the color to the UI elements based on what types of components they have
        /// </summary>
        private void ActivateEffects(EffectSet set)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                ActivateEffects(go, set);
                // This resets the element so it updates the colors in the editor
                go.SetActive(false);
                go.SetActive(true);
            }
        }

        public void ActivateEffects(GameObject gameObject, EffectSet set)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Image>())
            {
                if (set.gradient)
                {
                    GradientEffect gradientEffect = gameObject.GetComponent<GradientEffect>();
                    if (!gradientEffect)
                    {
                        gradientEffect = Undo.AddComponent<GradientEffect>(gameObject);
                    }
                    gradientEffect.top = set.gradientTop;
                    gradientEffect.bottom = set.gradientBottom;
                    EditorUtility.SetDirty(gradientEffect);
                }
                else if (gameObject.GetComponent<GradientEffect>())
                {
                    Undo.DestroyObjectImmediate(gameObject.GetComponent<GradientEffect>());
                }
            }

            if (gameObject.GetComponent<UnityEngine.UI.Image>() || gameObject.GetComponent<UnityEngine.UI.Text>())
            {
                if (set.shadow)
                {
                    ShadowEffect shadowEffect = gameObject.GetComponent<ShadowEffect>();
                    if (!shadowEffect)
                    {
                        shadowEffect = Undo.AddComponent<ShadowEffect>(gameObject);
                    }
                    shadowEffect.effectDistance = set.shadowOffset;
                    shadowEffect.effectColor = set.shadowColor;
                    EditorUtility.SetDirty(shadowEffect);
                }
                else if (gameObject.GetComponent<ShadowEffect>())
                {
                    Undo.DestroyObjectImmediate(gameObject.GetComponent<ShadowEffect>());
                }
            }

            if (gameObject.transform.childCount > 0) // Recursive search for components
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    ActivateEffects(gameObject.transform.GetChild(i).gameObject, set);
                }
            }
        }
        #endregion

        #region Color suggestion methods
        /// <summary>
        /// Adds all of the color arrays the editor window will suggest.
        /// </summary>
        public void SetUpColors()
        {
            colors = new List<Color[]>();
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(74, 37, 68)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(206, 20, 90)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(141, 39, 137)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(37, 82, 102)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(41, 165, 220)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(126, 209, 232)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(54, 148, 104)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(134, 192, 63)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(211, 218, 33)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(255, 204, 0)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(255, 153, 0)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(255, 173, 67)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(242, 110, 37)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(255, 102, 0)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(239, 106, 65)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(230, 36, 45)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(137, 24, 16)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(239, 101, 101)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(134, 98, 57)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(91, 54, 21)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(192, 150, 109)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(200, 200, 200)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(128, 128, 128)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(51, 51, 51)));
            colors.Add(GetColorDefault(GamestrapHelper.ColorRGBInt(21, 21, 21)));
        }

        /// <summary>
        /// Helper methods to create a color array
        /// </summary>
        /// <param name="baseColor">Base color of what the UI will look like</param>
        /// <returns></returns>
        public static Color[] GetColorDefault(Color baseColor)
        {
            Color highlighted = Color.Lerp(baseColor, Color.white, 0.3f);
            Color pressed = Color.Lerp(baseColor, Color.black, 0.6f);
            Color disabled = GamestrapHelper.ColorRGBInt(224, 224, 224);
            Color detail = Color.white;
            return new Color[] { baseColor, highlighted, pressed, disabled, detail };
        }
        #endregion
    }
}