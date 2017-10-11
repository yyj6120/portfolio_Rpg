﻿//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System;
//using Rpg.Character;

//namespace Rpg.Item
//{
//    public class vCreateInventoryEditor : EditorWindow
//    {
//        GUISkin skin;
//        Inventory inventoryPrefab;
//        ItemListData itemListData;
//        Vector2 rect = new Vector2(480, 210);
//        Vector2 scrool;

//        [MenuItem("Invector/Inventory/ItemManager (Player Only)", false, -1)]
//        public static void CreateNewInventory()
//        {
//            GetWindow<vCreateInventoryEditor>();
//        }

//        void OnGUI()
//        {
//          //  if (!skin) skin = Resources.Load("skin") as GUISkin;
//          //  GUI.skin = skin;

//            this.minSize = rect;
//            this.titleContent = new GUIContent("Inventory System", null, "ItemManager Creator Window");

//            GUILayout.BeginVertical("ItemManager Creator Window", "window");
//            EditorGUILayout.Space();
//            EditorGUILayout.Space();
//            EditorGUILayout.Space();
//            EditorGUILayout.Space();

//            GUILayout.BeginVertical("box");

//            inventoryPrefab = EditorGUILayout.ObjectField("Inventory Prefab: ", inventoryPrefab, typeof(Inventory), false) as Inventory;
//            itemListData = EditorGUILayout.ObjectField("Item List Data: ", itemListData, typeof(ItemListData), false) as ItemListData;

//            if (inventoryPrefab != null && inventoryPrefab.GetComponent<Inventory>() == null)
//            {
//                EditorGUILayout.HelpBox("Please select a Inventory Prefab that contains the vInventory script", MessageType.Warning);
//            }

//            GUILayout.EndVertical();

//            GUILayout.BeginHorizontal("box");
//            EditorGUILayout.LabelField("Need to know how it works?");
//            if (GUILayout.Button("Video Tutorial"))
//            {
//                //Application.OpenURL("https://www.youtube.com/watch?v=1aA_PU9-G-0&index=3&list=PLvgXGzhT_qehtuCYl2oyL-LrWoT7fhg9d");
//            }
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            GUILayout.FlexibleSpace();
//            if (inventoryPrefab != null && itemListData != null)
//            {
//                if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<ThirdPersonController>() != null)
//                {
//                    if (GUILayout.Button("Create"))
//                        Create();
//                }
//                else
//                    EditorGUILayout.HelpBox("Please select the Player to add this component", MessageType.Warning);
//            }
//            GUILayout.FlexibleSpace();
//            GUILayout.EndHorizontal();

//            GUILayout.EndVertical();
//        }

//        /// <summary>
//        /// Created the ItemManager
//        /// </summary>
//        void Create()
//        {
//            if (Selection.activeGameObject != null)
//            {
//                var itemManager = Selection.activeGameObject.AddComponent<ItemManager>();
//                itemManager.inventoryPrefab = inventoryPrefab;
//                itemManager.itemListData = itemListData;
//                ItemManagerUtilities.CreateDefaultEquipPoints(itemManager, itemManager.GetComponent<MeleeManager>());
//            }
//            else
//                Debug.Log("Please select the Player to add this component.");

//            this.Close();
//        }
//    }
//}