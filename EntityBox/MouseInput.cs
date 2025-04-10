// Mouse stuff

using System.Drawing;
using System.Collections.Generic;

namespace System.Windows.Forms
{
	public partial class EntityBox : Control
	{
		PointF? LastVia;         // The last vias when drawing with Shift

		public Point GetLastRightMouseButton()
		{
			return LastRMB;
		}

		public float GetDragDistance()
		{
			return draggingDist;
		}

		private Point SnapMousePosToGrid (int MouseX, int MouseY)
		{
			Point p = new Point();

			p.X = MouseX;
			p.Y = MouseY;

			if (snapToGrid)
			{
				var lp = ScreenToLambda(p.X, p.Y);
				lp.X = (float)Math.Round(lp.X / gridSize) * gridSize;
				lp.Y = (float)Math.Round(lp.Y / gridSize) * gridSize;
				p = LambdaToScreen(lp.X, lp.Y);
			}

			return p;
		}

		/// <summary>
		/// All input/output/inOut vias within a cell/block become ports
		/// </summary>
		public static List<Entity> GetCellPorts(Entity cell, List<Entity> ents)
		{
			List<Entity> ports = new List<Entity>();

			foreach (var ent in ents)
			{
				if (ent.IsPort())
				{
					if (cell.PathPoints != null && cell.PathPoints.Count != 0)
					{
						PointF[] poly = new PointF[cell.PathPoints.Count];
						PointF point = new PointF(ent.LambdaX, ent.LambdaY);

						int idx = 0;
						foreach (PointF pathPoint in cell.PathPoints)
						{
							poly[idx++] = pathPoint;
						}

						if (PointInPoly(poly, point) && ent.IsVisible())
						{
							ports.Add(ent);
						}
					}
					else
					{
						RectangleF rect = new RectangleF(cell.LambdaX, cell.LambdaY, cell.LambdaWidth, cell.LambdaHeight);
						if (rect.Contains(ent.LambdaX, ent.LambdaY) && ent.IsVisible())
						{
							ports.Add(ent);
						}
					}
				}
			}

			return ports;
		}

		//
		// Mouse hit test
		//

