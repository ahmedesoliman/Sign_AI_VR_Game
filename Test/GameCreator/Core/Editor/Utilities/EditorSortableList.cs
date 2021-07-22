namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class EditorSortableList
	{
		public class SwapIndexes
		{
			public int src;
			public int dst;
		};

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private bool sortActions = false;
		private bool isDragging = false;

		private int actionDragDstIndex = -1;
		private int actionDragSrcIndex = -1;

		private Vector2 dragMousePosition = Vector2.zero;

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public bool CaptureSortEvents(Rect handleRect, int index)
		{
			bool forceRepaint = false;

			switch (UnityEngine.Event.current.type) 
			{
			case EventType.MouseDown:
				if (handleRect.Contains(UnityEngine.Event.current.mousePosition))
				{
					this.isDragging = true;
					this.actionDragDstIndex = index;
					this.actionDragSrcIndex = index;
					this.dragMousePosition = UnityEngine.Event.current.mousePosition;
					forceRepaint = true;
				}
				break;

			case EventType.MouseDrag:
				if (!this.isDragging) break;
				this.dragMousePosition = UnityEngine.Event.current.mousePosition;
				forceRepaint = true;
				break;

			case EventType.MouseUp:
				if (!this.isDragging) break;
				this.sortActions = true;
				this.isDragging = false;
				forceRepaint = true;
				break;
			}

			return forceRepaint;
		}

		public void PaintDropPoints(Rect rect, int index, int arraySize)
		{
			if (this.isDragging)
			{
				Rect upperRect = this.GetUpperDropRect(rect);
				if (upperRect.Contains(this.dragMousePosition))
				{
					if (this.actionDragSrcIndex < index) this.actionDragDstIndex = index - 1;
					else this.actionDragDstIndex = index;
					this.PaintDropMarker(upperRect);
				}

				if (index >= arraySize - 1)
				{
					Rect lowerRect = this.GetLowerDropRect(rect);
					if (lowerRect.Contains(this.dragMousePosition))
					{
						if (this.actionDragSrcIndex < index+1) this.actionDragDstIndex = index;
						else this.actionDragDstIndex = index+1;
						this.PaintDropMarker(lowerRect);
					}
				}
			}
		}

		public SwapIndexes GetSortIndexes()
		{
			SwapIndexes result = null;
			if (this.sortActions && this.actionDragSrcIndex >= 0 && this.actionDragDstIndex >= 0)
			{
				result = new SwapIndexes()
				{
					src = this.actionDragSrcIndex,
					dst = this.actionDragDstIndex
				};
			}

			this.sortActions = false;
			return result;
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private Rect GetUpperDropRect(Rect boundaries)
		{
			return new Rect(
				boundaries.x - 5f,
				boundaries.y - 9f,
				boundaries.width,
				18f
			);
		}

		private Rect GetLowerDropRect(Rect boundaries)
		{
			Rect upperRect = this.GetUpperDropRect(boundaries);
			return new Rect(
				upperRect.x,
				upperRect.y + boundaries.height,
				upperRect.width,
				upperRect.height
			);
		}

		private void PaintDropMarker(Rect rect)
		{
			GUI.BeginGroup(rect);
			GUI.BeginGroup(new Rect(5f,9f,rect.width,9f), CoreGUIStyles.GetDropMarker());
			GUI.EndGroup();
			GUI.EndGroup();
		}
	}
}