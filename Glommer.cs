using System;
using System.Collections.Generic;
using System.Text;

namespace OsmGlommer
{
    public class Glommer
    {
        private OSMDataset osmData;

        // List of ways connecting to each endpoint node, keyed by Node ID
        private Dictionary<Int64, List<OSMWay>> endNodeWayList;
        private List<OSMWay> removeWay;

        public void DoGlom(OSMDataset osmDataset)
        {
            osmData = osmDataset;
            BuildEndpointList();
            PerformGlom();
            PurgeRemovedWays();
            ResetBBoxes();
        }


        private void ResetBBoxes()
        {
            foreach (var way in osmData.osmWays.Values)
            {
                SpatialUtilities.SetBboxFor(way);
                way.SetCenter();
            }
        }


        private void PurgeRemovedWays()
        {
            foreach(var way in removeWay)
            {
                osmData.osmWays.Remove(way.ID);
            }
        }

        private void PerformGlom()
        {
            removeWay = new List<OSMWay>();

            foreach (var id in endNodeWayList.Keys)
            {

                var wayList = endNodeWayList[id];
                foreach (var way in wayList)
                {
                    if (way.IsUsed) continue;  // Someone already glommed this way

                    OSMWay extendingWay = FindMatchingTags(way, wayList);
                    if (extendingWay != null)
                    {
                        var intersectionNode = osmData.osmNodes[id];
                        ExtendWay(way, intersectionNode, extendingWay);
                        extendingWay.IsUsed = true; // Mark as used since it was attached to 'way'
                        removeWay.Add(extendingWay);

                    }
                }
            }
        }


        /// <summary>
        /// Extend way by concatenating 'extendingWay' onto 'way' at the intersection node
        /// </summary>
        /// <param name="way"></param>
        /// <param name="intersectionNode"></param>
        /// <param name="extendingWay"></param>
        private void ExtendWay(OSMWay way, OSMNode intersectionNode, OSMWay extendingWay)
        {
            int direction = 1;
            int start = 0;
            OSMNode newIntersectNode = null;
            if (extendingWay.NodeList[0] == intersectionNode)
            {
                // Sequence Logic already set above

                newIntersectNode = extendingWay.NodeList[extendingWay.NodeList.Count - 1]; // Last node
            }
            else if (extendingWay.NodeList[extendingWay.NodeList.Count - 1] == intersectionNode)
            {
                // Attach from end of this way
                direction = -1;
                start = extendingWay.NodeList.Count - 1;
                newIntersectNode = extendingWay.NodeList[0]; // First node
            }


            if (way.NodeList[0] == intersectionNode)
            {
                // Attach before first node
                PrependTo(way, extendingWay, direction, start);

            } else if (way.NodeList[way.NodeList.Count-1] == intersectionNode)
            {
                // Attach after last node
                AppendTo(way, extendingWay, direction, start);
            }
            else
            {
                // 3-way intersection at same node?
                Console.WriteLine("Logic error during glom.");
            }

            if (newIntersectNode.ID != intersectionNode.ID)
            {
                ChangeIntersectingWay(newIntersectNode, extendingWay, way);
            }

            DropDuplicateNodes(way);
        }

        /// <summary>
        /// Remove duplicate node from extending way
        /// </summary>
        /// <param name="way"></param>
        private void DropDuplicateNodes(OSMWay way)
        {
            var lastNode = way.NodeList[0];
            for (int i= 1; i < way.NodeList.Count; i++)
            {
                var node = way.NodeList[i];
                if (node.ID == lastNode.ID)
                {
                    way.NodeList.RemoveAt(i); // Found a duplicate (normally only 1)
                    break;
                }
                lastNode = node;
            }
        }

        /// <summary>
        /// Since the way was extended - adjust that intersection to show that the tip of the 'extendedWay' is actually the
        /// new concatenated way.
        /// </summary>
        /// <param name="newIntersectNode"></param>
        /// <param name="extendingWay"></param>
        /// <param name="way"></param>
        private void ChangeIntersectingWay(OSMNode newIntersectNode, OSMWay extendingWay, OSMWay way)
        {
            var wayList = endNodeWayList[newIntersectNode.ID];
            wayList.Remove(extendingWay);
            wayList.Add(way);
        }

        private void AppendTo(OSMWay way, OSMWay extendingWay, int direction, int start)
        {
            int index = start;
            for (int i=0; i < extendingWay.NodeList.Count; i++)
            {
                way.NodeList.Add(extendingWay.NodeList[index]);
                index += direction;
            }
        }


        private void PrependTo(OSMWay way, OSMWay extendingWay, int direction, int start)
        {
            int index = start;
            for (int i = 0; i < extendingWay.NodeList.Count; i++)
            {
                way.NodeList.Insert(0, extendingWay.NodeList[index]);
                index += direction;
            }
        }



        /// <summary>
        /// Check if 2 ways have identical tags
        /// </summary>
        /// <param name="way"></param>
        /// <param name="testWay"></param>
        /// <returns>true if identical</returns>
        private bool IdenticalTags(OSMWay way, OSMWay testWay)
        {
            bool isSame = false;

            if (way.Tags.Count == testWay.Tags.Count)
            {
                foreach (string tagName in way.Tags.Keys)
                {
                    string tagValue = way.Tags[tagName];
                    if (!testWay.Tags.ContainsKey(tagName) ||
                        !testWay.Tags[tagName].Equals(tagValue))
                    {
                        return false;
                    }
                }
                isSame = true;
            }

            return isSame;
        }


        /// <summary>
        /// Search for a way with identical tags
        /// </summary>
        /// <param name="way"></param>
        /// <param name="wayList"></param>
        /// <returns></returns>
        private OSMWay FindMatchingTags(OSMWay way, List<OSMWay> wayList)
        {
            int foundCount = 0;
            OSMWay foundWay = null;
            foreach(var testWay in wayList)
            {
                if (testWay.ID != way.ID && !testWay.IsUsed)
                {
                    if (IdenticalTags(way, testWay)) 
                    { 
                            foundCount++;
                            foundWay = testWay;
                    }
                }
            }

            if (foundCount == 1)
            {
                return foundWay;
            }
            // Found 0 or 2+ matching ways
            return null;
        }

        private void BuildEndpointList()
        {
            endNodeWayList = new Dictionary<long, List<OSMWay>>();
            foreach (var way in osmData.osmWays.Values)
            {
                // Add each end
                AddEndNode(way.NodeList[0], way);
                AddEndNode(way.NodeList[way.NodeList.Count-1], way);
            }
        }

        private void AddEndNode(OSMNode osmNode, OSMWay way)
        {
            var id = osmNode.ID;
            if (!endNodeWayList.ContainsKey(id))
            {
                var newWayList = new List<OSMWay>();
                endNodeWayList.Add(id, newWayList);
            }

            var wayList = endNodeWayList[id];
            if (!wayList.Contains(way)) wayList.Add(way);
        }
    }
}
