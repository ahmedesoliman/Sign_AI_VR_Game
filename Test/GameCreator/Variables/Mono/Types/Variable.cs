namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Variable
    {
        private const bool SAVE_BY_DEFAULT = true;

        public enum VarType
        {
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        public enum DataType
        {
            Null,
            String,
            Number,
            Bool,
            Color,
            Vector2,
            Vector3,
            Texture2D,
            Sprite,
            GameObject
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public string name = "";
        public bool save = SAVE_BY_DEFAULT;
        public int type = (int)DataType.Null;
        public int tags = 0;

        [SerializeField] private VariableStr varStr = new VariableStr();
        [SerializeField] private VariableNum varNum = new VariableNum();
        [SerializeField] private VariableBol varBol = new VariableBol();
        [SerializeField] private VariableCol varCol = new VariableCol();
        [SerializeField] private VariableVc2 varVc2 = new VariableVc2();
        [SerializeField] private VariableVc3 varVc3 = new VariableVc3();
        [SerializeField] private VariableTxt varTxt = new VariableTxt();
        [SerializeField] private VariableSpr varSpr = new VariableSpr();
        [SerializeField] private VariableObj varObj = new VariableObj();

        // INITIALIZERS: --------------------------------------------------------------------------

        public Variable()
        {

        }

        public Variable(Variable variable)
        {
            this.name = variable.name;
            this.type = variable.type;
            this.save = variable.save;

            this.varStr = new VariableStr(variable.varStr.Get());
            this.varNum = new VariableNum(variable.varNum.Get());
            this.varBol = new VariableBol(variable.varBol.Get());
            this.varCol = new VariableCol(variable.varCol.Get());
            this.varVc2 = new VariableVc2(variable.varVc2.Get());
            this.varVc3 = new VariableVc3(variable.varVc3.Get());
            this.varTxt = new VariableTxt(variable.varTxt.Get());
            this.varSpr = new VariableSpr(variable.varSpr.Get());
            this.varObj = new VariableObj(variable.varObj.Get());
        }

        public Variable(string name, DataType type, object value, bool save = SAVE_BY_DEFAULT)
        {
            this.name = name;
            this.Set(type, value);
            this.save = save;
        }

        // GETTERS: -------------------------------------------------------------------------------

        public object Get()
        {
            switch ((DataType)this.type)
            {
                case DataType.String: return this.varStr.Get();
                case DataType.Number: return this.varNum.Get();
                case DataType.Bool: return this.varBol.Get();
                case DataType.Color: return this.varCol.Get();
                case DataType.Vector2: return this.varVc2.Get();
                case DataType.Vector3: return this.varVc3.Get();
                case DataType.Texture2D: return this.varTxt.Get();
                case DataType.Sprite: return this.varSpr.Get();
                case DataType.GameObject: return this.varObj.Get();
            }

            return null;
        }

        public T Get<T>()
        {
            return (T)this.Get();
        }

        public bool CanSave()
        {
            switch ((DataType)this.type)
            {
                case DataType.String: return this.varStr.CanSave();
                case DataType.Number: return this.varNum.CanSave();
                case DataType.Bool: return this.varBol.CanSave();
                case DataType.Color: return this.varCol.CanSave();
                case DataType.Vector2: return this.varVc2.CanSave();
                case DataType.Vector3: return this.varVc3.CanSave();
                case DataType.Texture2D: return this.varTxt.CanSave();
                case DataType.Sprite: return this.varSpr.CanSave();
                case DataType.GameObject: return this.varObj.CanSave();
            }

            return false;
        }

        public static bool CanSave(DataType type)
        {
            return VariableBase.CanSaveType(type);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public void Set(DataType type, object value)
        {
            this.type = (int)type;
            this.Update(value);
        }

        public void Update(object value)
        {
            switch ((DataType)this.type)
            {
                case DataType.String: this.varStr.Set((string)value); break;
                case DataType.Number: this.varNum.Set(Convert.ToSingle(value)); break;
                case DataType.Bool: this.varBol.Set((bool)value); break;
                case DataType.Color: this.varCol.Set((Color)value); break;
                case DataType.Vector2: this.varVc2.Set((Vector2)value); break;
                case DataType.Vector3: this.varVc3.Set((Vector3)value); break;
                case DataType.Texture2D: this.varTxt.Set((Texture2D)value); break;
                case DataType.Sprite: this.varSpr.Set((Sprite)value); break;
                case DataType.GameObject: this.varObj.Set((GameObject)value); break;
            }
        }
    }
}