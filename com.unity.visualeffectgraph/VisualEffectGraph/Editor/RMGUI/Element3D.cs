using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor;

namespace UnityEngine.Experimental.UIElements
{
    public class Element3D : VisualElement
    {
        Mesh m_Mesh;

        Material m_Material;

        Material m_LineMaterial;

        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }

        public Element3D()
        {
            clipChildren = true;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);

            m_Mesh = go.GetComponent<MeshFilter>().sharedMesh;
            m_Material = go.GetComponent<MeshRenderer>().sharedMaterial;

            GameObject.DestroyImmediate(go);

            position = new Vector3(0, 0, -5);
            rotation = Quaternion.identity;

            m_LineMaterial = new Material(Shader.Find("Unlit/Element3DGridShader"));
            m_LineMaterial.color = Color.gray;
        }

        RenderTexture m_RenderTexture;
        Texture2D m_BlitTexture;

        public override void DoRepaint()
        {
            Rect panelRect = this.panel.visualTree.layout;

            Rect viewPort = this.parent.ChangeCoordinatesTo(this, layout);

            if (m_RenderTexture == null)
            {
                m_RenderTexture = new RenderTexture(Mathf.CeilToInt(viewPort.width), Mathf.CeilToInt(viewPort.height), 32, RenderTextureFormat.Default);
            }
            if (m_RenderTexture.width != Mathf.CeilToInt(viewPort.width))
            {
                m_RenderTexture.Release();
                m_RenderTexture.width = Mathf.CeilToInt(viewPort.width);
            }
            if (m_RenderTexture.height != Mathf.CeilToInt(viewPort.height))
            {
                m_RenderTexture.Release();

                m_RenderTexture.height = Mathf.CeilToInt(viewPort.height);
            }

            if (m_BlitTexture == null || m_BlitTexture.height != m_RenderTexture.height || m_BlitTexture.width != m_RenderTexture.width)
            {
                if (m_BlitTexture != null)
                    m_BlitTexture.Resize(m_RenderTexture.width, m_BlitTexture.height);
                else
                {
                    m_BlitTexture = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.ARGB32, false);
                    style.backgroundImage = m_BlitTexture;
                }
            }

            //EditorGUIUtility.SetRenderTextureNoViewport(m_RenderTexture);
            RenderTexture.active = m_RenderTexture;

            GL.PushMatrix();
            //GL.Viewport(viewPort);
            GL.Clear(true, true, Color.red);
#if true
            GL.LoadProjectionMatrix(Matrix4x4.Perspective(60, viewPort.width / viewPort.height, 0.01f, 100));
            GL.modelview = Matrix4x4.Translate(position) * Matrix4x4.Rotate(rotation);


            m_LineMaterial.SetPass(0);


            float count = 20;

            GL.Begin(GL.LINES);
            for (float x = -count; x <= count; x++)
            {
                GL.Vertex3(x, 0, -count);
                GL.Vertex3(x, 0, count);
            }
            GL.End();

            GL.Begin(GL.LINES);
            for (float x = -count; x <= count; x++)
            {
                GL.Vertex3(-count, 0, x);
                GL.Vertex3(count, 0, x);
            }
            GL.End();
            GL.invertCulling = true;
            m_Material.SetPass(0);
            Graphics.DrawMeshNow(m_Mesh, Matrix4x4.identity);

            //Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, m_Material, 1);

#endif
            GL.PopMatrix();
            m_BlitTexture.ReadPixels(viewPort, 0, 0);

            RenderTexture.active = null;
            m_BlitTexture.Apply();

            base.DoRepaint();
        }
    }
}
