using Godot;

public partial class Oscilloscope : ColorRect
{
    private Line2D _line;
    private ColorRect _scanBar;

    public override void _Ready()
    {
        _line = new Line2D { Width = 3, DefaultColor = Colors.SpringGreen, Antialiased = true };
        AddChild(_line);

        _scanBar = new ColorRect { Color = new Color(1, 1, 1, 0.4f), Size = new Vector2(2, Size.Y) };
        AddChild(_scanBar);
        _scanBar.Visible = false;
    }

    public override void _Draw()
    {
        float hStep = Size.Y / 5;
        for (int i = 0; i <= 5; i++)
        {
            float y = Size.Y - (i * hStep);
            DrawLine(new Vector2(0, y), new Vector2(Size.X, y), new Color(1, 1, 1, 0.1f), 1);
        }
    }

    public void Display(int[] signal, float progress = 1.0f)
    {
        _line.ClearPoints();
        if (signal == null || signal.Length == 0) return;

        _scanBar.Visible = progress > 0 && progress < 1.0f;
        _scanBar.Position = new Vector2(Size.X * progress, 0);

        float xStep = Size.X / signal.Length;
        float yUnit = Size.Y / 5;

        int pointsToDraw = (int)(signal.Length * progress);
        for (int i = 0; i < pointsToDraw; i++)
        {
            float y = Size.Y - (signal[i] * yUnit);
            _line.AddPoint(new Vector2(i * xStep, y));
            _line.AddPoint(new Vector2((i + 1) * xStep, y));
        }
    }

    public void Clear() { _line.ClearPoints(); _scanBar.Visible = false; }
}