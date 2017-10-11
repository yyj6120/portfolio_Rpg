using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public class WaypointArea : MonoBehaviour
    {
        public List<Waypoint> waypoints;
        public bool randomWayPoint;

        public Waypoint GetRandomWayPoint()
        {
            System.Random random = new System.Random(100);
            var _nodes = GetValidPoints();
            var index = random.Next(0, waypoints.Count - 1);
            if (_nodes != null && _nodes.Count > 0 && index < _nodes.Count)
                return _nodes[index];

            return null;
        }

        public List<Waypoint> GetValidPoints()
        {
            var _nodes = waypoints.FindAll(node => node.isValid);
            return _nodes;
        }

        public List<Point> GetValidSubPoints(Waypoint waipoint)
        {
            var _nodes = waipoint.subPoints.FindAll(node => node.isValid);
            return _nodes;
        }

        public Waypoint GetWayPoint(int index)
        {
            var _nodes = GetValidPoints();
            if (_nodes != null && _nodes.Count > 0 && index < _nodes.Count) return _nodes[index];

            return null;
        }
    }