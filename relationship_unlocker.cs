using MelonLoader;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(UnlockAllRelationshipsMod.Main), "RelationshipInspector", "1.0.0", "DriizzyyB")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace UnlockAllRelationshipsMod
{
    public class Main : MelonMod
    {
        [Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("[RelationshipInspector] Game started. Scanning all loaded types:");
            ListMatchingTypes();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                MelonLogger.Msg("F7 pressed - scanning all manager types for relationship data...");
                TryDumpAllLikelyManagers();
            }
        }

        private void ListMatchingTypes()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    string name = type.FullName.ToLower();
                    if (name.Contains("customer") || name.Contains("manager") || name.Contains("relationship") || name.Contains("person") || name.Contains("people"))
                    {
                        MelonLogger.Msg("Type: " + type.FullName);
                    }
                }
            }
        }

        private void TryDumpAllLikelyManagers()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (!type.FullName.ToLower().Contains("manager")) continue;

                    object instance = null;
                    var instanceProp = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (instanceProp != null)
                    {
                        instance = instanceProp.GetValue(null);
                    }

                    var instanceField = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (instanceField != null && instance == null)
                    {
                        instance = instanceField.GetValue(null);
                    }

                    if (instance != null)
                    {
                        MelonLogger.Msg($"\n[Found Manager] {type.FullName}");
                        DumpObjectFields(instance, 1);
                    }
                }
            }
        }

        private void DumpObjectFields(object obj, int indentLevel)
        {
            var type = obj.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            string indent = new string(' ', indentLevel * 2);

            foreach (var field in type.GetFields(flags))
            {
                try
                {
                    var value = field.GetValue(obj);
                    MelonLogger.Msg($"{indent}{field.Name}: {value}");
                }
                catch { }
            }

            foreach (var prop in type.GetProperties(flags))
            {
                if (!prop.CanRead) continue;
                try
                {
                    var value = prop.GetValue(obj);
                    MelonLogger.Msg($"{indent}{prop.Name}: {value}");
                }
                catch { }
            }
        }
    }
}