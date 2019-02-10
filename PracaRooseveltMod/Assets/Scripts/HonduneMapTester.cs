//**Created By Hondune**\\
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HonduneMapTester : MonoBehaviour
{
    bool showUI = true;
    bool disableUI;
    Vector2 scrollPosition = Vector2.zero;
    string[] assetFiles;
    GameObject prefab;
    Material splineMat;
    void Start()
    {
        splineMat = new Material(Shader.Find("HDRenderPipeline/Unlit"));
        splineMat.SetFloat("_BlendMode", 1);
        splineMat.SetFloat("_SurfaceType", 1);
        splineMat.SetColor("_UnlitColor", new Color(1, 0, 0, 1));
        splineMat.enableInstancing = true;
        DontDestroyOnLoad(gameObject);
        assetFiles = Directory.GetFiles(Application.dataPath + "\\AssetBundles");
    }

    void OnGUI()
    {

        if (Input.GetKeyDown("m"))
        {
            showUI = true;
        }
        if (!showUI)
            return;

        GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 10, 10), "");

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 35;
        GUI.Label(new Rect(Screen.width / 3, 0, Screen.width / 3, 100), "Hondune's Skater XL Map Importer", guiStyle);
        GUI.Label(new Rect(Screen.width / 3, 50, Screen.width / 3, 100), "Brought to you by hondune.com");
        if (assetFiles == null)
        {
            return;
        }
        scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 3, 100, Screen.width / 3, Screen.height - 200), scrollPosition, new Rect(0, 0, Screen.width / 3 - 20, 25 * assetFiles.Length));
        int next = 0;
        for (int i = 0; i < assetFiles.Length; i++)
        {
            if (!Path.GetFileName(assetFiles[i]).Contains(".") && Path.GetFileName(assetFiles[i]) != "AssetBundles")
            {
                if (GUI.Button(new Rect(10, 25 * next, Screen.width / 3 - 30, 20), "Load " + Path.GetFileName(assetFiles[i])))
                {
                    LoadAssetBundle(i);
                    showUI = false;
                }
                next++;
            }
        }
        GUI.EndScrollView();
        if (next == 0)
        {
            GUI.Label(new Rect(Screen.width / 3, 100, Screen.width / 3, 100), "No bundles found in AssetBundles folder!");
        }
    }

    void LoadAssetBundle(int selection)
    {
        print("Loading " + assetFiles[selection]);
        AssetBundle bundle = AssetBundle.LoadFromFile(assetFiles[selection]);
        if (bundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        Application.LoadLevel(Path.GetFileNameWithoutExtension(bundle.GetAllScenePaths()[0]));
        Invoke("SetUpGrinds", 1);
    }
    void SetUpGrinds()
    {
        prefab = GameObject.Find("Grinds");
        if (this.prefab != null)
        {
            Transform grindParent = new GameObject("Grind Triggers And Splines").transform;
            foreach (Transform child in prefab.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("GrindSpline"))
                {
                    Transform thisGrind = new GameObject(child.name + "SplineDisplay").transform;
                    thisGrind.parent = grindParent;
                    Vector3[] points = new Vector3[child.childCount];
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = child.GetChild(i).position;
                    }
                    for (int i = 0; i < points.Length - 1; i++)
                    {
                        GameObject colliderOb = new GameObject("RailCol" + i);
                        colliderOb.layer = 12;
                        colliderOb.transform.position = points[i];
                        colliderOb.transform.LookAt(points[i + 1]);
                        float length = Vector3.Distance(points[i], points[i + 1]);
                        GameObject displaySpline = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        displaySpline.transform.parent = colliderOb.transform;
                        displaySpline.transform.localRotation = Quaternion.identity;
                        displaySpline.transform.localScale = new Vector3(0.08f, 0.08f, length);
                        displaySpline.transform.localPosition = Vector3.forward * length / 2;
                        displaySpline.GetComponent<Renderer>().material = splineMat;
                        colliderOb.transform.parent = thisGrind;
                        Debug.DrawLine(points[i], points[i + 1], Color.red);
                    }
                }
            }
        }
        else if (prefab == null)
        {
            Invoke("SetUpGrinds", 0.5f);
        }
    }
}
