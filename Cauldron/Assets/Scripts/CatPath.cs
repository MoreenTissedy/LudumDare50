using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatPath : MonoBehaviour
{
    [SerializeField] private List<Transform> wayPoints;

    public Vector3[] GetPath(Transform catTransform, bool toStartSpot)
    {
        var path = new List<Vector3> 
        {
            catTransform.position
        };

        bool secondPointRightSide;
        if (toStartSpot)
        {
            secondPointRightSide = catTransform.position.x < wayPoints.First().position.x;
        }
        else
        {
            secondPointRightSide = catTransform.position.x < wayPoints.Last().position.x;
        }
        
        var secondWayPoint = FindClosestWaypoint(catTransform.position.x, secondPointRightSide);

        int index = wayPoints.IndexOf(secondWayPoint);

        switch (toStartSpot)
        {
            case true:
                for (int i = index; i >= 0; i--)
                {
                    path.Add(wayPoints[i].position);
                }
                break;
            case false:
                for (int i = index; i < wayPoints.Count; i++)
                {
                    path.Add(wayPoints[i].position);
                }
                break;
        }
        return path.ToArray();
    }

    public Transform FindClosestWaypoint(float xPos, bool rightWay)
    {
        Transform closestWaypoint = null;
        float closestDistance = float.MaxValue;

        foreach (var wayPoint in wayPoints)
        {
            float distance = Mathf.Abs(xPos - wayPoint.position.x);
            if (distance < closestDistance)
            {
                switch (rightWay)
                {
                    case true when xPos > wayPoint.position.x:
                    case false when xPos < wayPoint.position.x:
                        continue;
                    default:
                        closestDistance = distance;
                        closestWaypoint = wayPoint;
                        break;
                }
            }
        }
        
        return closestWaypoint;
    }
}
