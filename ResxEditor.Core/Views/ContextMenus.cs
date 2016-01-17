﻿using System;
using System.Linq;
using Gdk;
using Gtk;
using ResxEditor.Core.Interfaces;

namespace ResxEditor.Core.Views
{
	public class RowActionMenuItem : MenuItem
	{
		public RowActionMenuItem(string label, System.Action rowAction) : base(label)
		{
			ButtonReleaseEvent += (o, args) => { rowAction.Invoke(); };
		}
	}

	public class CopyCellMenuItem : MenuItem
	{
		Func<TreePath, string> GetValueFromRow { get; }
		TreePath[] SelectedRows { get; }
		EventButton EventButton { get; }

		public CopyCellMenuItem (TreePath[] selectedRows, EventButton eventButton, string label, Func<TreePath, string> getValue) : base (label)
		{
			if (selectedRows.Length == 0) {
				throw new IndexOutOfRangeException ("Missing selected resource rows");
			}

			SelectedRows = selectedRows;
			EventButton = eventButton;
			GetValueFromRow = getValue;

			ButtonReleaseEvent += (o, e) => OnCopy ();
		}

		void OnCopy () {
			if (SelectedRows.Length > 1) {
				Console.WriteLine ("Multiple rows selected. Currently not supported: defaulting to first.");
			}

			var selectedPath = SelectedRows.First ();

			Clipboard clipboard = GetClipboard (Gdk.Selection.Clipboard);
			clipboard.Text = GetValueFromRow.Invoke (selectedPath);
		}
	}

	public class CellContextMenu : Menu
	{
		public CellContextMenu (IResourceController resourceController, IResourceListStore storeController, TreePath[] selectedRows, EventButton eventButton)
		{
			#region ArrangeGUI
			Append (new RowActionMenuItem("Add New Row", resourceController.AddNewResource));
			Append (new RowActionMenuItem("Remove Current Row", resourceController.RemoveCurrentResource));

			Append (new SeparatorMenuItem());

			Append (new CopyCellMenuItem (selectedRows, eventButton, "Copy Name", storeController.GetName));
			Append (new CopyCellMenuItem (selectedRows, eventButton, "Copy Value", storeController.GetValue));
			Append (new CopyCellMenuItem (selectedRows, eventButton, "Copy Comment", storeController.GetComment));

			ShowAll ();
			#endregion
		}
	}

	public class NoCellContextMenu : Menu
	{
		public NoCellContextMenu (IResourceController resourceController, EventButton eventButton)
		{
			#region ArrangeGUI
			Append (new RowActionMenuItem("Add New Row", resourceController.AddNewResource));

			ShowAll ();
			#endregion
		}
	}
}

