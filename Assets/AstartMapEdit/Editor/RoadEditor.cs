using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapEditorManager))] //ָ���Զ���Editor��Ҫ�󶨵�Mono���ͣ��������typeof
public class RoadEditor : Editor //�̳�Editor
{
    MapEditorManager m_EditorManager;
    private bool isPlacing = false;//�Ƿ����༭ģʽ
    private bool enterPlacingBatMode = false;//�Ƿ������������ģʽ
    private string mapPath = "Assets/AssetsPackage/Maps/MapData/mapTex.asset";

    private void OnSceneGUI()
    {
        if (this.isPlacing == false) return;

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Space)//����ǿո�Ļ�,���������ı�
            {
                this.enterPlacingBatMode = !this.enterPlacingBatMode;

            }
            else if(Event.current.keyCode == KeyCode.C)//�����C�Ļ�,���뵥���ı�ģʽ
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

    //����block����
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
        this.SetMapValue(ref hitInfo, 1);//1����Ϊ�������ߵ�·
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

    private void OnDisable()//�����������scene���ں�ر�editorģʽ
    {
        this.enterPlacingBatMode = false;
        this.isPlacing = false;
    }

    /// <summary>
    /// ��дOnInspectorGUI��֮�����е�GUI���ƶ��ڴ˷����С�
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //���ø��෽������һ��GUI��TutorialMono��ԭ���Ŀ����л����ݵȻ����������һ�Ρ�
        //��������ø��෽���������Mono��InspectorȫȨ�����������ơ�

        this.m_EditorManager = (MapEditorManager)this.target;
        GUILayout.Label("�����������ļ�������·��");
        this.mapPath = GUILayout.TextField(this.mapPath); 

        SceneView view = GetSceneView();

        //�ж��Ƿ�Ϊ�༭ģʽ�������������ʾstart editing ��ť
        if (!isPlacing && GUILayout.Button("Start Editing", GUILayout.Height(40))) 
        {
            this.isPlacing = true;
            this.enterPlacingBatMode = false;//��ʼ��
            view.Focus();
        }
        GUI.backgroundColor = Color.yellow;

        //�ж��Ƿ�Ϊ�༭ģʽ�����������ʾend editing ��ť
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