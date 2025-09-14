using UnityEngine;
using System.Collections;

public class SpriteGizmo : MonoBehaviour 
{
    [SerializeField]
    private Color   m_color;
    public Color Color
    {
        get { return m_color; }
        set { m_color = value; }
    }

    [SerializeField]
    private string  m_sprite;
    public string Sprite
    {
        get { return m_sprite; }
        set { m_sprite = value; }
    }

    [SerializeField]
    private Vector3 m_offset;
    public Vector3 Offset
    {
        get { return m_offset;}
        set { m_offset = value; }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color;
        Gizmos.DrawIcon(transform.position + Offset, Sprite, true);
    }
}
