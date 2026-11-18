using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZombiRunner;

// Creates real, editable scene objects; no runtime UI generation is required.
public static class MainMenuSceneBuilder
{
    const string ScenePath = "Assets/Scenes/MainMenu.unity";
    static Font heading, body;
    static Color cream = new Color32(240, 230, 207, 255);
    static Color muted = new Color32(149, 168, 159, 255);
    static Color red = new Color32(177, 35, 27, 255);
    static Color ink = new Color32(9, 17, 17, 255);

    [InitializeOnLoadMethod]
    static void Schedule() { EditorApplication.delayCall += CreateIfRequested; }
    static void CreateIfRequested()
    {
        if (File.Exists("Temp/PolishMainMenu.request"))
        {
            File.Delete("Temp/PolishMainMenu.request");
            var active = SceneManager.GetActiveScene();
            if (active.path == ScenePath)
            {
                foreach (var go in active.GetRootGameObjects())
                {
                    foreach (var text in go.GetComponentsInChildren<Text>(true)) text.verticalOverflow = VerticalWrapMode.Overflow;
                    if (go.name == "Infected showcase") { go.transform.localScale = Vector3.one * 1.75f; go.transform.position = new Vector3(1.3f, -.65f, 0); }
                }
                var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Menu/Silhouette.mat");
                mat.shader = Shader.Find("Unlit/Color");
                mat.color = new Color32(15, 35, 32, 255);
                EditorSceneManager.SaveScene(active);
                AssetDatabase.SaveAssets();
                Debug.Log("MAIN_MENU_POLISHED");
            }
        }
        if (!File.Exists("Temp/CreateMainMenu.request") || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete("Temp/CreateMainMenu.request");
        Build();
    }

    [MenuItem("Tools/Zombie Runner/Create Main Menu")]
    public static void Build()
    {
        if (File.Exists(ScenePath)) { Debug.Log("Main menu already exists; edit the scene directly."); return; }
        var previous = SceneManager.GetActiveScene();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        heading = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Examples & Extras/Fonts/Anton.ttf");
        body = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
        if (!heading) heading = body;
        var camera = new GameObject("Menu Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 1.5f, -6);
        camera.transform.LookAt(new Vector3(0, 1.1f, 0));
        camera.fieldOfView = 37;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(12, 31, 28, 255);
        RenderSettings.ambientLight = new Color(.25f, .34f, .3f);
        Light("Warm key", new Vector3(35, -35, 0), new Color(1f, .68f, .36f), 1.7f);
        Light("Cold rim", new Vector3(10, 145, 0), new Color(.22f, .8f, .7f), 2.3f);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fantasy Horde - Zombie/prefab/zombie_regular.prefab");
        if (prefab)
        {
            var model = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            model.name = "Infected showcase";
            model.transform.localScale = Vector3.one * 1.75f;
            model.transform.position = new Vector3(1.3f, -.65f, 0);
            model.transform.rotation = Quaternion.Euler(0, 195, 0);
            foreach (var behaviour in model.GetComponentsInChildren<MonoBehaviour>()) Object.DestroyImmediate(behaviour);
            foreach (var animator in model.GetComponentsInChildren<Animator>()) animator.enabled = false;
        }
        // Layered silhouettes form an inexpensive city backdrop.
        var material = new Material(Shader.Find("Unlit/Color"));
        material.color = new Color32(15, 35, 32, 255);
        Directory.CreateDirectory("Assets/Materials/Menu");
        AssetDatabase.CreateAsset(material, "Assets/Materials/Menu/Silhouette.mat");
        for (int i = 0; i < 15; i++)
        {
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tower.name = "City silhouette " + i;
            float h = 2 + (i * 7 % 5);
            tower.transform.position = new Vector3(-10 + i * 1.5f, h / 2 - 1, 5 + i % 3);
            tower.transform.localScale = new Vector3(1.1f, h, 1);
            tower.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(tower.GetComponent<Collider>());
        }
        var canvas = new GameObject("Main Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600, 900);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var root = Rect("Layout", canvas.transform, 0, 0, 1600, 900);
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Vector2.zero;
        var controller = canvas.gameObject.AddComponent<MainMenu>();
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Panel(root, "Left shade", 0, 100, 690, 685, new Color(0.015f, .04f, .035f, .87f));
        Panel(root, "Header", 0, 0, 1600, 112, ink);
        Panel(root, "Header rule", 0, 110, 1600, 2, new Color32(67, 81, 65, 255));
        Label(root, "ZOMBIE", 46, 17, 260, 50, 44, red, true);
        Label(root, "R U N N E R", 48, 66, 260, 29, 22, cream, true);
        Label(root, "SURVIVAL IS THE ONLY OBJECTIVE", 570, 43, 520, 30, 18, muted);
        Label(root, "OFFLINE  /  ENDLESS", 1240, 43, 310, 30, 19, cream);
        Label(root, "PREPARE YOUR RUN", 55, 144, 550, 35, 24, cream, true);
        Label(root, "THE CITY HAS FALLEN. KEEP MOVING.", 55, 183, 560, 26, 15, muted);
        Card(root, "01", "YOUR SURVIVOR", "THE LAST RUNNER", "Ready for the streets", 233, controller.Survivor);
        Card(root, "02", "SELECTED DISTRICT", "DEAD CITY", "An endless route through the outbreak", 399, controller.District);
        Card(root, "03", "WEAPONS & SUPPLIES", "FIELD ARMORY", "Find it. Equip it. Make every shot count.", 565, controller.Armory);
        Label(root, "QUARANTINE ZONE  /  001", 920, 157, 620, 30, 19, muted);
        Label(root, "NO WAY BACK.", 910, 622, 650, 65, 53, cream, true);
        Label(root, "ONLY THE NEXT BLOCK.", 914, 691, 600, 35, 22, cream);
        Panel(root, "Footer", 0, 784, 1600, 116, ink);
        Panel(root, "Footer rule", 0, 784, 1600, 2, new Color32(67, 81, 65, 255));
        Label(root, "RUN. FIGHT. SURVIVE.", 55, 823, 375, 32, 21, muted, true);
        Button(root, "HOW TO PLAY", 510, 807, 230, 66, controller.Tutorial, false);
        Button(root, "SETTINGS", 758, 807, 180, 66, controller.Settings, false);
        var sound = Button(root, "SOUND  ON", 956, 807, 190, 66, controller.ToggleSound, false);
        controller.soundLabel = sound.GetComponentInChildren<Text>();
        controller.playButton = Button(root, "PLAY    >", 1180, 801, 370, 78, controller.Play, true);
        var dialog = Panel(root, "Dialog overlay", 0, 0, 1600, 900, new Color(0, 0, 0, .88f));
        var box = Panel(dialog, "Dialog", 350, 180, 900, 540, ink);
        Panel(box, "Accent", 0, 0, 900, 5, red);
        controller.dialogTitle = Label(box, "", 50, 35, 800, 65, 42, cream, true);
        controller.dialogBody = Label(box, "", 50, 125, 800, 290, 23, cream);
        Button(box, "CLOSE", 610, 440, 240, 65, controller.CloseDialog, true);
        controller.dialog = dialog.gameObject;
        dialog.gameObject.SetActive(false);
        EditorSceneManager.SaveScene(scene, ScenePath);
        var scenes = EditorBuildSettings.scenes.Where(s => s.path != ScenePath && s.path != "Assets/Scenes/gameplay.unity" && !s.path.EndsWith("/SampleScene.unity")).ToList();
        scenes.Insert(0, new EditorBuildSettingsScene("Assets/Scenes/gameplay.unity", true));
        scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        AssetDatabase.SaveAssets();
        if (previous.IsValid()) SceneManager.SetActiveScene(previous);
        EditorSceneManager.CloseScene(scene, true);
        Debug.Log("MAIN_MENU_CREATED: " + ScenePath);
    }
    static void Light(string name, Vector3 angles, Color color, float intensity)
    {
        var light = new GameObject(name, typeof(Light)).GetComponent<Light>();
        light.type = LightType.Directional; light.color = color; light.intensity = intensity;
        light.transform.eulerAngles = angles;
    }
    static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(parent, false); r.anchorMin = r.anchorMax = new Vector2(0, 1);
        r.pivot = new Vector2(0, 1); r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
    }
    static RectTransform Panel(Transform parent, string name, float x, float y, float w, float h, Color c)
    {
        var r = Rect(name, parent, x, y, w, h); r.gameObject.AddComponent<Image>().color = c; return r;
    }
    static Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color c, bool bold = false)
    {
        var r = Rect(text.Length > 0 ? text : "Text", parent, x, y, w, h);
        var t = r.gameObject.AddComponent<Text>(); t.font = bold ? heading : body;
        t.text = text; t.fontSize = size; t.color = c; t.raycastTarget = false;
        t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow; return t;
    }
    static Button Button(Transform parent, string title, float x, float y, float w, float h, UnityAction action, bool primary)
    {
        var r = Panel(parent, title, x, y, w, h, primary ? red : new Color32(28, 42, 39, 255));
        var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = r.GetComponent<Image>();
        var colors = b.colors; colors.highlightedColor = new Color(1.3f, 1.3f, 1.3f); colors.pressedColor = new Color(.7f, .7f, .7f); b.colors = colors;
        var t = Label(r, title, 8, 0, w - 16, h, primary ? 30 : 20, cream, true); t.alignment = TextAnchor.MiddleCenter;
        UnityEventTools.AddPersistentListener(b.onClick, action); return b;
    }
    static void Card(Transform parent, string number, string eyebrow, string title, string subtitle, float y, UnityAction action)
    {
        var b = Button(parent, title, 55, y, 590, 145, action, true);
        Object.DestroyImmediate(b.GetComponentInChildren<Text>().gameObject);
        Panel(b.transform, "Border", 0, 0, 4, 145, new Color32(231, 92, 60, 255));
        Label(b.transform, number, 23, 21, 90, 95, 65, new Color(1, .65f, .4f, .35f), true);
        Label(b.transform, eyebrow, 133, 18, 420, 25, 14, new Color32(237, 183, 154, 255));
        Label(b.transform, title, 130, 46, 440, 51, 36, cream, true);
        Label(b.transform, subtitle, 133, 108, 430, 25, 16, cream);
    }
}