		private Entity EntityHitTest(int MouseX, int MouseY)
		{
			PointF point = new Point(MouseX, MouseY);
			PointF[] rect = new PointF[4];
			float zf = (float)Zoom / 100.0F;

			List<Entity> reversed = SortEntitiesReverse();

			foreach (Entity entity in reversed)
			{
				if (!entity.Visible)
					continue;

				// Check that all parents are visible

				Entity parent = entity.parent;
				bool parentsVisible = true;

				while (parent != null)
				{
					if (!parent.Visible)
					{
						parentsVisible = false;
						break;
					}

					parent = parent.parent;
				}
				if (!parentsVisible)
					continue;

				if (entity.IsWire() && HideWires == false)
				{
					PointF start = LambdaToScreen(entity.LambdaX, entity.LambdaY);
					PointF end = LambdaToScreen(entity.LambdaEndX, entity.LambdaEndY);

					if (end.X < start.X)
					{
						PointF temp = start;
						start = end;
						end = temp;
					}

					PointF ortho = new PointF(end.X - start.X, end.Y - start.Y);

					float len = (float)Math.Sqrt(Math.Pow(ortho.X, 2) +
												  Math.Pow(ortho.Y, 2));
					len = Math.Max(1.0F, len);

					var WireSize = entity.WidthOverride != 0 ? entity.WidthOverride : WireBaseSize;

					PointF rot = RotatePoint(ortho, -90);
					PointF normalized = new PointF(rot.X / len, rot.Y / len);
					PointF baseVect = new PointF(normalized.X * ((WireSize * zf) / 2),
												  normalized.Y * ((WireSize * zf) / 2));

					rect[0].X = baseVect.X + start.X;
					rect[0].Y = baseVect.Y + start.Y;
					rect[3].X = baseVect.X + end.X;
					rect[3].Y = baseVect.Y + end.Y;

					rot = RotatePoint(ortho, +90);
					normalized = new PointF(rot.X / len, rot.Y / len);
					baseVect = new PointF(normalized.X * ((WireSize * zf) / 2),
										   normalized.Y * ((WireSize * zf) / 2));

					rect[1].X = baseVect.X + start.X;
					rect[1].Y = baseVect.Y + start.Y;
					rect[2].X = baseVect.X + end.X;
					rect[2].Y = baseVect.Y + end.Y;

					if (PointInPoly(rect, point) == true)
						return entity;
				}
				else if (entity.IsCell() && HideCells == false)
				{
					if (entity.PathPoints != null && entity.PathPoints.Count != 0)
					{
						PointF[] poly = new PointF[entity.PathPoints.Count];

						int idx = 0;
						foreach (PointF pathPoint in entity.PathPoints)
						{
							poly[idx++] = (PointF)LambdaToScreen(pathPoint.X, pathPoint.Y);
						}

						if (PointInPoly(poly, point) == true)
							return entity;
					}
					else
					{
						rect[0] = LambdaToScreen(entity.LambdaX, entity.LambdaY);
						rect[1] = LambdaToScreen(entity.LambdaX, entity.LambdaY + entity.LambdaHeight);
						rect[2] = LambdaToScreen(entity.LambdaX + entity.LambdaWidth, entity.LambdaY + entity.LambdaHeight);
						rect[3] = LambdaToScreen(entity.LambdaX + entity.LambdaWidth, entity.LambdaY);

						if (PointInPoly(rect, point) == true)
							return entity;
					}
				}
				else if (entity.Type == EntityType.Beacon)
				{
					Point beaconOrigin = LambdaToScreen(entity.LambdaX, entity.LambdaY);

					Point imageOrigin = new Point(beaconOrigin.X - (int)((float)(beaconImage.Width * zf) / 2),
													beaconOrigin.Y - (int)((float)beaconImage.Height * zf));

					rect[0] = new PointF(imageOrigin.X, imageOrigin.Y);
					rect[1] = new PointF(imageOrigin.X, imageOrigin.Y + beaconImage.Height * zf);
					rect[2] = new PointF(imageOrigin.X + beaconImage.Width * zf, imageOrigin.Y + beaconImage.Height * zf);
					rect[3] = new PointF(imageOrigin.X + beaconImage.Width * zf, imageOrigin.Y);

					if (PointInPoly(rect, point) == true)
						return entity;
				}
				else if (entity.IsVias() && HideVias == false)
				{
					var ViasSize = entity.WidthOverride != 0 ? entity.WidthOverride : ViasBaseSize;

					rect[0] = LambdaToScreen(entity.LambdaX, entity.LambdaY);
					rect[0].X -= ((float)ViasSize * zf);
					rect[0].Y -= ((float)ViasSize * zf);

					rect[1] = LambdaToScreen(entity.LambdaX, entity.LambdaY);
					rect[1].X += ((float)ViasSize * zf);
					rect[1].Y -= ((float)ViasSize * zf);

					rect[2] = LambdaToScreen(entity.LambdaX, entity.LambdaY);
					rect[2].X += ((float)ViasSize * zf);
					rect[2].Y += ((float)ViasSize * zf);

					rect[3] = LambdaToScreen(entity.LambdaX, entity.LambdaY);
					rect[3].X -= ((float)ViasSize * zf);
					rect[3].Y += ((float)ViasSize * zf);

					if (PointInPoly(rect, point) == true)
						return entity;
				}
				else if (entity.IsRegion() && HideRegions == false)
				{
					PointF[] poly = new PointF[entity.PathPoints.Count];

					int idx = 0;
					foreach (PointF pathPoint in entity.PathPoints)
					{
						poly[idx++] = (PointF)LambdaToScreen(pathPoint.X, pathPoint.Y);
					}

					if (PointInPoly(poly, point) == true)
						return entity;
				}
			}

			return null;
		}


		//
		// Mouse events handling
		//

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// Scrolling

			if (e.Button == MouseButtons.Right && ScrollingBegin == false && DrawingBegin == false)
			{
				SavedMouseX = e.X;
				SavedMouseY = e.Y;
				SavedScrollX = _ScrollX;
				SavedScrollY = _ScrollY;
				ScrollingBegin = true;
			}

