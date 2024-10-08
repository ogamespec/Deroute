using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml.Serialization;

public enum EntityType
{
	Root = 0,
	ViasInput = 0x80,
	ViasOutput,
	ViasInout,
	ViasConnect,
	ViasFloating,
	ViasPower,
	ViasGround,
	WireInterconnect,
	WirePower,
	WireGround,
	CellNot,
	CellBuffer,
	CellMux,
	CellLogic,
	CellAdder,
	CellBusSupp,
	CellFlipFlop,
	CellLatch,
	CellOther,
	UnitRegfile,
	UnitMemory,
	UnitCustom,
	Beacon,
	Region,
	Layer,          // Invisible entity-container
}

public enum ViasShape
{
	Square = 1,
	Round,
}

public enum TextAlignment
{
	GlobalSettings = 0,
	Top,
	TopLeft,
	TopRight,
	BottomLeft,
	Bottom,
	BottomRight,
}

public enum LogicValue
{
	X = -2,
	Z = -1,
	Zero = 0,
	One = 1,
}


public class Entity
{
	private string _Label;
	private float _LambdaWidth;
	private float _LambdaHeight;
	private float _LambdaX;
	private float _LambdaY;
	private float _LambdaEndX;
	private float _LambdaEndY;
	private EntityType _Type;
	private bool _Selected;
	private EntityBox parentBox = null;
	private Color _ColorOverride;
	private Font _FontOverride;
	private TextAlignment labelAlignment;
	private int _Priority;
	private List<PointF> _pathPoints = null;
	[XmlIgnore]
	public List<PointF> SavedPathPoints = null;
	private int _WidthOverride;         // vias / wire
	private List<EntityType> _traverseBlackList = null;
	private string _moduleName = null;

	[XmlIgnore]
	public long SelectTimeStamp;

	[XmlIgnore]
	public int UserData;        // For temp values

	[XmlIgnore]
	public float SavedLambdaX;
	[XmlIgnore]
	public float SavedLambdaY;
	[XmlIgnore]
	public float SavedLambdaEndX;
	[XmlIgnore]
	public float SavedLambdaEndY;

	public Entity ()
	{
	}

	public Entity (Entity other)
	{
		_Label = other._Label;
		_LambdaWidth = other._LambdaWidth;
		_LambdaHeight = other._LambdaHeight;
		_LambdaX = other._LambdaX;
		_LambdaY = other._LambdaY;
		_LambdaEndX = other._LambdaEndX;
		_LambdaEndY = other._LambdaEndY;
		_Type = other._Type;
		_Selected = other._Selected;
		_ColorOverride = other._ColorOverride;
		_FontOverride = other._FontOverride;
		labelAlignment = other.labelAlignment;
		_Priority = other._Priority;
		_WidthOverride = other._WidthOverride;
		if (other._pathPoints != null)
		{
			_pathPoints = new List<PointF>();
			_pathPoints.AddRange(other._pathPoints);
		}
		if (_traverseBlackList != null)
		{
			_traverseBlackList = new List<EntityType>();
			_traverseBlackList.AddRange(other._traverseBlackList);
		}
	}

	[Category("Entity Properties")]
	public string Label
	{
		get { return _Label; }
		set
		{
			_Label = value;

			if (parentBox != null)
			{
				parentBox.LabelEdited(this);
				parentBox.Invalidate();
			}
		}
	}

	[Category("Entity Properties")]
	public float LambdaWidth
	{
		get { return _LambdaWidth; }
		set { _LambdaWidth = value;
			if (parentBox != null) parentBox.Invalidate(); }
	}

	[Category("Entity Properties")]
	public float LambdaHeight
	{
		get { return _LambdaHeight; }
		set { _LambdaHeight = value;
			if (parentBox != null) parentBox.Invalidate(); }
	}

	[Category("Entity Properties")]
	public float LambdaX
	{
		get { return _LambdaX; }
		set { _LambdaX = value;
			if (parentBox != null) parentBox.Invalidate();  }
	}

	[Category("Entity Properties")]
	public float LambdaY
	{
		get { return _LambdaY; }
		set { _LambdaY = value;
			if (parentBox != null) parentBox.Invalidate();     }
	}

	[Category("Entity Properties")]
	public float LambdaEndX
	{
		get { return _LambdaEndX; }
		set { _LambdaEndX = value;
			if (parentBox != null) parentBox.Invalidate();    }
	}

	[Category("Entity Properties")]
	public float LambdaEndY
	{
		get { return _LambdaEndY; }
		set { _LambdaEndY = value;
			if (parentBox != null) parentBox.Invalidate();    }
	}

	[Category("Entity Properties")]
	public EntityType Type
	{
		get { return _Type; }
		set { _Type = value;
			if (parentBox != null) parentBox.Invalidate();    }
	}

	[Category("Entity Properties")]
	[XmlIgnore]
	public bool Selected
	{
		get { return _Selected; }
		set
		{
			_Selected = value;

			SelectTimeStamp = DateTime.Now.Ticks;

			if (parentBox != null)
				parentBox.Invalidate();
		}
	}

