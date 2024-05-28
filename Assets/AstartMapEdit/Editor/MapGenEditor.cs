using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MapGenEditor :EditorWindow
{
    private int mapX = 64;
    private int mapZ = 64;
    private int blockSize = 8;
    private int blockHeight = 8;
    
    [MenuItem("Tool/MapEditorGen")]
    static void run()
    {
        EditorWindow.GetWindow(typeof(MapGenEditor));
    }

    //���������
    public void OnGUI()
    {
        GUILayout.Label("��ͼX�������");
        this.mapX = Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("��ͼZ�������");
        this.mapZ = Convert.ToInt32(GUILayout.TextField(this.mapZ.ToString()));

        GUILayout.Label("��ͼ����ĸ߶�");
        this.blockHeight = Convert.ToInt32(GUILayout.TextField(this.blockHeight.ToString()));

        GUILayout.Label("��ͼ����Ĵ�С");
        this.blockSize = Convert.ToInt32(GUILayout.TextField(this.blockSize.ToString()));

        GUILayout.Label("��ѡ���ͼԭ��");
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("û��ѡ�е�ͼԭ�㣬�޷�����");
        }

        if (GUILayout.Button("��ԭ�������ɵ�ͼ��"))
        {
            if(Selection.activeGameObject != null)
            {
                Debug.Log("��ʼ����...");
                this.CreateBlocksAt(Selection.activeGameObject);
                Debug.Log("������ɡ�");
            }
        }

        if (GUILayout.Button("���õ�ͼ��"))
        {
            if(Selection.activeGameObject != null)
            {
                this.ResetBlocks(Selection.activeGameObject);
            }
        }

        if(GUILayout.Button("����ͼ��"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ClearBlocksAt(Selection.activeGameObject);
            }
        }
    }

    private void ResetBlocks(GameObject org)
    {
        int count = org.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject child = org.transform.GetChild(i).gameObject;
            if (child != null)
            {
                if (child.name == "block")
                {
                    BlockDate blockDate = child.GetComponent<BlockDate>();
                    blockDate.isGO = 0;
                    child.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }

    private void ClearBlocksAt(GameObject org)
    {
        int count= org.transform.childCount;
        for(int i=0; i<count; i++)
        {
            GameObject child = org.transform.GetChild(0).gameObject;
            if(child != null)
            {
                if (child.name == "block") 
                GameObject.DestroyImmediate(child);
            }
        }
    }

    private void CreateBlocksAt(GameObject org)
    {
        MapEditorManager mapEditorManager =org.GetComponent<MapEditorManager>();
        if (mapEditorManager == null)
        {
            mapEditorManager = org.AddComponent<MapEditorManager>();
        }
        mapEditorManager.mapX = this.mapX;
        mapEditorManager.mapZ = this.mapZ;
        mapEditorManager.blockSize = this.blockSize; 

        this.ClearBlocksAt(org);
        //�������屾�����ĵ��ƫ��
        Vector3 startPos = new Vector3(this.blockSize / 2, 0, this.blockSize / 2); 
        GameObject cubePerfab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstartMapEdit/Res/Cube.prefab");
        if (cubePerfab == null)
        {
            Debug.LogError("����·��");
        }

        //������ͼ��
        for(int i = 0; i < mapX; i++)
        {
            Vector3 pos = startPos;
            for (int j = 0; j < mapZ; j++) 
            {
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePerfab) as GameObject;
                cube.name = "block";
                cube.transform.SetParent(org.transform, false);
                cube.transform.localPosition = pos;
                cube.transform.localScale=new Vector3(this.blockSize,this.blockHeight,this.blockSize);
                BlockDate blockDate = cube.AddComponent<BlockDate>();
                blockDate.mapX = j;
                blockDate.mapZ = i;
                blockDate.isGO = 0;

                pos.x += this.blockSize;
            }
            startPos.z += this.blockSize;
        }

    }

    void OnSelectionChange()
    {
        this.Repaint();
    }
}


