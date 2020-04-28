using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class AttackGraph
    {

        private static bool[] scratchPad = new bool[100];

        Dictionary<int, ControllableNode> ours = new Dictionary<int, ControllableNode>();
        Dictionary<int, Node> theirs = new Dictionary<int, Node>();
        List<Link> links = new List<Link>();

        public static readonly float KillValue = 1.0f;
        public static readonly float LossValue = -1.1f;
        

        public class Node
        {
            int id = 0;
            public List<Link> links = new List<Link>();

            public float GetSurvivalRate()
            {
                float result = 0.0f;
                Node[] enemies = links.Select(l => l.GetOther(this)).ToArray();
                float[] probabilities = links.Select(l => l.options[(int)l.direction].probability).ToArray();

                int nLinks = links.Count;

                int maxValue = 0x01 << nLinks;
                for (int i = 0; i < maxValue; i++)
                {
                    int nActiveLinks = CountBits(i);
                    float pNotKilling = 1.0f;
                    float pOccurence = 1.0f;

                    for(int j = 0; j < nLinks; j++)
                    {
                        bool activeLink = (i & (0x01 << j)) != 0;

                        pOccurence *= activeLink ? probabilities[j] : 1.0f - probabilities[j];

                        if (activeLink)
                            pNotKilling *= enemies[j].GetProbabilityOfNotKilling(this, nActiveLinks);
                    }

                    result += pOccurence * pNotKilling;
                }

                return result;
            }

            public float GetProbabilityOfNotKilling(Node other, int otherActiveLinks)
            {
                float[] probabilities = links
                                .Where(l => l.GetOther(this) != other)
                                .Select(l => l.options[(int)l.direction].probability)
                                .ToArray();

                float result = 0.0f;

                int nLinks = probabilities.Length;
                int maxValue = 0x01 << nLinks;
                for (int i = 0; i < maxValue; i++)
                {
                    int nActiveLinks = CountBits(i);

                    if (nActiveLinks + 1 <= otherActiveLinks)
                        continue;
                    
                    float pOccurence = 1.0f;

                    for (int j = 0; j < nLinks; j++)
                    {
                        bool activeLink = (i & (0x01 << j)) != 0;
                        pOccurence *= activeLink ? probabilities[j] : 1.0f - probabilities[j];
                    }

                    result += pOccurence;
                }
                
                return result;

            }

            public float GetExpectedLinkCount()
            {
                return links.Sum(l => l.options[(int)l.direction].probability);
            }

           
        }

        public class ControllableNode : Node
        {
            public Direction direction = Direction.Halt;
        }


        public class Link
        {
            public Direction direction = Direction.Halt;
            public LinkOption[] options = new LinkOption[5];

            public Node a;
            public Node b;

            public Node GetOther(Node node)
            {
                if (node == a)
                    return b;
                else
                    return a;
            }

            public Vector2i GetDestination(Direction direction)
            {
                return options[(int)direction].destination;
            }

            public float GetProbability()
            {
                return options[(int)direction].probability;
            }
        }

        public struct LinkOption
        {
            public float probability;
            public Vector2i destination;
            public bool available;
        }



        public void AddNodeGroup(int myAnt, int theirAnt, IEnumerable<LinkOption> options)
        {
            if (!ours.ContainsKey(myAnt))
                ours.Add(myAnt, new ControllableNode());

            ControllableNode myAntNode = ours[myAnt];

            if (!theirs.ContainsKey(theirAnt))
                theirs.Add(theirAnt, new Node());

            Node theirAntNode = theirs[theirAnt];


            Link link = new Link();
            link.a = myAntNode;
            link.b = theirAntNode;
            link.options = options.ToArray();

            myAntNode.links.Add(link);
            theirAntNode.links.Add(link);
        }

        public float GetValue()
        {
            float kills = 0.0f;
            foreach (Node node in theirs.Values)
                kills += 1.0f - node.GetSurvivalRate();

            float losses = 0.0f;
            foreach (Node node in ours.Values)
                losses += 1.0f - node.GetSurvivalRate();

            return KillValue * kills + LossValue * losses;
        }

        public Direction GetDirection(int antId)
        {
            return ours[antId].direction;
        }

        public float GetEngagement(int antId)
        {
            ControllableNode myAnt = ours[antId];
            return myAnt.links.Sum(l => l.options[(int)myAnt.direction].probability);
        }

        public float GetMySurvivalRate(int antId)
        {
            return ours[antId].GetSurvivalRate();
        }

        public float GetTheirSurvivalRate(int antId)
        {
            return theirs[antId].GetSurvivalRate();
        }

        public Vector2i GetProjectedPosition(int antId)
        {
            return ours[antId].links[0].GetDestination(ours[antId].direction);
        }

        public void SetDirection(int id, Direction direction)
        {
            ours[id].direction = direction;
            ours[id].links.ForEach(l => l.direction = direction);
        }



        public void Clear()
        {
            ours.Clear();
            theirs.Clear();
            links.Clear();
        }

        private static int CountBits(int data)
        {
            int result = 0;
            while (data > 0)
            {
                if ((data & 0x01) != 0)
                    result++;

                data >>= 1;
            }
            return result;
        }

    }
}
