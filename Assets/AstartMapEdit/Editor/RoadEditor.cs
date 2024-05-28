using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapEditorManager))] //指定自定义Editor所要绑定的Mono类型，这里就是typeof
public class RoadEditor : Editor //继承Editor
{
    MapEditorManager m_EditorManager;
    private bool isPlacing = false;//是否进入编辑模式
    private bool enterPlacingBatMode = false;//是否进入连续铺设模式
    private string mapPath = "Assets/AssetsPackage/Maps/MapData/mapTex.asset";

    private void OnSceneGUI()
    {
        if (this.isPlacing == false) return;

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Space)//如果是空格的话,进入连续改变
            {
                this.enterPlacingBatMode = !this.enterPlacingBatMode;

            }
            else if(Event.current.keyCode == KeyCode.C)//如果是C的话,进入单个改变模式
            {
                this.enterPlacingBatMode = false;
                this.SetMapBlock();
                return; 
            }
        }

        if(this.enterPlacingBatMode == false)
        {
            return;
        }
        else
        {
            this.SetMapBlock();
        }
    }

    //单个block设置
    void SetMapBlock()
    {
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if (!Physics.Raycast(worldRay, out hitInfo))
        {
            return;
        }
        if (hitInfo.collider.gameObject.name != "block")
        {
            return;
        }
        this.SetMapValue(ref hitInfo, 1);//1代表为可以行走的路
    }

    

    void SetMapValue(ref RaycastHit raycastHitInfo, int value)
    {
        BlockDate blockDate = raycastHitInfo.collider.gameObject.GetComponent<BlockDate>();
        if (blockDate != null)
        {
            blockDate.isGO = value;
        }
        if (blockDate.isGO == 1)
        {
            raycastHitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            raycastHitInfo.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();
        return view;
    }

    private void OnDisable()//当鼠标点击其他scene窗口后关闭editor模式
    {
        this.enterPlacingBatMode = false;
        this.isPlacing = false;
    }

    /// <summary>
    /// 重写OnInspectorGUI，之后所有的GUI绘制都在此方法中。
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //调用父类方法绘制一次GUI，TutorialMono中原本的可序列化数据等会在这里绘制一次。
        //如果不调用父类方法，则这个Mono的Inspector全权由下面代码绘制。

        this.m_EditorManager = (MapEditorManager)this.target;
        GUILayout.Label("请设置配置文件的生成路径");
        this.mapPath = GUILayout.TextField(this.mapPath); 

        SceneView view = GetSceneView();

        //判断是否为编辑模式，如果不是则显示start editing 按钮
        if (!isPlacing && GUILayout.Button("Start Editing", GUILayout.Height(40))) 
        {
            this.isPlacing = true;
            this.enterPlacingBatMode = false;//初始化
            view.Focus();
        }
        GUI.backgroundColor = Color.yellow;

        //判断是否为编辑模式，如果是则显示end editing 按钮
        if (this.isPlacing && GUILayout.Button("End Editing", GUILayout.Height(40)))
        {
            this.isPlacing = false;
            this.enterPlacingBatMode = false;
            this.ExportMapBitMap();
        }
    }

    private void ExportMapBitMap()
    {
        Texture2D mapTex = new Texture2D(this.m_EditorManager.mapX, this.m_EditorManager.mapZ, TextureFormat.Alpha8, false);
        byte[] rawData=mapTex.GetRawTextureData();
        for(int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0;
        }
        for(int i = 0;i<this.m_EditorManager.gameObject.transform.childCount;i++)
        {
            if (m_EditorManager.gameObject.transform.GetChild(i).name == "block") 
            {
                BlockDate blockData = m_EditorManager.gameObject.transform.transform.GetChild(i).GetComponent<BlockDate>();
                rawData[i] = (byte)((blockData.isGO==0) ? 0 : 255);
            }
        }
        mapTex.LoadRawTextureData(rawData);

        AssetDatabase.DeleteAsset(this.mapPath);
        AssetDatabase.CreateAsset(mapTex, this.mapPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}