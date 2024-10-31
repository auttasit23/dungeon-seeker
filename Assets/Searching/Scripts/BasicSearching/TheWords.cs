using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Searching
{
    public class TheWords : MonoBehaviour
    {
        char[,] board = new char[,] {
            {'s', 'e', 'a'},
            {'e', 'c', 's'},
            {'a', 't', 'h'}
        };
        string[] words = new string[] { "sea", "se", "set", "ce", "st", "tha" };

        void Start()
        {
            Array.Sort(words, (a, b) => a.Length - b.Length);
            Dictionary<string, bool> foundWords = new Dictionary<string, bool>();

            // top-left corner is (0, 0)
            // down is +1, right is +1
            // up is -1, left is -1
            var directions = new (int, int)[] {
                (0, 1), (1, 0), (0, -1), (-1, 0), // down, right, up, left
                (1, 1), (-1, -1), (-1, 1), (1, -1) // right-down, left-up, left-down, right-up
            };

            foreach (var word in words)
            {
                Debug.Log("processing world: " + word);

                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (word[0] != board[i, j])
                        {
                            continue;
                        }
                        // found the i, j in the board that match the first character of the word
                        // create visited board to keep track of the visited cell
                        // first mark all cell as unvisited (-1)
                        int[,] visited = new int[board.GetLength(0), board.GetLength(1)];
                        for (int ii = 0; ii < board.GetLength(0); ii++)
                        {
                            for (int jj = 0; jj < board.GetLength(1); jj++)
                            {
                                visited[ii, jj] = -1;
                            }
                        }

                        // mark that we found the first character of the word at i, j
                        visited[i, j] = 0;

                        for (int wi = 1; wi < word.Length; wi++)
                        {
                            // find all visited cell which is has visited value = wi - 1
                            for (int ii = 0; ii < board.GetLength(0); ii++)
                            {
                                for (int jj = 0; jj < board.GetLength(1); jj++)
                                {
                                    if (visited[ii, jj] == wi - 1)
                                    {
                                        // check all directions
                                        for (int d = 0; d < directions.Length; d++)
                                        {
                                            var (dx, dy) = directions[d];
                                            int ni = ii + dx;
                                            int nj = jj + dy;

                                            // out of the board
                                            if (ni < 0 || ni >= board.GetLength(0) || nj < 0 || nj >= board.GetLength(1))
                                            {
                                                continue;
                                            }

                                            // stamp the visited cell if the cell has the same character as the n th character of word
                                            if (board[ni, nj] == word[wi] && visited[ni, nj] == -1)
                                            {
                                                visited[ni, nj] = wi;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // print debug visited board
                        string debugVisited = "";
                        for (int ii = 0; ii < board.GetLength(0); ii++)
                        {
                            string row = "";
                            for (int jj = 0; jj < board.GetLength(1); jj++)
                            {
                                row += visited[ii, jj] + " ";
                            }
                            debugVisited += row + "\n";
                        }
                        Debug.Log(debugVisited);

                        for (int ii = 0; ii < board.GetLength(0); ii++)
                        {
                            for (int jj = 0; jj < board.GetLength(1); jj++)
                            {
                                if (visited[ii, jj] == word.Length - 1)
                                {
                                    foundWords[word] = true;
                                }
                            }
                        }
                        Debug.Log("----------------------------------------");

                    }
                }
            }

            LinkedList<string> foundWordsList = new LinkedList<string>();
            foreach (var word in words)
            {
                if (foundWords.ContainsKey(word))
                {
                    foundWordsList.AddLast(word);
                }
            }

            foreach (var word in foundWordsList)
            {
                Debug.Log(word);
            }
        }
    }
}