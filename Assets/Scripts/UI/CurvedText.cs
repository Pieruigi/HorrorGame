using TMPro;
using UnityEngine;

public class CurvedText : MonoBehaviour
{
    [Header("Curve Settings")]
    public CurveAxis curveAxis = CurveAxis.X;
    
    [Range(10f, 500f)]
    public float curveRadiusX = 100f;
    
    [Range(10f, 500f)]
    public float curveRadiusY = 100f;
    
    public bool invertCurveX = false;
    public bool invertCurveY = false;
    
    private TMP_Text m_TextComponent;
    
    public enum CurveAxis
    {
        None,
        X,
        Y,
        Both
    }
    
    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }
    
    void Start()
    {
        if (m_TextComponent != null)
        {
            StartCoroutine(WarpText());
        }
    }
    
    private System.Collections.IEnumerator WarpText()
    {
        // Aspetta che TMP sia pronto
        m_TextComponent.ForceMeshUpdate();
        yield return new WaitForSeconds(0.1f);
        
        VertexWarp();
    }
    
    private void VertexWarp()
    {
        if (m_TextComponent == null) return;
        
        m_TextComponent.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        
        int characterCount = textInfo.characterCount;
        
        if (characterCount == 0) return;
        
        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;
            
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            
            // Applica la curvatura a ogni vertice
            vertices[vertexIndex + 0] = WarpVertex(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = WarpVertex(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = WarpVertex(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = WarpVertex(vertices[vertexIndex + 3]);
        }
        
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
    
    private Vector3 WarpVertex(Vector3 pos)
    {
        switch (curveAxis)
        {
            case CurveAxis.X:
                return WarpX(pos);
            case CurveAxis.Y:
                return WarpY(pos);
            case CurveAxis.Both:
                return WarpBoth(pos);
            default:
                return pos;
        }
    }
    
    private Vector3 WarpX(Vector3 pos)
    {
        // Curva orizzontale: X rimane, modifica Y in base a X (cilindro verticale)
        float x = pos.x;
        float angle = x / curveRadiusX;
        float newY = curveRadiusY * Mathf.Sin(angle);
        float newZ = curveRadiusX * (1f - Mathf.Cos(angle));
        
        if (invertCurveX)
            newZ = -newZ;
        
        return new Vector3(x, pos.y + newY, newZ);
    }
    
    private Vector3 WarpY(Vector3 pos)
    {
        // Curva verticale: Y rimane, modifica X in base a Y (cilindro orizzontale)
        float y = pos.y;
        float angle = y / curveRadiusY;
        float newX = curveRadiusX * Mathf.Sin(angle);
        float newZ = curveRadiusY * (1f - Mathf.Cos(angle));
        
        if (invertCurveY)
            newZ = -newZ;
        
        return new Vector3(pos.x + newX, y, newZ);
    }
    
    private Vector3 WarpBoth(Vector3 pos)
    {
        // Curva sferica
        float angleX = pos.x / curveRadiusX;
        float angleY = pos.y / curveRadiusY;
        
        float offsetY = curveRadiusY * Mathf.Sin(angleX);
        float offsetX = curveRadiusX * Mathf.Sin(angleY);
        
        float zX = curveRadiusX * (1f - Mathf.Cos(angleX));
        float zY = curveRadiusY * (1f - Mathf.Cos(angleY));
        
        if (invertCurveX)
            zX = -zX;
        if (invertCurveY)
            zY = -zY;
        
        return new Vector3(pos.x + offsetX, pos.y + offsetY, zX + zY);
    }
    
    [ContextMenu("Apply Curve")]
    public void ApplyCurve()
    {
        if (m_TextComponent == null)
            m_TextComponent = GetComponent<TMP_Text>();
        
        VertexWarp();
    }
}