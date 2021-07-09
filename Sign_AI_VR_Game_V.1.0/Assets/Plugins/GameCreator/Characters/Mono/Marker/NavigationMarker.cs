namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("Game Creator/Characters/Marker", 100)]
	public class NavigationMarker : MonoBehaviour 
	{
        // PROPERTIES: ----------------------------------------------------------------------------

        private const float GIZMO_MESH_SCALE = 0.5f;
		private static readonly Vector2[] GIZMO_OUTLINE_MESH = new Vector2[]
		{
			new Vector2(0,1), new Vector2(1,2), new Vector2(2,3), 
			new Vector2(3,4), new Vector2(4,5), new Vector2(5,6),
			new Vector2(6,0)
		};

        #if UNITY_EDITOR
        public static bool LABEL_SHOW = true;
        public const string LABEL_KEY = "gamecreator-marker-label";
        #endif

        private const float MESH_VERTICAL_OFFSET = 0.1f;
		private static readonly Color GIZMO_ARROW_COLOR = new Color(0f,0f,0f, 0.2f);
		private static Mesh GIZMO_MESH;

        public Color color = Color.yellow;
        public string label = "";
        [Range(0.0f, 5.0f)]
        public float stopThreshold = 0.0f;

        // GIZMOS METHODS: ------------------------------------------------------------------------

        #if UNITY_EDITOR
        private void OnEnable()
        {
            LABEL_SHOW = EditorPrefs.GetBool(LABEL_KEY, true);
        }
        #endif

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position + (Vector3.up * MESH_VERTICAL_OFFSET);
            Gizmos.color = GIZMO_ARROW_COLOR;
            Gizmos.DrawMesh(NavigationMarker.GetMarkerMesh(), position, transform.rotation, Vector3.one);

            Gizmos.color = new Color(this.color.r, this.color.g, this.color.b, 0.5f);
            Mesh mesh = NavigationMarker.GetMarkerMesh();
            for (int i = 0; i < GIZMO_OUTLINE_MESH.Length; ++i)
            {
                Gizmos.DrawLine(
                    position + (transform.rotation * mesh.vertices[(int)GIZMO_OUTLINE_MESH[i].x]),
                    position + (transform.rotation * mesh.vertices[(int)GIZMO_OUTLINE_MESH[i].y])
                );
            }

            #if UNITY_EDITOR
            if (LABEL_SHOW)
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = this.color;
                Handles.Label(transform.position, this.label, labelStyle);
            }

            if (this.stopThreshold > Mathf.Epsilon)
            {
                Handles.color = this.color;
                Handles.DrawWireDisc(position, Vector3.up, this.stopThreshold);
            }
            #endif
        }

		// MARKER MESH GENERATOR: -----------------------------------------------------------------

		private static Mesh GetMarkerMesh()
		{
			if (GIZMO_MESH == null) GIZMO_MESH = NavigationMarker.GenerateMarkerMesh();
			return GIZMO_MESH;
		}

		private static Mesh GenerateMarkerMesh()
		{
			Vector3[] vertices = new Vector3[7]
            {
                new Vector3( 0.0f, 0f,  1f) * GIZMO_MESH_SCALE,
                new Vector3(-1.0f, 0f,  0f) * GIZMO_MESH_SCALE,
                new Vector3(-0.5f, 0f,  0f) * GIZMO_MESH_SCALE,
                new Vector3(-0.5f, 0f, -1f) * GIZMO_MESH_SCALE,
                new Vector3( 0.5f, 0f, -1f) * GIZMO_MESH_SCALE,
                new Vector3( 0.5f, 0f,  0f) * GIZMO_MESH_SCALE,
                new Vector3( 1.0f, 0f,  0f) * GIZMO_MESH_SCALE
			};

			int[] triangles = new int[15]
			{
				0, 6, 5,
				0, 5, 4,
				0, 4, 3,
				0, 3, 2,
				0, 2, 1
			};

			Vector2[] uvs = new Vector2[7]
			{
				new Vector2(0.5f, 1.0f),
				new Vector2(1.0f, 0.5f),
				new Vector2(.25f, 0.5f),
				new Vector2(.25f, 0.0f),
				new Vector2(.75f, 0.0f),
				new Vector2(.75f, 0.5f),
				new Vector2(1.0f, 0.5f)
			};

			Vector3[] normals = new Vector3[7]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.uv = uvs;

			return mesh;
		}
	}
}