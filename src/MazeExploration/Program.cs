using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeExploration
{
    class Node
    {
        public Node(int x, int y)
        {
            X = x;
            Y = y;
            Score = 0;
        }
        
        public Node(int x, int y, Node parent)
        {
            X = x;
            Y = y;
            Parent = parent;
            Score = parent?.Score + 1 ?? 0;
        }
        
        public int X { get; }
        public int Y { get; }
        public int Score { get; }
        public Node Parent { get; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            // 幅優先探索
            
            var maze = @"
**************************
*S* *                    *
* * *  *  *************  *
* *   *    ************  *
*    *                   *
********************** ***
*                        *
** ***********************
*      *              G  *
*  *      *********** *  *
*    *        ******* *  *
*       *                *
**************************
";
            var mazeArray = maze
                .Split(Environment.NewLine)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToCharArray())
                .ToArray();

            var labels = Enumerable.Range(0, mazeArray.Length)
                .Select(x => Enumerable.Repeat(false, mazeArray[x].Length).ToArray()).ToArray();
            
            var start = new Func<(int x, int y)>(() =>
            {
                for (int y = 0; y < mazeArray.Length; ++y)
                {
                    var line = mazeArray[y];
                    for (int x = 0; x < line.Length; ++x)
                    {
                        var c = line[x];
                        if (c == 'S')
                        {
                            return (x, y);
                        }
                    }
                }
                throw new InvalidOperationException("start is not found.");
            }).Invoke();
            
            var queue = new Queue<Node>();
            queue.Enqueue(new Node(start.x, start.y));
            labels[start.y][start.x] = true;

            Node UpdateQueue(int x, int y, Node parent)
            {
                if (0 <= y && y < mazeArray.Length)
                {
                    var line = mazeArray[y];
                    if (0 <= x && x < line.Length)
                    {
                        var c = line[x];
                        var l = labels[y][x];
                        if (c == ' ' && !l)
                        {
                            queue.Enqueue(new Node(x,y,parent));
                            labels[y][x] = true;
                        }
                        else if (c == 'G')
                        {
                            return parent;
                        }
                    }
                }

                return null;
            }

            Node result = null;
            
            var count = 0;
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                {
                    result = UpdateQueue(node.X, node.Y + 1, node);
                    if (result != null) break;
                    result = UpdateQueue(node.X - 1, node.Y, node);
                    if (result != null) break;
                    result = UpdateQueue(node.X, node.Y - 1, node);
                    if (result != null) break;
                    result = UpdateQueue(node.X + 1, node.Y, node);
                    if (result != null) break;
                }
                count++;
            }
            
            if(result == null)
                throw new InvalidOperationException("goal is not found.");

            Node current = result;
            while (current != null)
            {
                if (current.Parent != null)
                {
                    mazeArray[current.Y][current.X] = '$';
                }
                current = current.Parent;
            }

            var output = string.Join(Environment.NewLine, mazeArray.Select(line => string.Join("", line)));
            Console.WriteLine(output);
            Console.WriteLine($"score: {result.Score}, Count: {count}");
        }
    }
}