			// Drawing

			if (e.Button == MouseButtons.Left && Mode != EntityMode.Selection &&
				 DrawingBegin == false && ScrollingBegin == false)
			{
				Entity entity;
				bool Okay;

				//
				// Cannot draw cells / custom blocks over other entites
				//

				Okay = true;

				entity = EntityHitTest(e.X, e.Y);
				if (entity != null && (Mode == EntityMode.CellAdder ||
					 Mode == EntityMode.CellBuffer || Mode == EntityMode.CellBusSupp ||
					 Mode == EntityMode.CellFlipFlop || Mode == EntityMode.CellLatch ||
					 Mode == EntityMode.CellLogic || Mode == EntityMode.CellMux ||
					 Mode == EntityMode.CellNot || Mode == EntityMode.CellOther || Mode == EntityMode.UnitCustom ||
					 Mode == EntityMode.UnitMemory || Mode == EntityMode.UnitRegfile))
				{
					Okay = false;
				}

				if (Okay == true)
				{
					var p = SnapMousePosToGrid(e.X, e.Y);
					SavedMouseX = p.X;
					SavedMouseY = p.Y;
					SavedScrollX = _ScrollX;
					SavedScrollY = _ScrollY;
					DrawingBegin = true;
				}
			}

			// Dragging / selection

			if (e.Button == MouseButtons.Left && Mode == EntityMode.Selection
				 && DraggingBegin == false && SelectionBegin == false)
			{
				selected = GetSelected();

				if (selected.Count > 0)
				{
					foreach (Entity entity in selected)
					{
						if (entity.PathPoints != null && entity.PathPoints.Count != 0)
						{
							entity.SavedPathPoints = new List<PointF>(entity.PathPoints);
						}

						entity.SavedLambdaX = entity.LambdaX;
						entity.SavedLambdaY = entity.LambdaY;

						entity.SavedLambdaEndX = entity.LambdaEndX;
						entity.SavedLambdaEndY = entity.LambdaEndY;
					}

					DragStartMouseX = e.X;
					DragStartMouseY = e.Y;
					DraggingBegin = true;
				}
				else
				{
					SelectStartMouseX = e.X;
					SelectStartMouseY = e.Y;
					SelectionBegin = true;
				}
			}

			base.OnMouseDown(e);
		}

