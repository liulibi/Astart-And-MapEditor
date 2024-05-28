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

    //控制器面板
    public void OnGUI()
    {
        GUILayout.Label("地图X方向块数");
        this.mapX = Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("地图Z方向块数");
        this.mapZ = Convert.ToInt32(GUILayout.TextField(this.mapZ.ToString()));

        GUILayout.Label("地图方块的高度");
        this.blockHeight = Convert.ToInt32(GUILayout.TextField(this.blockHeight.ToString()));

        GUILayout.Label("地图方块的大小");
        this.blockSize = Convert.ToInt32(GUILayout.TextField(this.blockSize.ToString()));

        GUILayout.Label("请选择地图原点");
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("没有选中地图原点，无法生成");
        }

        if (GUILayout.Button("在原点下生成地图块"))
        {
            if(Selection.activeGameObject != null)
            {
                Debug.Log("开始生成...");
                this.CreateBlocksAt(Selection.activeGameObject);
                Debug.Log("完成生成。");
            }
        }

        if (GUILayout.Button("重置地图块"))
        {
            if(Selection.activeGameObject != null)
            {
                this.ResetBlocks(Selection.activeGameObject);
            }
        }

        if(GUILayout.Button("清理图块"))
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
        //考虑物体本身中心点的偏移
        Vector3 startPos = new Vector3(this.blockSize / 2, 0, this.blockSize / 2); 
        GameObject cubePerfab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AstartMapEdit/Res/Cube.prefab");
        if (cubePerfab == null)
        {
            Debug.LogError("错误路径");
        }

        //创建地图块
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


