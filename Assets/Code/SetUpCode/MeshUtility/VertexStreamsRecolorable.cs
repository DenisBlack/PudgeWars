using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoomBoy.Runtime.MeshUtility
{
    [ExecuteInEditMode]
    public class VertexStreamsRecolorable : MonoBehaviour
    {
        public const string baseColorName = "_BaseColor";
        public const string specularColorName = "_SpecColor";

        [Serializable]
        public struct Colors
        {
            public string name;
            public Color baseColor;
            public Color specularColor;
        }

        [SerializeField] private MeshFilter filter = default;
        [SerializeField] private new MeshRenderer renderer = default;
        [SerializeField] private Material vertexColorMaterial = default;
        [SerializeField, HideInInspector] private Colors[] colors = default;

        private List<int[]> subMeshVerticies = default;
        private Mesh vertexStreamsMesh;

        public ref Colors this[int subMesh] => ref colors[subMesh];
        public int SubMeshCount => filter?.sharedMesh.subMeshCount ?? 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (filter == null) return;
            if (colors == null)
            {
                colors = new Colors[filter.sharedMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i].baseColor = Color.white;
                    colors[i].baseColor = (Color.white * 0.5f);
                }
                return;
            }
            if (colors.Length > filter.sharedMesh.subMeshCount)
            {
                Colors[] colors = new Colors[filter.sharedMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = this.colors[i];
                }
                this.colors = colors;
                return;
            }
            else if (colors.Length < filter.sharedMesh.subMeshCount)
            {
                Colors[] colors = new Colors[filter.sharedMesh.subMeshCount];
                if (this.colors.Length > 0)
                {
                    colors[0] = this.colors[0];
                }
                else
                {
                    colors[0].baseColor = Color.white;
                    colors[0].specularColor = Color.white * 0.5f;
                }
                for (int i = 1; i < colors.Length; i++)
                {
                    if (i < this.colors.Length)
                    {
                        colors[i] = this.colors[i];
                        continue;
                    }
                    colors[i] = colors[i - 1];
                }
                this.colors = colors;
                return;
            }
        }
#endif
        public static List<int[]> OneVertexArrPerSubmesh(Mesh m, bool sort = false)
        {
            List<int[]> result = null;
            //if (m.subMeshCount > 1)
            //{
            result = new List<int[]>();
            for (int i = 0; i < m.subMeshCount; i++)
            {
                int[] submeshVerts = m.GetTriangles(i).Distinct().ToArray();
                if (sort) Array.Sort(submeshVerts);
                result.Add(submeshVerts);
            }
            //}
            return result;
        }

        private void Awake()
        {
            if (filter == null) return;
            if (colors == null)
            {
                colors = new Colors[filter.sharedMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i].baseColor = Color.white;
                    colors[i].baseColor = (Color.white * 0.5f);
                }
            }
            subMeshVerticies = OneVertexArrPerSubmesh(filter.sharedMesh);
            vertexStreamsMesh = new Mesh();
            vertexStreamsMesh.vertices = filter.sharedMesh.vertices;
            Color[] colorBuffer = new Color[filter.sharedMesh.vertexCount];
            Vector4[] specularBuffer = new Vector4[filter.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
                    specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
                }
            }
            vertexStreamsMesh.colors = colorBuffer;
            vertexStreamsMesh.SetUVs(2, specularBuffer);
            vertexStreamsMesh.UploadMeshData(false);
            renderer.additionalVertexStreams = vertexStreamsMesh;
        }

#if UNITY_EDITOR
        [ContextMenu("Refresh")]
        private void Refresh()
        {
            Awake();
        }

        [ContextMenu("Translate")]
        private void Translate()
        {
            if (!filter || !renderer) return;
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                colors[i].name = materials[i].name;
                colors[i].baseColor = materials[i].GetColor(baseColorName);
                colors[i].specularColor = materials[i].GetColor(specularColorName);
                materials[i] = vertexColorMaterial;
            }
            renderer.sharedMaterials = materials;
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
            Awake();
        }

        public void RecolorBase(int index, Color color)
        {
            if (!filter || !renderer) return;
            colors[index].baseColor = color;
            Color[] colorBuffer = new Color[filter.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
                }
            }
            vertexStreamsMesh.colors = colorBuffer;
            vertexStreamsMesh.UploadMeshData(false);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }

        public void RecolorSpecular(int index, Color color)
        {
            if (!filter || !renderer) return;
            colors[index].specularColor = color;
            Vector4[] specularBuffer = new Vector4[filter.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
                }
            }
            vertexStreamsMesh.SetUVs(2, specularBuffer);
            vertexStreamsMesh.UploadMeshData(false);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }

        public void Rename(int index, string name)
        {
            colors[index].name = name;
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}