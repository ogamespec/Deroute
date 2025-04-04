﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DerouteSharp
{
	public partial class FormCells : Form
	{
		public bool Modifed = false;
		private List<CellSupport.Cell> cells_db = new List<CellSupport.Cell>();
		private CellSupport.Cell new_cell = new CellSupport.Cell();
		private ListViewItem selected_item = null;
		private EntityBox parent_box;

		public FormCells(EntityBox parent, List<CellSupport.Cell> cells)
		{
			InitializeComponent();
			cells_db = cells;
			parent_box = parent;
			entityBox1.Lambda = parent_box.Lambda;
		}

		private void FormCells_Load(object sender, EventArgs e)
		{
			entityBox1.AssociateSelectionPropertyGrid(propertyGrid1);
			DatabaseToControls();
		}

		#region "CRUD"

		public void CreateCell(Bitmap source_image, Point point, Size size)
		{
			if (source_image == null)
				return;

			new_cell = new CellSupport.Cell();
			new_cell.cell_image = new Bitmap(source_image);

			entityBox1.Image = new Bitmap(source_image);
			entityBox1.Invalidate();

			entityBox1.Mode = EntityMode.Selection;

			FormEnterValue enter_value = new FormEnterValue("Enter cell name");
			enter_value.FormClosed += Enter_value_FormClosed;
			enter_value.ShowDialog();
		}

		private void Enter_value_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormEnterValue enter_value = (FormEnterValue)sender;
			new_cell.Name = enter_value.StrValue;
			cells_db.Add(new_cell);

			Modifed = true;
			DatabaseToControls();
		}

		private void DatabaseToControls()
		{
			listView1.Clear();
			listView1.BeginUpdate();

			foreach (var cell in cells_db)
			{
				ListViewItem item = new ListViewItem(cell.Name);
				item.Tag = cell;
				listView1.Items.Add(item);
			}

			listView1.EndUpdate();
		}

		private void EntitiesToCell()
		{
			if (selected_item != null)
			{
				CellSupport.Cell cell = selected_item.Tag as CellSupport.Cell;
				cell.Entities = entityBox1.GetEntities();
			}
		}

		public List<CellSupport.Cell> GetCollection()
		{
			return cells_db;
		}

		private void DeleteCell (string name)
		{
			CellSupport.Cell cell_to_remove = null;

			foreach (var cell in cells_db)
			{
				if (cell.Name == name)
				{
					cell_to_remove = cell;
					break;
				}
			}

			if (cell_to_remove != null)
			{
				cells_db.Remove(cell_to_remove);
			}

			Modifed = true;
		}

		#endregion "CRUD"


		#region "Mode Selection"

		private void SelectionButtonHighlight()
		{
			toolStripDropDownButton1.BackColor = SystemColors.Control;
			toolStripDropDownButton2.BackColor = SystemColors.Control;
			toolStripDropDownButton3.BackColor = SystemColors.Control;
		}

		private void ViasButtonHighlight()
		{
			toolStripDropDownButton1.BackColor = SystemColors.ActiveCaption;
			toolStripDropDownButton2.BackColor = SystemColors.Control;
			toolStripDropDownButton3.BackColor = SystemColors.Control;
		}

		private void WiresButtonHighlight()
		{
			toolStripDropDownButton1.BackColor = SystemColors.Control;
			toolStripDropDownButton2.BackColor = SystemColors.ActiveCaption;
			toolStripDropDownButton3.BackColor = SystemColors.Control;
		}

		private void CellsButtonHighlight()
		{
			toolStripDropDownButton1.BackColor = SystemColors.Control;
			toolStripDropDownButton2.BackColor = SystemColors.Control;
			toolStripDropDownButton3.BackColor = SystemColors.ActiveCaption;
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasConnect;
			ViasButtonHighlight();
		}

		private void viasPowerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasPower;
			ViasButtonHighlight();
		}

		private void viasGroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasGround;
			ViasButtonHighlight();
		}

		private void viasInputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasInput;
			ViasButtonHighlight();
		}

		private void viasOutputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasOutput;
			ViasButtonHighlight();
		}

		private void viasInoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasInout;
			ViasButtonHighlight();
		}

		private void viasFloatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.ViasFloating;
			ViasButtonHighlight();
		}

		private void wireInterconnectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.WireInterconnect;
			WiresButtonHighlight();
		}

		private void wirePowerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.WirePower;
			WiresButtonHighlight();
		}

		private void wireGroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.WireGround;
			WiresButtonHighlight();
		}

		private void cellNotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellNot;
			CellsButtonHighlight();
		}

		private void cellBufferToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellBuffer;
			CellsButtonHighlight();
		}

		private void cellMuxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellMux;
			CellsButtonHighlight();
		}

		private void cellLogicToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellLogic;
			CellsButtonHighlight();
		}

		private void cellAdderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellAdder;
			CellsButtonHighlight();
		}

		private void cellBusSupportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellBusSupp;
			CellsButtonHighlight();
		}

		private void cellFlipFlopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellFlipFlop;
			CellsButtonHighlight();
		}

		private void cellLatchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellLatch;
			CellsButtonHighlight();
		}

		private void cellOtherToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.CellOther;
			CellsButtonHighlight();
		}

		private void unitRegisterFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.UnitRegfile;
			CellsButtonHighlight();
		}

		private void unitMemoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.UnitMemory;
			CellsButtonHighlight();
		}

		private void unitCustomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			entityBox1.Mode = EntityMode.UnitCustom;
			CellsButtonHighlight();
		}

		#endregion "Mode Selection"

		private void FormCells_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Escape)
			{
				entityBox1.Mode = EntityMode.Selection;
				SelectionButtonHighlight();
			}
			else if (e.KeyCode == Keys.F2)
			{
				entityBox1.Mode = EntityMode.ViasConnect;
				ViasButtonHighlight();
			}
			else if (e.KeyCode == Keys.F3)
			{
				entityBox1.Mode = EntityMode.WireInterconnect;
				WiresButtonHighlight();
			}
			else if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			EntitiesToCell();
			Modifed = true;
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				ListViewItem item = listView1.SelectedItems[0];
				selected_item = item;
				propertyGrid1.SelectedObject = item.Tag;
				bool abort_image_loading = false;

				CellSupport.Cell cell = item.Tag as CellSupport.Cell;
				entityBox1.LoadImage(new Bitmap(cell.cell_image), ref abort_image_loading);
				entityBox1.root.Children.AddRange(cell.Entities);
				entityBox1.Invalidate();
			}
			else
			{
				propertyGrid1.SelectedObject = null;

				entityBox1.UnloadImage();
				entityBox1.DeleteAllEntites();
				entityBox1.Invalidate();

				selected_item = null;
			}
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			if (selected_item != null)
			{
				CellSupport.Cell cell = selected_item.Tag as CellSupport.Cell;
				Console.WriteLine("Add {0} to EntityBox", cell.Name);
				parent_box.AddEntitiesByCrosshair(cell.Entities);
			}
		}

		private void listView1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && selected_item != null)
			{
				CellSupport.Cell cell = selected_item.Tag as CellSupport.Cell;
				Console.WriteLine("Delete {0}", cell.Name);
				listView1.Items.Remove(selected_item);
				DeleteCell(cell.Name);
			}
		}
	}
}
