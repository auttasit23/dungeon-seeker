using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tree
{

    public class DialogueManager : MonoBehaviour
    {
        public DialogueTree tree;
        public DialogueNode currentNode;

        public void Start()
        {
            LoadConversations();
            currentNode = tree.root;
            currentNode.Print();
        }

        private void LoadConversations()
        {
            #region Depicting the dialogue tree
            // NPC: Ah, traveler! What brings you to this old place?
            //     |
            //     +-- [1] Can you give me a quest?
            //     |       |
            //     |       +-- NPC: I have a task for you. There’s a beast in the woods. Can you take care of it?
            //     |               |
            //     |               +-- [1] I’m ready for anything!
            //     |               |       |
            //     |               |       +-- NPC: You're not ready for this yet. Come back when you're stronger.
            //     |               |
            //     |               +-- [2] Maybe later.
            //     |                       |
            //     |                       +-- NPC: Safe travels, adventurer.
            //     |
            //     +-- [2] Where is the village?
            //     |       |
            //     |       +-- NPC: Follow the road south, and you’ll reach the village.
            //     |
            //     +-- [3] How do I get to the forest?
            //     |       |
            //     |       +-- NPC: Head west, into the forest. But beware, it's dangerous.
            //     |
            //     +-- [4] Goodbye.
            //             |
            //             +-- NPC: Safe travels, adventurer.
            #endregion

            // Create the dialogue nodes
            DialogueNode greeting = new DialogueNode("Ah, traveler! What brings you to this old place?");
            DialogueNode askForQuest = new DialogueNode("I have a task for you. There’s a beast in the woods. Can you take care of it?");
            DialogueNode questDenied = new DialogueNode("You're not ready for this yet. Come back when you're stronger.");
            DialogueNode directionsVillage = new DialogueNode("Follow the road south, and you’ll reach the village.");
            DialogueNode directionsForest = new DialogueNode("Head west, into the forest. But beware, it's dangerous.");
            DialogueNode goodbye = new DialogueNode("Safe travels, adventurer.");
            DialogueNode noIdea = new DialogueNode("I'm afraid I can't help you with that.");

            // Build the tree, adding custom responses
            greeting.AddNext(askForQuest, "Can you give me a quest?");
            greeting.AddNext(directionsVillage, "Where is the village?");
            greeting.AddNext(directionsForest, "How do I get to the forest?");
            greeting.AddNext(goodbye, "Goodbye.");

            askForQuest.AddNext(questDenied, "I’m ready for anything!");
            askForQuest.AddNext(goodbye, "Maybe later.");

            // Set up the root of the dialogue tree
            tree = new DialogueTree(greeting);
        }

        void Update()
        {
            int choice = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1)) choice = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) choice = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) choice = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) choice = 4;

            if (choice != -1)
            {
                var choiceText = new List<string>(currentNode.nexts.Keys);
                if (choice <= choiceText.Count)
                {
                    int index = choice - 1; // 1-based => 0-based
                    Debug.Log("Player: " + choiceText[index]);
                    currentNode = currentNode.nexts[choiceText[index]];
                    currentNode.Print();
                }
            }
        }
    }

}