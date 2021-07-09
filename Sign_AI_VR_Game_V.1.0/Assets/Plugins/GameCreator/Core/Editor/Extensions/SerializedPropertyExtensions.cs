namespace GameCreator.Core
{
	using UnityEngine;
	using UnityEditor;

	public static class SerializedPropertyExtensions
	{
		private const string ERR_NOT_ARRAY = "SerializedProperty {0} is not an array.";
		private const string ERR_NEG_ARRAY = "SerializedProperty {0} can't have negative elements removed.";
		private const string ERR_OUT_BOUND = "SerializedProperty {0} can't remove {1} element as array has {2} elements.";
		private const string ERR_NUL_ARRAY = "Can't remove a null element";
		private const string ERR_NOT_FOUND = "Element {0} wasn't found in property {1}";

		public static void AddToObjectArray<T> (this SerializedProperty spArray, T element) where T : Object
		{
			if (!spArray.isArray) throw new UnityException(string.Format(ERR_NOT_ARRAY, spArray.name));

			spArray.InsertArrayElementAtIndex(spArray.arraySize);
			spArray.GetArrayElementAtIndex(spArray.arraySize - 1).objectReferenceValue = element;

			spArray.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			spArray.serializedObject.Update();
		}

		public static void RemoveFromObjectArrayAt (this SerializedProperty spArray, int index)
		{
			if(index < 0) throw new UnityException(string.Format(ERR_NEG_ARRAY, spArray.name));
			if (!spArray.isArray) throw new UnityException(string.Format(ERR_NOT_ARRAY, spArray.name));
			if(index > spArray.arraySize - 1) 
			{
				throw new UnityException(string.Format(ERR_OUT_BOUND, spArray.name, index, spArray.arraySize));
			}

			if (spArray.GetArrayElementAtIndex(index).objectReferenceValue) spArray.DeleteArrayElementAtIndex(index);
			spArray.DeleteArrayElementAtIndex(index);

			spArray.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			spArray.serializedObject.Update();
		}
	}
}