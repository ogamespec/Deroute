// Entity Props

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public partial class EntityBox : Control
	{
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout
		{
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}

		[Category("Appearance"), DefaultValue(null)]
		[Browsable(false)]
		public Image BeaconImage
		{
			get { return beaconImage; }
			set
			{
				if (beaconImage != value)
				{
					if (beaconImage != null)
					{
						beaconImage.Dispose();
						GC.Collect();
					}

					beaconImage = new Bitmap(value);
					Invalidate();
				}
			}
		}

		[Category("Appearance"), DefaultValue(null)]
		[Browsable(false)]
		public Image Image
		{
			get { return _imageOrig; }
			set
			{
				if (_imageOrig != value)
				{
					if (_imageOrig != null)
					{
						_imageOrig.Dispose();
						GC.Collect();
					}

					_imageOrig = ToGrayscale(value);

					ScrollingBegin = false;
					Invalidate();
				}
			}
		}

		[Category("Appearance")]
		public int ImageOpacity
		{
			get { return _imageOpacity; }
			set
			{
				_imageOpacity = Math.Max(0, Math.Min(100, value));
				Invalidate();
			}
		}

		[Category("Logic")]
		public bool SelectEntitiesAfterAdd
		{
			get { return selectEntitiesAfterAdd; }
			set { selectEntitiesAfterAdd = value; }
		}

		[Category("Logic")]
		public bool WireSelectionAutoTraverse
		{
			get { return wireSelectionAutoTraverse; }
			set { wireSelectionAutoTraverse = value; }
		}

		[Category("Logic")]
		public bool Grayscale
		{
			get { return grayscale; }
			set { grayscale = value; }
		}

		[Category("Logic")]
		public bool OptimizeTilemap
		{
			get { return tilemap_image; }
			set { tilemap_image = value; }
		}

		[Category("Logic")]
		public float Lambda
		{
			get { return _lambda; }
			set
			{
				_lambda = value;
				Invalidate();
			}
		}

		[Category("Logic")]
		public EntityMode Mode
		{
			get { return drawMode; }
			set
			{
				drawMode = value;

				if (drawMode == EntityMode.Selection)
					DrawingBegin = false;
			}
		}

		[Category("Appearance")]
		public float ScrollX
		{
			get { return _ScrollX; }
			set
			{
				_ScrollX = value;
				Invalidate();

				if (OnScrollChanged != null)
					OnScrollChanged(this, EventArgs.Empty);
			}
		}

		[Category("Appearance")]
		public float ScrollY
		{
			get { return _ScrollY; }
			set
			{
				_ScrollY = value;
				Invalidate();

				if (OnScrollChanged != null)
					OnScrollChanged(this, EventArgs.Empty);
			}
		}

		[Category("Appearance")]
		public int Zoom
		{
			get { return _zoom; }
			set
			{
				int oldZoom = _zoom;

				if (value < 10)
					value = 10;

				if (value > 400)
					value = 400;

				_zoom = value;

				if (oldZoom != _zoom)
				{
					Invalidate();

					if (OnZoomChanged != null)
						OnZoomChanged(this, EventArgs.Empty);
				}
			}
		}

		[Category("Appearance")]
		public bool HideImage
		{
			get { return hideImage; }
			set { hideImage = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideVias
		{
			get { return hideVias; }
			set { hideVias = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideWires
		{
			get { return hideWires; }
			set { hideWires = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideCells
		{
			get { return hideCells; }
			set { hideCells = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideGrid
		{
			get { return hideGrid; }
			set { hideGrid = value; Invalidate(); }
		}

		[Category("Appearance")]
		public float GridSize
		{
			get { return gridSize; }
			set { gridSize = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool SnapToGrid
		{
			get { return snapToGrid; }
			set { snapToGrid = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideLambdaMetrics
		{
			get { return hideLambdaMetrics; }
			set { hideLambdaMetrics = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool HideRegions
		{
			get { return hideRegions; }
			set { hideRegions = value; Invalidate(); }
		}

		[Category("Appearance")]
		public Color SelectionBoxColor
		{
			get { return selectionBoxColor; }
			set { selectionBoxColor = value; Invalidate(); }
		}

		[Category("Appearance")]
		public bool SelectCellWithPorts
		{
			get { return selectCellWithPorts; }
			set { selectCellWithPorts = value; Invalidate(); }
		}

		//
		// Entity properties
		//

		private Color _ViasInputColor;
		private Color _ViasOutputColor;
		private Color _ViasInoutColor;
		private Color _ViasConnectColor;
		private Color _ViasFloatingColor;
		private Color _ViasPowerColor;
		private Color _ViasGroundColor;
		private Color _WireInterconnectColor;
		private Color _WirePowerColor;
		private Color _WireGroundColor;
		private Color _CellNotColor;
		private Color _CellBufferColor;
		private Color _CellMuxColor;
		private Color _CellLogicColor;
		private Color _CellAdderColor;
		private Color _CellBusSuppColor;
		private Color _CellFlipFlopColor;
		private Color _CellLatchColor;
		private Color _CellOtherColor;
		private Color _UnitRegfileColor;
		private Color _UnitMemoryColor;
		private Color _UnitCustomColor;
		private Color _SelectionColor;
		private Color _ViasOverrideColor;
		private Color _WireOverrideColor;
		private Color _CellOverrideColor;
		private Color _RegionOverrideColor;
		private Color _HighZColor;
		private Color _ZeroColor;
		private Color _OneColor;
		private ViasShape _viasShape;
		private int _viasBaseSize;
		private int _wireBaseSize;
		private TextAlignment _cellTextAlignment;
		private TextAlignment _viasTextAlignment;
		private TextAlignment _wireTextAlignment;
		private int _ViasOpacity;
		private int _WireOpacity;
		private int _CellOpacity;
		private int _RegionOpacity;
		private int _ViasPriority;
		private int _WirePriority;
		private int _CellPriority;
		private int _BeaconPriority;
		private int _RegionPriority;
		private bool _AutoPriority;
		private string _viasGroundText;
		private string _viasPowerText;

		private void DefaultEntityAppearance()
		{
			_viasShape = ViasShape.Round;
			_viasBaseSize = 4;
			_wireBaseSize = 5;
			_cellTextAlignment = TextAlignment.TopLeft;
			_viasTextAlignment = TextAlignment.Top;
			_wireTextAlignment = TextAlignment.TopLeft;

			_ViasInputColor = Color.Green;
			_ViasOutputColor = Color.Red;
			_ViasInoutColor = Color.Gold;
			_ViasConnectColor = Color.Black;
			_ViasFloatingColor = Color.Gray;
			_ViasPowerColor = Color.Tomato;
			_ViasGroundColor = Color.Lime;

			_WireInterconnectColor = Color.Blue;
			_WirePowerColor = Color.Red;
			_WireGroundColor = Color.Green;

			_CellNotColor = Color.Navy;
			_CellBufferColor = Color.SteelBlue;
			_CellMuxColor = Color.DarkOrange;
			_CellLogicColor = Color.Yellow;
			_CellAdderColor = Color.Red;
			_CellBusSuppColor = Color.DarkViolet;
			_CellFlipFlopColor = Color.Lime;
			_CellLatchColor = Color.Cyan;
			_CellOtherColor = Color.Snow;

			_UnitRegfileColor = Color.Snow;
			_UnitMemoryColor = Color.Snow;
			_UnitCustomColor = Color.Snow;

			_SelectionColor = Color.LimeGreen;

			_ViasOverrideColor = Color.Black;
			_WireOverrideColor = Color.Black;
			_CellOverrideColor = Color.Black;
			_RegionOverrideColor = Color.Black;

			_HighZColor = Color.Gold;
			_ZeroColor = Color.Green;
			_OneColor = Color.LawnGreen;

			_ViasOpacity = 255;
			_WireOpacity = 128;
			_CellOpacity = 128;
			_RegionOpacity = 128;

			_BeaconPriority = 4;
			_ViasPriority = 3;
			_WirePriority = 2;
			_CellPriority = 1;
			_RegionPriority = 0;
			_AutoPriority = true;

			_viasGroundText = "1'b0";
			_viasPowerText = "1'b1";
			selectCellWithPorts = true;
		}

		[Category("Entity Appearance")]
		public ViasShape ViasShape
		{
			get { return _viasShape; }
			set { _viasShape = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public int ViasBaseSize
		{
			get { return _viasBaseSize; }
			set { _viasBaseSize = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public int WireBaseSize
		{
			get { return _wireBaseSize; }
			set { _wireBaseSize = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public TextAlignment CellTextAlignment
		{
			get { return _cellTextAlignment; }
			set { _cellTextAlignment = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public TextAlignment WireTextAlignment
		{
			get { return _wireTextAlignment; }
			set { _wireTextAlignment = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public TextAlignment ViasTextAlignment
		{
			get { return _viasTextAlignment; }
			set { _viasTextAlignment = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasInputColor
		{
			get { return _ViasInputColor; }
			set { _ViasInputColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasOutputColor
		{
			get { return _ViasOutputColor; }
			set { _ViasOutputColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasInoutColor
		{
			get { return _ViasInoutColor; }
			set { _ViasInoutColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasConnectColor
		{
			get { return _ViasConnectColor; }
			set { _ViasConnectColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasFloatingColor
		{
			get { return _ViasFloatingColor; }
			set { _ViasFloatingColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasPowerColor
		{
			get { return _ViasPowerColor; }
			set { _ViasPowerColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasGroundColor
		{
			get { return _ViasGroundColor; }
			set { _ViasGroundColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color WireInterconnectColor
		{
			get { return _WireInterconnectColor; }
			set { _WireInterconnectColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color WirePowerColor
		{
			get { return _WirePowerColor; }
			set { _WirePowerColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color WireGroundColor
		{
			get { return _WireGroundColor; }
			set { _WireGroundColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellNotColor
		{
			get { return _CellNotColor; }
			set { _CellNotColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellBufferColor
		{
			get { return _CellBufferColor; }
			set { _CellBufferColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellMuxColor
		{
			get { return _CellMuxColor; }
			set { _CellMuxColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellLogicColor
		{
			get { return _CellLogicColor; }
			set { _CellLogicColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellAdderColor
		{
			get { return _CellAdderColor; }
			set { _CellAdderColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellBusSuppColor
		{
			get { return _CellBusSuppColor; }
			set { _CellBusSuppColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellFlipFlopColor
		{
			get { return _CellFlipFlopColor; }
			set { _CellFlipFlopColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellLatchColor
		{
			get { return _CellLatchColor; }
			set { _CellLatchColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellOtherColor
		{
			get { return _CellOtherColor; }
			set { _CellOtherColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color UnitRegfileColor
		{
			get { return _UnitRegfileColor; }
			set { _UnitRegfileColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color UnitMemoryColor
		{
			get { return _UnitMemoryColor; }
			set { _UnitMemoryColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color UnitCustomColor
		{
			get { return _UnitCustomColor; }
			set { _UnitCustomColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color SelectionColor
		{
			get { return _SelectionColor; }
			set { _SelectionColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ViasOverrideColor
		{
			get { return _ViasOverrideColor; }
			set { _ViasOverrideColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color WireOverrideColor
		{
			get { return _WireOverrideColor; }
			set { _WireOverrideColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color CellOverrideColor
		{
			get { return _CellOverrideColor; }
			set { _CellOverrideColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color RegionOverrideColor
		{
			get { return _RegionOverrideColor; }
			set { _RegionOverrideColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color HighZColor
		{
			get { return _HighZColor; }
			set { _HighZColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color ZeroColor
		{
			get { return _ZeroColor; }
			set { _ZeroColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public Color OneColor
		{
			get { return _OneColor; }
			set { _OneColor = value; Invalidate(); }
		}

		[Category("Entity Appearance")]
		public int ViasOpacity
		{
			get { return _ViasOpacity; }
			set
			{
				_ViasOpacity = Math.Max(0, Math.Min(255, value));
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int WireOpacity
		{
			get { return _WireOpacity; }
			set
			{
				_WireOpacity = Math.Max(0, Math.Min(255, value));
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int CellOpacity
		{
			get { return _CellOpacity; }
			set
			{
				_CellOpacity = Math.Max(0, Math.Min(255, value));
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int RegionOpacity
		{
			get { return _RegionOpacity; }
			set
			{
				_RegionOpacity = Math.Max(0, Math.Min(255, value));
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int BeaconPriority
		{
			get { return _BeaconPriority; }
			set
			{
				_BeaconPriority = value;
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int ViasPriority
		{
			get { return _ViasPriority; }
			set
			{
				_ViasPriority = value;
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int WirePriority
		{
			get { return _WirePriority; }
			set
			{
				_WirePriority = value;
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int CellPriority
		{
			get { return _CellPriority; }
			set
			{
				_CellPriority = value;
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public bool AutoPriority
		{
			get { return _AutoPriority; }
			set
			{
				_AutoPriority = value;
				SortEntities();
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public int RegionPriority
		{
			get { return _RegionPriority; }
			set
			{
				_RegionPriority = value;
				Invalidate();
			}
		}

		[Category("Entity Appearance")]
		public string ViasGroundText
		{
			get { return _viasGroundText; }
			set { _viasGroundText = value; }
		}

		[Category("Entity Appearance")]
		public string ViasPowerText
		{
			get { return _viasPowerText; }
			set { _viasPowerText = value; }
		}

	}
}