		private bool AnyParentInvisible (Entity entity)
		{
			while (entity.parent != null)
			{
				if (!entity.Visible)
					return true;
				entity = entity.parent;
			}
			return false;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Focus();

			long timeStampNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			if ((timeStampNow - UnserializeLastStamp) < 500)
				return;

			if (e.Button == MouseButtons.Right)
			{
				LastRMB.X = e.X;
				LastRMB.Y = e.Y;
			}
			else
			{
				LastRMB.X = -1;
				LastRMB.Y = -1;
			}

			if (e.Button == MouseButtons.Right && ScrollingBegin)
			{
				ScrollingBegin = false;
				Invalidate();
			}

			// Select entity

			if (e.Button == MouseButtons.Left && Mode == EntityMode.Selection)
			{
				// Catch entities overlapping selection box

				bool CatchSomething = false;

				if (SelectionBegin)
				{
					// Selection box area

					PointF selectionStart = ScreenToLambda(SelectStartMouseX, SelectStartMouseY);
					PointF selectionEnd = ScreenToLambda(e.X, e.Y);

					PointF selectionOrigin = new PointF();
					PointF selectionSize = new PointF();

					selectionSize.X = Math.Abs(selectionEnd.X - selectionStart.X);
					selectionSize.Y = Math.Abs(selectionEnd.Y - selectionStart.Y);

					if (selectionEnd.X > selectionStart.X)
					{
						if (selectionEnd.Y > selectionStart.Y)
						{
							selectionOrigin.X = selectionStart.X;
							selectionOrigin.Y = selectionStart.Y;
						}
						else
						{
							selectionOrigin.X = selectionStart.X;
							selectionOrigin.Y = selectionEnd.Y;
						}
					}
					else
					{
						if (selectionEnd.Y > selectionStart.Y)
						{
							selectionOrigin.X = selectionEnd.X;
							selectionOrigin.Y = selectionStart.Y;
						}
						else
						{
							selectionOrigin.X = selectionEnd.X;
							selectionOrigin.Y = selectionEnd.Y;
						}
					}

					RectangleF rect = new RectangleF(selectionOrigin.X,
													  selectionOrigin.Y,
													  selectionSize.X,
													  selectionSize.Y);

					// Estimate area. Doesn't count selection below 4 lambda square

					float square = selectionSize.X * selectionSize.Y;

					if (square >= 4.0F)
					{
						foreach (Entity ent in GetEntities())
						{
							if (ent.Selected || AnyParentInvisible(ent))
								continue;

							if (ent.IsCell())
							{
								if (ent.PathPoints != null && ent.PathPoints.Count != 0)
								{
									foreach (PointF point in ent.PathPoints)
									{
										if (rect.Contains(point))
										{
											SelectEntity(ent);
											CatchSomething = true;
										}
									}
								}
								else
								{
									RectangleF rect2 = new RectangleF(ent.LambdaX, ent.LambdaY,
																	   ent.LambdaWidth, ent.LambdaHeight);

									if (rect.IntersectsWith(rect2))
									{
										SelectEntity(ent);
										CatchSomething = true;
									}
								}
							}
							else if (ent.IsWire())
							{
								PointF point1 = new PointF(ent.LambdaX, ent.LambdaY);
								PointF point2 = new PointF(ent.LambdaEndX, ent.LambdaEndY);

								if (LineIntersectsRect(point1, point2, rect))
								{
									SelectEntity(ent);
									CatchSomething = true;
								}
							}
							else if (ent.IsVias())
							{
								PointF point1 = new PointF(ent.LambdaX, ent.LambdaY);

								if (rect.Contains(point1))
								{
									SelectEntity(ent);
									CatchSomething = true;
								}
							}
							else if (ent.IsRegion())
							{
								foreach (PointF point in ent.PathPoints)
								{
									if (rect.Contains(point))
									{
										SelectEntity(ent);
										CatchSomething = true;
									}
								}
							}
						}

						if (CatchSomething == true)
							Invalidate();
					}
				}

				Entity entity = EntityHitTest(e.X, e.Y);

				if (entity != null && CatchSomething == false)
				{
					if (entity.Selected == true && draggingDist < 1F)
					{
						if (selectCellWithPorts && entity.IsCell())
						{
							List<Entity> ents = GetEntities();
							List<Entity> ports = GetCellPorts(entity, ents);
							foreach (var port in ports)
							{
								port.Selected = false;
							}
						}

						entity.Selected = false;
						Invalidate();

						if (entityGrid != null)
							entityGrid.SelectedObject = null;
					}
					else
					{
						if (selectCellWithPorts && entity.IsCell())
						{
							List<Entity> ents = GetEntities();
							List<Entity> ports = GetCellPorts(entity, ents);
							foreach (var port in ports)
							{
								port.Selected = true;
							}
						}

						SelectEntity(entity);
						if (entity.IsWire() && wireSelectionAutoTraverse)
						{
							TraversalSelection(1);
						}
						if (entity.IsVias())
						{
							LastVia = new PointF(entity.LambdaX, entity.LambdaY);
						}
						Invalidate();

						if (entityGrid != null)
							entityGrid.SelectedObject = entity;
					}
				}
				else
				{
					if (draggingDist < 1F && CatchSomething == false)
						RemoveSelection();
				}
			}

			// Add vias

			if (e.Button == MouseButtons.Left &&
				 (Mode == EntityMode.ViasConnect || Mode == EntityMode.ViasFloating || Mode == EntityMode.ViasGround ||
				  Mode == EntityMode.ViasInout || Mode == EntityMode.ViasInput || Mode == EntityMode.ViasOutput ||
				  Mode == EntityMode.ViasPower) && DrawingBegin)
			{
				if (Control.ModifierKeys == Keys.Shift)
				{
					if (LastVia == null)
					{
						var via = AddVias((EntityType)Mode, e.X, e.Y, Color.Black);
						if (via != null)
							LastVia = new PointF(via.LambdaX, via.LambdaY);
					}
					else
					{
						Point start = LambdaToScreen(((PointF)LastVia).X, ((PointF)LastVia).Y);
						AddWire(EntityType.WireInterconnect, start.X, start.Y, e.X, e.Y);
						LastVia = ScreenToLambda(e.X, e.Y);
					}
				}
				else
				{
					var via = AddVias((EntityType)Mode, e.X, e.Y, Color.Black);
					if (via != null)
						LastVia = new PointF(via.LambdaX, via.LambdaY);
				}

				DrawingBegin = false;
			}

			// Add beacon

			if (e.Button == MouseButtons.Left &&
				Mode == EntityMode.Beacon && DrawingBegin)
			{
				AddBeacon(e.X, e.Y);

				DrawingBegin = false;
			}

			// Add wire

			if (e.Button == MouseButtons.Left && (Mode == EntityMode.WireGround ||
				  Mode == EntityMode.WireInterconnect || Mode == EntityMode.WirePower) && DrawingBegin)
			{
				var p = SnapMousePosToGrid(e.X, e.Y);
				AddWire((EntityType)Mode, SavedMouseX, SavedMouseY, p.X, p.Y);

				DrawingBegin = false;
			}

			// Add cell

			if (e.Button == MouseButtons.Left && (
					Mode == EntityMode.CellNot ||
					Mode == EntityMode.CellBuffer ||
					Mode == EntityMode.CellMux ||
					Mode == EntityMode.CellLogic ||
					Mode == EntityMode.CellAdder ||
					Mode == EntityMode.CellBusSupp ||
					Mode == EntityMode.CellFlipFlop ||
					Mode == EntityMode.CellLatch ||
					Mode == EntityMode.CellOther ||
					Mode == EntityMode.UnitRegfile ||
					Mode == EntityMode.UnitMemory ||
					Mode == EntityMode.UnitCustom
					) && DrawingBegin)
			{
				var p = SnapMousePosToGrid(e.X, e.Y);
				AddCell((EntityType)Mode, SavedMouseX, SavedMouseY, p.X, p.Y);

				DrawingBegin = false;
			}

			// End Drag

			if (e.Button == MouseButtons.Left && DraggingBegin)
			{
				// Fix move jitter

				if (draggingDist < 2 * Lambda)
				{
					foreach (Entity entity in selected)
					{
						entity.LambdaX = entity.SavedLambdaX;
						entity.LambdaY = entity.SavedLambdaY;
						entity.LambdaEndX = entity.SavedLambdaEndX;
						entity.LambdaEndY = entity.SavedLambdaEndY;

						if (OnEntityScroll != null)
							OnEntityScroll(this, entity, EventArgs.Empty);
					}
				}

				selected.Clear();
				draggingDist = 0.0F;
				DraggingBegin = false;
			}

			// Clear selection box

			if (SelectionBegin)
			{
				if (OnSelectionBox != null)
				{
					PointF selectionStart = ScreenToLambda(SelectStartMouseX, SelectStartMouseY);
					PointF selectionEnd = ScreenToLambda(e.X, e.Y);
					float lx = Math.Min(selectionStart.X, selectionEnd.X);
					float ly = Math.Min(selectionStart.Y, selectionEnd.Y);
					float w = Math.Abs (selectionEnd.X - selectionStart.X);
					float h = Math.Abs(selectionEnd.Y - selectionStart.Y);
					OnSelectionBox(this, new PointF(lx, ly), new PointF(w, h), EventArgs.Empty);
				}
				SelectionBegin = false;
				Invalidate();
			}

			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			// Scroll animation

			Point screenCoord = new Point();

			if (ScrollingBegin)
			{
				switch (Mode)
				{
					case EntityMode.Selection:
					default:
						screenCoord = LambdaToScreen(SavedScrollX, SavedScrollY);

						PointF lambdaCoord = ScreenToLambda(screenCoord.X + e.X - SavedMouseX,
														  screenCoord.Y + e.Y - SavedMouseY);

						UpdateScrollDirectionAngle(lambdaCoord);

						ScrollX = lambdaCoord.X;
						ScrollY = lambdaCoord.Y;
						break;
				}

				Invalidate();
			}

			// Wire drawing animation

			if (DrawingBegin && (Mode == EntityMode.WireGround ||
				   Mode == EntityMode.WireInterconnect || Mode == EntityMode.WirePower))
			{
				var p = SnapMousePosToGrid(e.X, e.Y);
				LastMouseX = p.X;
				LastMouseY = p.Y;
				Invalidate();
			}

			// Cell drawing animation

			if (DrawingBegin && (
					Mode == EntityMode.CellNot ||
					Mode == EntityMode.CellBuffer ||
					Mode == EntityMode.CellMux ||
					Mode == EntityMode.CellLogic ||
					Mode == EntityMode.CellAdder ||
					Mode == EntityMode.CellBusSupp ||
					Mode == EntityMode.CellFlipFlop ||
					Mode == EntityMode.CellLatch ||
					Mode == EntityMode.CellOther ||
					Mode == EntityMode.UnitRegfile ||
					Mode == EntityMode.UnitMemory ||
					Mode == EntityMode.UnitCustom))
			{
				var p = SnapMousePosToGrid(e.X, e.Y);
				LastMouseX = p.X;
				LastMouseY = p.Y;
				Invalidate();
			}

			// Drag animation

			if (DraggingBegin && selected.Count > 0)
			{
				foreach (Entity entity in selected)
				{
					if (entity.PathPoints != null && entity.PathPoints.Count != 0)
					{
						for (int i = 0; i < entity.PathPoints.Count; i++)
						{
							Point point = LambdaToScreen(entity.SavedPathPoints[i].X,
														  entity.SavedPathPoints[i].Y);

							point.X += e.X - DragStartMouseX;
							point.Y += e.Y - DragStartMouseY;

							entity.PathPoints[i] = ScreenToLambda(point.X, point.Y);
						}
					}
					else
					{
						Point point = LambdaToScreen(entity.SavedLambdaX, entity.SavedLambdaY);

						point.X += e.X - DragStartMouseX;
						point.Y += e.Y - DragStartMouseY;

						PointF lambda = ScreenToLambda(point.X, point.Y);

						entity.LambdaX = lambda.X;
						entity.LambdaY = lambda.Y;

						point = LambdaToScreen(entity.SavedLambdaEndX, entity.SavedLambdaEndY);

						point.X += e.X - DragStartMouseX;
						point.Y += e.Y - DragStartMouseY;

						lambda = ScreenToLambda(point.X, point.Y);

						entity.LambdaEndX = lambda.X;
						entity.LambdaEndY = lambda.Y;

						if (OnEntityScroll != null)
							OnEntityScroll(this, entity, EventArgs.Empty);
					}

					Point dist = new Point(Math.Abs(e.X - DragStartMouseX),
											 Math.Abs(e.Y - DragStartMouseY));

					draggingDist = (float)Math.Sqrt(Math.Pow(dist.X, 2) +
													 Math.Pow(dist.Y, 2));
				}

				Invalidate();
			}

			// Selection box animation

			if (SelectionBegin)
			{
				LastMouseX = e.X;
				LastMouseY = e.Y;
				Invalidate();
			}

			base.OnMouseMove(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			int delta;

			LastRMB.X = -1;
			LastRMB.Y = -1;

			if (e.Delta > 0)
				delta = +10;
			else
				delta = -10;

			switch (Mode)
			{
				case EntityMode.Selection:
				default:

					// Get old mouse pos in lambda space
					PointF oldMouse = ScreenToLambda(e.X, e.Y);

					// Teh zoom
					Zoom += delta;

					// Get new mouse pos in lambda
					PointF mousePos = ScreenToLambda(e.X, e.Y);

					// Adjust Scroll
					_ScrollX += mousePos.X - oldMouse.X;
					_ScrollY += mousePos.Y - oldMouse.Y;

					Invalidate();

					break;
			}

			base.OnMouseWheel(e);
		}

	}
}
