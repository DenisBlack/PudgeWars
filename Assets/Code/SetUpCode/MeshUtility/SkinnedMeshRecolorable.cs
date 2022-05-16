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
    public class SkinnedMeshRecolorable : MonoBehaviour
    {
        [Serializable]
        public struct Colors
        {
            public Color baseColor;
            public Color specularColor;
        }

        [SerializeField] private new SkinnedMeshRenderer renderer = default;
        [SerializeField, HideInInspector] private Colors[] colors = default;
        private List<int[]> subMeshVerticies = default;

        [SerializeField, HideInInspector] private Mesh originMesh;
        private Mesh colorMesh;

        public ref Colors this[int subMesh] => ref colors[subMesh];
        public int SubMeshCount => colorMesh.subMeshCount;
        public bool Instantiated { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!renderer || !colorMesh) return;
            if (colors == null)
            {
                colors = new Colors[colorMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i].baseColor = Color.white;
                    colors[i].baseColor = (Color.white * 0.5f);
                }
                return;
            }
            if (colors.Length > colorMesh.subMeshCount)
            {
                Colors[] colors = new Colors[colorMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = this.colors[i];
                }
                this.colors = colors;
                return;
            }
            else if (colors.Length < colorMesh.subMeshCount)
            {
                Colors[] colors = new Colors[colorMesh.subMeshCount];
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
            if (renderer == null) return;
            if (originMesh == null)
            {
                originMesh = renderer.sharedMesh;
            }
            if (colorMesh == null || colorMesh.vertexCount != originMesh.vertexCount)
            {
                colorMesh = Instantiate(originMesh);
            }
            if (colors == null)
            {
                colors = new Colors[renderer.sharedMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i].baseColor = Color.white;
                    colors[i].baseColor = (Color.white * 0.5f);
                }
            }
            if (colors.Length > colorMesh.subMeshCount)
            {
                Colors[] colors = new Colors[colorMesh.subMeshCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = this.colors[i];
                }
                this.colors = colors;
            }
            else if (colors.Length < colorMesh.subMeshCount)
            {
                Colors[] colors = new Colors[colorMesh.subMeshCount];
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
            }
            subMeshVerticies = OneVertexArrPerSubmesh(colorMesh);
            Color[] colorBuffer = new Color[colorMesh.vertexCount];
            Vector4[] specularBuffer = new Vector4[colorMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
                    specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
                }
            }
            colorMesh.colors = colorBuffer;
            colorMesh.SetUVs(2, specularBuffer);
            colorMesh.UploadMeshData(false);
            renderer.sharedMesh = colorMesh;
            Instantiated = true;
        }

        public void RuntimeRecolorBase(int index, Color color)
        {
            if (!renderer) return;
            colors[index].baseColor = color;
            Color[] colorBuffer = new Color[renderer.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
                }
            }
            colorMesh.colors = colorBuffer;
            colorMesh.UploadMeshData(false);
        }

        public void RuntimeRecolorSpecular(int index, Color color)
        {
            if (!renderer) return;
            colors[index].specularColor = color;
            Vector4[] specularBuffer = new Vector4[renderer.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
                }
            }
            colorMesh.SetUVs(2, specularBuffer);
            colorMesh.UploadMeshData(false);
        }

        private void RuntimeResetMesh()
        {
            originMesh = null;
            colorMesh = null;
            Awake();
        }

        //private void OnEnable()
        //{
        //    Color[] colorBuffer = new Color[filter.sharedMesh.vertexCount];
        //    Vector4[] specularBuffer = new Vector4[filter.sharedMesh.vertexCount];
        //    for (int i = 0; i < subMeshVerticies.Count; i++)
        //    {
        //        for (int j = 0; j < subMeshVerticies[i].Length; j++)
        //        {
        //            colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
        //            specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
        //        }
        //    }
        //    vertexStreamsMesh.colors = colorBuffer;
        //    vertexStreamsMesh.SetUVs(2, specularBuffer);
        //    vertexStreamsMesh.UploadMeshData(false);
        //}

#if UNITY_EDITOR
        //public void OnEditorEnable()
        //{
        //    if (!filter || !renderer) return;
        //    vertexStreamsMesh = new Mesh();
        //    Color[] colorBuffer = new Color[filter.sharedMesh.vertexCount];
        //    for (int i = 0; i < colorBuffer.Length; i++)
        //    {
        //        colorBuffer[i] = baseColor;
        //    }
        //    vertexStreamsMesh.vertices = filter.sharedMesh.vertices;
        //    vertexStreamsMesh.colors = colorBuffer;
        //    Vector4[] specularBuffer = new Vector4[filter.sharedMesh.vertexCount];
        //    for (int i = 0; i < specularBuffer.Length; i++)
        //    {
        //        specularBuffer[i] = specularColor;
        //    }
        //    vertexStreamsMesh.SetUVs(2, specularBuffer);
        //    vertexStreamsMesh.UploadMeshData(false);
        //    renderer.additionalVertexStreams = vertexStreamsMesh;
        //}

        [ContextMenu("Reset Mesh")]
        private void ResetMesh()
        {
            originMesh = null;
            colorMesh = null;
            Awake();
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }

        public void RecolorBase(int index, Color color)
        {
            if (!renderer) return;
            colors[index].baseColor = color;
            Color[] colorBuffer = new Color[renderer.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    colorBuffer[subMeshVerticies[i][j]] = colors[i].baseColor;
                }
            }
            colorMesh.colors = colorBuffer;
            colorMesh.UploadMeshData(false);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }

        public void RecolorSpecular(int index, Color color)
        {
            if (!renderer) return;
            colors[index].specularColor = color;
            Vector4[] specularBuffer = new Vector4[renderer.sharedMesh.vertexCount];
            for (int i = 0; i < subMeshVerticies.Count; i++)
            {
                for (int j = 0; j < subMeshVerticies[i].Length; j++)
                {
                    specularBuffer[subMeshVerticies[i][j]] = colors[i].specularColor;
                }
            }
            colorMesh.SetUVs(2, specularBuffer);
            colorMesh.UploadMeshData(false);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}