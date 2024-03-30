using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ModifiableSpriteShape : MonoBehaviour{
    SpriteShapeController spriteShapeController;
    void Start(){
        spriteShapeController = GetComponent<SpriteShapeController>();
    }
    public void AddPoint(Vector3 pos){
        spriteShapeController.spline.InsertPointAt(spriteShapeController.spline.GetPointCount(), pos);
        spriteShapeController.BakeCollider();
        spriteShapeController.BakeMesh();
    }
    public void RemoveLastPoint()
    {
        int pointCount = spriteShapeController.spline.GetPointCount();
        if (pointCount > 0)
        {
            spriteShapeController.spline.RemovePointAt(pointCount - 1);
            spriteShapeController.BakeCollider();
            spriteShapeController.BakeMesh();
        }
    }
    public void SetPoint(int index, Vector3 newPosition)
    {
        if (index >= 0 && index < spriteShapeController.spline.GetPointCount())
        {
            spriteShapeController.spline.SetPosition(index, newPosition);
            spriteShapeController.BakeCollider();
            spriteShapeController.BakeMesh();
        }
    }
    public Vector2 CalculateThirdPoint(Vector2 point1, Vector2 point2)
    {
        // Calculate the distance between point1 and point2
        float distance = Vector2.Distance(point1, point2);

        // Calculate the position of the third point along the line between point1 and point2
        // Here, we're assuming that point1 is at the bottom-left corner and point2 is at the top-right corner
        Vector2 direction = (point2 - point1).normalized;
        return point1 + direction * distance;
    }
}
