using System;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, castDistance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void ClampToGround(Transform target)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, castDistance, groundLayer);
        if (hit.collider != null)
        {
            float halfHeight = target.GetComponent<Collider2D>().bounds.extents.y;
            Vector3 pos = target.position;
            pos.y = hit.point.y + halfHeight;
            target.position = pos;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
    }
    
}
