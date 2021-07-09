namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [DisallowMultipleComponent]
    [AddComponentMenu("Game Creator/List Variables")]
    public class ListVariables : LocalVariables
    {
        public enum Position
        {
            Index,
            First,
            Last,
            Previous,
            Current,
            Next,
            Random,
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool save = true;
        public Variable.DataType type = Variable.DataType.GameObject;

        public List<Variable> variables;
        public int iterator { get; private set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Start()
        {
            if (!Application.isPlaying) return;

            this.Initialize(true);
            if (save) SaveLoadManager.Instance.Initialize(this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(int index)
        {
            this.Initialize();
            if (index < 0 || index >= this.variables.Count) return null;
            
            this.iterator = index;
            return this.variables[index];
        }

        public Variable Get(Position position, int index = 0)
        {
            this.Initialize();
            switch (position)
            {
                case Position.Index: return this.Get(index);
                case Position.First: return this.Get(0);
                case Position.Last: return this.Get(this.variables.Count - 1);
                case Position.Current: return this.Get(this.iterator);
                case Position.Next:
                    this.NextIterator();
                    return this.Get(this.iterator);

                case Position.Previous:
                    this.PrevIterator();
                    return this.Get(this.iterator);

                case Position.Random:
                    int randomIndex = UnityEngine.Random.Range(0, this.variables.Count);
                    return this.Get(randomIndex);
            }

            return null;
        }

        public void Push(object value, int index)
        {
            this.Initialize();
            Variable variable = new Variable(
                Guid.NewGuid().ToString("N"),
                this.type,
                value,
                this.save
            );

            index = Math.Max(index, 0);
            index = Math.Min(index, this.variables.Count);
            this.variables.Insert(index, variable);

            VariablesManager.events.OnListAdd(gameObject, index, value);
        }

        public void Push(object value, Position position = Position.Last, int index = 0)
        {
            this.Initialize();
            switch (position)
            {
                case Position.Index:
                    this.Push(value, index);
                    break;

                case Position.First: 
                    this.Push(value, 0); 
                    break;

                case Position.Last: 
                    this.Push(value, this.variables.Count); 
                    break;

                case Position.Current: 
                    this.Push(value, this.iterator);
                    break;

                case Position.Next:
                    this.Push(value, this.iterator + 1);
                    break;

                case Position.Previous:
                    this.Push(value, this.iterator - 1);
                    break;

                case Position.Random:
                    int random = UnityEngine.Random.Range(0, this.variables.Count);
                    this.Push(value, random);
                    break;
            }
        }

        public void Remove(int index)
        {
            this.Initialize();
            if (index < 0 || index >= this.variables.Count) return;

            object value = this.variables[index].Get();
            this.variables.RemoveAt(index);

            VariablesManager.events.OnListRemove(gameObject, this.iterator, value);
        }

        public void Remove(Position position = Position.First, int index = 0)
        {
            this.Initialize();
            switch (position)
            {
                case Position.Index:
                    this.Remove(index);
                    break;

                case Position.First:
                    this.Remove(0);
                    break;

                case Position.Last:
                    this.Remove(this.variables.Count - 1);
                    break;

                case Position.Current:
                    this.Remove(this.iterator);
                    break;

                case Position.Next:
                    this.Remove(this.iterator + 1);
                    break;

                case Position.Previous:
                    this.Remove(this.iterator - 1);
                    break;

                case Position.Random:
                    int random = UnityEngine.Random.Range(0, this.variables.Count);
                    this.Remove(random);
                    break;
            }
        }

        public void SetInterator(int value)
        {
            this.iterator = Mathf.Clamp(
                value,
                0, this.variables.Count - 1
            );
        }

        public void NextIterator()
        {
            this.iterator += 1;
            if (this.iterator >= this.variables.Count) this.iterator = 0;
        }

        public void PrevIterator()
        {
            this.iterator -= 1;
            if (this.iterator < 0) this.iterator = this.variables.Count - 1;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected override void RequireInit(bool force = false)
        {
            if (this.initalized && !force) return;
            this.initalized = true;

            this.variables = new List<Variable>();
            for (int i = 0; i < this.references.Length; ++i)
            {
                Variable variable = this.references[i].variable;
                if (variable == null) continue;

                this.variables.Add(new Variable(variable));
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override string GetUniqueName()
        {
            string uniqueName = string.Format(
                "variables:list:{0}",
                this.GetID()
            );

            return uniqueName;
        }

        public override object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();
            if (this.variables == null || this.variables.Count == 0)
            {
                return container;
            }

            foreach (Variable item in this.variables)
            {
                if (this.save && item != null && item.CanSave())
                {
                    container.variables.Add(item);
                }
            }

            return container;
        }

        public override void ResetData()
        {
            this.RequireInit(true);
        }

        public override void OnLoad(object generic)
        {
            if (generic == null) return;
            if (!this.save) return;

            DatabaseVariables.Container container = (DatabaseVariables.Container)generic;
            while (this.variables.Count > 0) this.Remove(Position.First);

            for (int i = 0; i < container.variables.Count; ++i)
            {
                this.Push(container.variables[i].Get(), i);
            }
        }
    }
}