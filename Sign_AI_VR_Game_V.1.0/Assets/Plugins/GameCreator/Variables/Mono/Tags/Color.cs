namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class HexColor
    {
        public string hex;

        private bool init = false;
        private Color color;

        // INITIALIZERS: --------------------------------------------------------------------------

        public HexColor()
        {
            this.hex = "#ffffff";
            this.init = false;
            this.color = Color.white;
        }

        public HexColor(string hex)
        {
            this.hex = hex;
            this.init = false;
            this.color = Color.white;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Color GetColor()
        {
            if (!this.init)
            {
                this.init = true;
                ColorUtility.TryParseHtmlString(this.hex, out this.color);
            }

            return color;
        }
    }
}