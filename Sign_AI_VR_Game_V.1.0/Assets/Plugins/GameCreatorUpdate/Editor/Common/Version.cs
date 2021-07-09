namespace GameCreator.Update
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[System.Serializable]
    public class Version
    {
        public static Version NONE
        {
            get
            {
                return new Version(0, 0, 0);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public int major;
        public int minor;
        public int patch;

        // INITIALIZERS: --------------------------------------------------------------------------

        public Version()
        {
            this.major = 0;
            this.minor = 0;
            this.patch = 0;
        }

        public Version(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public Version(string version)
        {
            string[] codes = version.Trim().Split('.');
            if (codes.Length != 3) return;
            this.major = int.Parse(codes[0]);
            this.minor = int.Parse(codes[1]);
            this.patch = int.Parse(codes[2]);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

		public bool HigherThan(Version other)
		{
            if (this.major > other.major) return true;
            if (this.major == other.major)
            {
                if (this.minor > other.minor) return true;
                if (this.minor == other.minor)
                {
                    if (this.patch > other.patch) return true;
                }
            }

            return false;
		}

        public bool LowerThan(Version other)
        {
            if (this.major < other.major) return true;
            if (this.major == other.major)
            {
                if (this.minor < other.minor) return true;
                if (this.minor == other.minor)
                {
                    if (this.patch < other.patch) return true;
                }
            }

            return false;
        }

        public bool Equals(Version other)
        {
            if (this.major != other.major) return false;
            if (this.minor != other.minor) return false;
            if (this.patch != other.patch) return false;
            return true;
        }

        public bool IsUndefined()
        {
            if (this.major != 0) return false;
            if (this.minor != 0) return false;
            if (this.patch != 0) return false;
            return true;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string ToString()
		{
            return string.Format(
                "{0}.{1}.{2}",
                this.major,
                this.minor,
                this.patch
            );
		}
	}
}