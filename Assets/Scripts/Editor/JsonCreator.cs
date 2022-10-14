using Newtonsoft.Json;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using Node;

namespace Editor.Json
{
    public class JsonCreator : OdinMenuEditorWindow
    {
        private List<NodeObject> nodes = new List<NodeObject>();

        [SerializeField, FolderPath]
        private string PathToFile;

        [MenuItem("Tools/Json creator")]
        private static void Open()
        {
            var window = GetWindow<JsonCreator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        private void DeserializeJson(OdinMenuTree tree)
        {
            var path = Path.Combine("Assets", "StreamingAssets", "Config.json");
            var json = File.ReadAllText(path);
            nodes = JsonConvert.DeserializeObject<List<NodeObject>>(json);

            FullDesirialization(tree, nodes);

            Debug.Log(nodes.Count);
        }

        private void FullDesirialization(OdinMenuTree tree, List<NodeObject> nodeObjects, string name = "")
        {
            foreach (var node in nodeObjects)
            {
                if (node.NodeObjects != null && node.NodeObjects.Count > 0)
                {
                    tree.Add(node.Name, node);

                    foreach (var nodeObject in node.NodeObjects)
                    {
                        tree.Add(nodeObject.Name, nodeObject);

                        if (nodeObject.NodeObjects != null && nodeObject.NodeObjects.Count > 0)
                        {
                            FullDesirialization(tree, node.NodeObjects);
                            SetDescriprion(node.NodeObjects);
                            SetDescriprion(nodeObject.NodeObjects);
                        }
                    }
                }
            }

        }

        private void FullSerialization(NodeObject nodeObj = null, string name = "", List<NodeObject> nodeObjects = null)
        {
            if (nodeObjects == null && nodeObj != null)
            {
                if (!string.IsNullOrEmpty(name) && !nodeObj.Name.Contains(name))
                {
                    var split = nodeObj.Name.LastIndexOf('/');
                    var splited = nodeObj.Name.Substring(split + 1);
                    nodeObj.Name = name + splited;
                }
            }

            else
            {
                foreach (var node in nodeObjects)
                {
                    if (node.NodeObjects != null && node.NodeObjects.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(name) && !node.Name.Contains(name))
                        {
                            node.Name = name + node.Name;
                        }

                        foreach (var nodeObject in node.NodeObjects)
                        {
                            if (nodeObject.NodeObjects != null && nodeObject.NodeObjects.Count > 0)
                                FullSerialization(name: node.Name + "/", nodeObjects: node.NodeObjects);
                            else
                                FullSerialization(nodeObject, node.Name + "/");
                        }
                    }
                }
            }


        }

        private void SaveNewJson()
        {
            FullSerialization(nodeObjects: nodes);

            var path = Path.Combine("Assets", "StreamingAssets", "Config.json");

            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, nodes);
            }
        }

        private void SetDescriprion(List<NodeObject> nodeObjects)
        {
            foreach (var node in nodeObjects)
            {
                string descriptionString = "";
                var subNames = node.Name.Split('/');
                var lastIndex = subNames.Length - 1;

                if (lastIndex - 1 >= 0)
                {
                    string children = "";
                    if (node.NodeObjects != null)
                    {
                        foreach (var nodeObject in node.NodeObjects)
                        {
                            var subNamesChild = nodeObject.Name.Split('/');
                            children += $"{subNamesChild[subNamesChild.Length - 1]} ";
                        }

                    }

                    descriptionString += $"{subNames[lastIndex]}" +
                    $"\nПредки: {subNames[lastIndex - 1]}" +
                    $"\nПотомки: {children}";
                }
                node.Description = descriptionString;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            DeserializeJson(tree);

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Save")))
                {
                    SaveNewJson();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

}