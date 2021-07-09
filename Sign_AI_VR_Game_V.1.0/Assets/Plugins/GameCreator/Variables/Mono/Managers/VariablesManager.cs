namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class VariablesManager
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public static VariablesEvents events = new VariablesEvents();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Reset()
        {
            GlobalVariablesManager.Instance.ResetData();

            foreach (KeyValuePair<string, LocalVariables> localVariable in LocalVariables.REGISTER)
            {
                if (localVariable.Value == null) continue;
                localVariable.Value.ResetData();
            }
        }

        // GETTERS: -------------------------------------------------------------------------------

        public static object GetGlobal(string name)
        {
            Variable variable = GlobalVariablesManager.Instance.Get(name);
            return (variable != null ? variable.Get() : null);
        }

        public static object GetLocal(GameObject target, string name, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            return (variable != null ? variable.Get() : null);
        }

        public static Variable.DataType GetGlobalType(string name, bool inChildren = false)
        {
            Variable variable = GlobalVariablesManager.Instance.Get(name);
            return (variable != null
                ? (Variable.DataType)variable.type
                : Variable.DataType.Null
            );
        }

        public static Variable.DataType GetLocalType(GameObject target, string name, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            return (variable != null
                ? (Variable.DataType)variable.type
                : Variable.DataType.Null
            );
        }

        public static Variable GetListItem(GameObject target, ListVariables.Position position, int index = 0)
        {
            ListVariables list = target.GetComponent<ListVariables>();
            return GetListItem(list, position);
        }

        public static Variable GetListItem(ListVariables list, ListVariables.Position position, int index = 0)
        {
            return list.Get(position, index);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public static void SetGlobal(string name, object value)
        {
            Variable variable = GlobalVariablesManager.Instance.Get(name);
            if (variable != null)
            {
                variable.Update(value);
                VariablesManager.events.OnChangeGlobal(name);
            }
        }

        public static void SetLocal(GameObject target, string name, object value, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            if (variable != null)
            {
                variable.Update(value);
                VariablesManager.events.OnChangeLocal(target, name);
            }
        }

        public static void ListPush(GameObject target, int index, object value)
        {
            ListVariables list = target.GetComponent<ListVariables>();
            ListPush(list, index, value);
        }

        public static void ListPush(ListVariables target, int index, object value)
        {
            target.Push(value, index);
        }

        public static void ListPush(GameObject target, ListVariables.Position position, object value)
        {
            ListVariables list = target.GetComponent<ListVariables>();
            ListPush(list, position, value);
        }

        public static void ListPush(ListVariables target, ListVariables.Position position, object value)
        {
            target.Push(value, position);
        }

        public static void ListRemove(GameObject target, int index)
        {
            ListVariables list = target.GetComponent<ListVariables>();
            ListRemove(list, index);
        }

        public static void ListRemove(ListVariables target, int index)
        {
            target.Remove(index);
        }

        public static void ListRemove(GameObject target, ListVariables.Position position)
        {
            ListVariables list = target.GetComponent<ListVariables>();
            ListRemove(list, position);
        }

        public static void ListRemove(ListVariables target, ListVariables.Position position)
        {
            target.Remove(position);
        }

        public static void ListClear(GameObject target)
        {
            ListClear(target.GetComponent<ListVariables>());
        }

        public static void ListClear(ListVariables target)
        {
            if (target == null) return;
            for (int i = target.variables.Count - 1; i >= 0; --i)
            {
                VariablesManager.ListRemove(target, i);
            }
        }

        // CHECKERS: ------------------------------------------------------------------------------

        public static bool ExistsGlobal(string name)
        {
            Variable variable = GlobalVariablesManager.Instance.Get(name);
            return variable != null;
        }

        public static bool ExistsLocal(GameObject target, string name, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            return variable != null;
        }
    }
}