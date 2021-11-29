using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueTool
{
   
    public class DialogueNode : ScriptableObject
    {
        public string name;
        public string text;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);


    }
}