	[Category("Entity Properties")]
	[Description("Set color other than Black to override it.")]
	[XmlIgnore]
	public Color ColorOverride
	{
		get { return _ColorOverride; }
		set
		{
			_ColorOverride = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[XmlElement("ColorOverride")]
	[Browsable(false)]
	public string ClrGridHtml
	{
		get { return ColorTranslator.ToHtml(_ColorOverride); }
		set { _ColorOverride = ColorTranslator.FromHtml(value); }
	}

	[Category("Entity Properties")]
	[Description("Set font other than null to override it.")]
	[XmlIgnore]
	public Font FontOverride
	{
		get { return _FontOverride; }
		set
		{
			_FontOverride = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[XmlElement("FontOverride")]
	[Browsable(false)]
	public string FontGridHtml
	{
		get { return FontXmlConverter.ConvertToString(_FontOverride); }
		set { _FontOverride = FontXmlConverter.ConvertToFont(value); }
	}

	[Category("Entity Properties")]
	public TextAlignment LabelAlignment
	{
		get { return labelAlignment; }
		set
		{
			labelAlignment = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[Category("Entity Properties")]
	public int Priority
	{
		get { return _Priority; }
		set
		{
			_Priority = value;
			if (parentBox != null)
			{
				parentBox.SortEntities();
				parentBox.Invalidate();
			}
		}
	}

	[Category("Entity Properties")]
	public List<PointF> PathPoints
	{
		get { return _pathPoints; }
		set
		{
			_pathPoints = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[Category("Entity Properties")]
	public int WidthOverride
	{
		get { return _WidthOverride; }
		set
		{
			_WidthOverride = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[XmlIgnore]
	public Entity parent = null;

	private List<Entity> children = new List<Entity>();
	[Category("Entity Properties")]
	public List<Entity> Children
	{
		get { return children; }
		set { children = value; }
	}

	private bool visible = true;
	[Category("Entity Properties")]
	[XmlIgnore]
	public bool Visible
	{
		get { return visible; }
		set
		{
			visible = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	public void SetParentControl ( EntityBox parent )
	{
		parentBox = parent;
	}

	public bool IsWire()
	{
		return (Type == EntityType.WireGround ||
				 Type == EntityType.WireInterconnect ||
				 Type == EntityType.WirePower);
	}

	public bool IsVias()
	{
		return (Type == EntityType.ViasConnect ||
				 Type == EntityType.ViasFloating ||
				 Type == EntityType.ViasGround ||
				 Type == EntityType.ViasInout ||
				 Type == EntityType.ViasInput ||
				 Type == EntityType.ViasOutput ||
				 Type == EntityType.ViasPower);
	}

	public bool IsPort()
	{
		return Type == EntityType.ViasInput || Type == EntityType.ViasOutput || Type == EntityType.ViasInout;
	}

	public bool IsCell()
	{
		return (Type == EntityType.CellNot ||
				 Type == EntityType.CellBuffer ||
				 Type == EntityType.CellMux ||
				 Type == EntityType.CellLogic ||
				 Type == EntityType.CellAdder ||
				 Type == EntityType.CellBusSupp ||
				 Type == EntityType.CellFlipFlop ||
				 Type == EntityType.CellLatch ||
				 Type == EntityType.CellOther ||
				 Type == EntityType.UnitRegfile ||
				 Type == EntityType.UnitMemory ||
				 Type == EntityType.UnitCustom);
	}

	public bool IsUnit()
	{
		return (Type == EntityType.UnitRegfile ||
				 Type == EntityType.UnitMemory ||
				 Type == EntityType.UnitCustom);
	}

	public bool IsRegion()
	{
		return (Type == EntityType.Region);
	}

	public float WireLength()
	{
		if (!IsWire())
			return 0.0F;

		return (float)Math.Sqrt(Math.Pow(LambdaEndX - LambdaX, 2) +
								Math.Pow(LambdaEndY - LambdaY, 2));
	}

	[Category("Entity Properties")]
	[XmlIgnore]
	public float WireLengthLambda
	{
		get { return WireLength(); }
		set { }
	}

	public float Tangent()
	{
		if (!IsWire())
			return float.NaN;

		return (float)(LambdaEndY - LambdaY) / (LambdaEndX - LambdaX + float.Epsilon);
	}

	/// <summary>
	/// Verify that the entity itself and all of its parents are visible.
	/// </summary>
	/// <returns></returns>
	public bool IsVisible()
	{
		if (!Visible)
			return false;

		bool parent_visible = true;

		var next_parent = parent;
		while (next_parent != null)
		{
			if (!next_parent.Visible)
			{
				parent_visible = false;
				break;
			}
			next_parent = next_parent.parent;
		}

		return parent_visible;
	}

	[Category("Entity Properties")]
	[XmlIgnore]
	public float WireTangent
	{
		get { return Tangent(); }
		set { }
	}

	[Category("Entity Properties")]
	[Description("List for prohibiting traverse to specified entity types. It is used when it is necessary, for example, to prevent a wire from going to illegal entities.")]
	public List<EntityType> TraverseBlackList
	{
		get { return _traverseBlackList; }
		set
		{
			_traverseBlackList = value;
			if (parentBox != null) parentBox.Invalidate();
		}
	}

	[Category("Entity Properties")]
	[Description("To assign a shared Verilog module to multiple cells/units, specify the module name here. All modules are displayed on the `Modules` tab")]
	public string Module
	{
		get { return _moduleName; }
		set
		{
			_moduleName = value;

			if (parentBox != null)
			{
				parentBox.ModuleEdited(this);
				parentBox.Invalidate();
			}
		}
	}


	#region "Sim Support"

	[XmlIgnore]
	[Category("Simulation")]
	[Description("Determines if the entity is being monitored. If enabled, then after each simulation step the value goes to Waves")]
	public bool Scope { get; set; } = false;

	[XmlIgnore]
	[Category("Simulation")]
	[Description("Current logic value for the simulator")]
	public LogicValue Val { get; set; } = LogicValue.X;

	[XmlIgnore]
	[Category("Simulation")]
	[Description("Previous logic value for the simulator")]
	public LogicValue PrevVal { get; set; } = LogicValue.X;

	#endregion "Sim Support"

}
