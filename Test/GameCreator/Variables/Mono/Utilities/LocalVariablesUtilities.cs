namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class LocalVariablesUtilities
    {
        private const string SLASH = "/";
        private static readonly char[] SLASH_CHAR = new char[1] { '/' };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static Variable Get(GameObject target, string name, bool inChildren)
        {
            if (target == null) return null;
            if (string.IsNullOrEmpty(name)) return null;

            if (name.Contains(SLASH))
            {
                string[] path = name.Split(SLASH_CHAR);
                int index = 0;

                Variable variable = GetSingle(target, path[index], inChildren);
                index += 1;

                while (variable != null && index < path.Length)
                {
                    if (variable.Get() is GameObject)
                    {
                        variable = GetSingle(variable.Get<GameObject>(), path[index], inChildren);
                    }

                    index++;
                }

                return variable;
            }

            return GetSingle(target, name, inChildren);
        }

        private static Variable GetSingle(GameObject target, string name, bool inChildren)
        {
            if (target == null) return null;
            if (string.IsNullOrEmpty(name)) return null;

            LocalVariables[] localVariables = GatherLocals(target, inChildren);
            int localsLength = localVariables.Length;

            for (int i = 0; i < localsLength; ++i)
            {
                Variable variable = localVariables[i].Get(name);
                if (variable != null) return variable;
            }

            return null;
        }


        public static LocalVariables[] GatherLocals(GameObject target, bool inChildren)
        {
            if (!inChildren) return target.GetComponents<LocalVariables>();
            else return target.GetComponentsInChildren<LocalVariables>();
        }
    }
}