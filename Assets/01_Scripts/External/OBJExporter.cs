﻿using UnityEngine;
using System.IO;
using System.Text;
using System;

//Original Obtained from SteenPetersen & ReDarkTechnology on the Unity Forum. Adjusted for personal requirement, Original Can be found below.
public class ExportMeshToOBJ : ScriptableObject
{
    public static async void ExportToOBJ(GameObject obj, Action ifExistsCallback = null)
    {
        if (obj == null)
        {
            Debug.Log("No object selected.");
            return;
        }

        if (!obj.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            Debug.Log("No MeshFilter is found in selected GameObject.", obj);
            return;
        }

        if (meshFilter.sharedMesh == null)
        {
            Debug.Log("No mesh is found in selected GameObject.", obj);
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, $"{obj.name}.obj");

        if (ifExistsCallback != null && File.Exists(path))
        {
            ifExistsCallback?.Invoke();
            return;
        }

        StreamWriter writer = new(path);
        await writer.WriteAsync(GetMeshOBJ(obj.name, meshFilter.sharedMesh));
        writer.Close();
    }

    public static string GetMeshOBJ(string name, Mesh mesh)
    {
        StringBuilder sb = new();

        foreach (var v in mesh.vertices)
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));

        foreach (var v in mesh.normals)
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));

        for (int material = 0; material < mesh.subMeshCount; material++)
        {
            sb.Append(string.Format("\ng {0}\n", name));
            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n",
                triangles[i] + 1,
                triangles[i + 1] + 1,
                triangles[i + 2] + 1));
            }
        }

        return sb.ToString();
    }
}

//Original ->

//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Text;

//public class ExportMeshToOBJ : ScriptableObject
//{
//    [MenuItem("GameObject/Export to OBJ")]
//    static void ExportToOBJ()
//    {
//        GameObject obj = Selection.activeObject as GameObject;
//        if (obj == null)
//        {
//            Debug.Log("No object selected.");
//            return;
//        }

//        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
//        if (meshFilter == null)
//        {
//            Debug.Log("No MeshFilter is found in selected GameObject.", obj);
//            return;
//        }

//        if (meshFilter.sharedMesh == null)
//        {
//            Debug.Log("No mesh is found in selected GameObject.", obj);
//            return;
//        }

//        string path = EditorUtility.SaveFilePanel("Export OBJ", "", obj.name, "obj");
//        StreamWriter writer = new StreamWriter(path);
//        writer.Write(GetMeshOBJ(obj.name, meshFilter.sharedMesh));
//        writer.Close();
//    }

//    [MenuItem("GameObject/Export to OBJs")]
//    static void ExportToOBJs()
//    {
//        var objs = Selection.gameObjects;
//        if (objs.Length < 1)
//        {
//            Debug.Log("No object selected.");
//            return;
//        }
//        var directory = EditorUtility.SaveFolderPanel("Export OBJs to", "", "OBJFiles");
//        foreach (var obj in objs)
//        {
//            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
//            if (meshFilter == null)
//            {
//                Debug.LogWarning($"No MeshFilter is found in selected GameObject: {obj.name}", obj);
//                continue;
//            }

//            if (meshFilter.sharedMesh == null)
//            {
//                Debug.LogWarning($"No mesh is found in selected GameObject: {obj.name}", obj);
//                continue;
//            }

//            string path = Path.Combine(directory, obj.name + ".obj");
//            StreamWriter writer = new StreamWriter(path);
//            writer.Write(GetMeshOBJ(obj.name, meshFilter.sharedMesh));
//            writer.Close();
//        }
//    }

//    public static string GetMeshOBJ(string name, Mesh mesh)
//    {
//        StringBuilder sb = new StringBuilder();

//        foreach (Vector3 v in mesh.vertices)
//            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));

//        foreach (Vector3 v in mesh.normals)
//            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));

//        for (int material = 0; material < mesh.subMeshCount; material++)
//        {
//            sb.Append(string.Format("\ng {0}\n", name));
//            int[] triangles = mesh.GetTriangles(material);
//            for (int i = 0; i < triangles.Length; i += 3)
//            {
//                sb.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n",
//                triangles[i] + 1,
//                triangles[i + 1] + 1,
//                triangles[i + 2] + 1));
//            }
//        }

//        return sb.ToString();
//    }
//}