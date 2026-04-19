using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainBoard : Node2D
{
    public class NodeData
    {
        public string TrueType, VisualType;
        public Vector2 Position;
        public List<Vector2I> Neighbors = new List<Vector2I>();

        public NodeData(string t, string v, Vector2 pos)
        {
            TrueType = t;
            VisualType = v;
            Position = pos;
        }
    }

    private Dictionary<Vector2I, NodeData> _nodes = new Dictionary<Vector2I, NodeData>();
    private List<LevelRegistry.LevelConfig> _levels;
    private int _currentLevelIdx = 0;

    private bool[] _levelCompletionStatus;

    private List<Vector2I> _path = new List<Vector2I>();
    private Vector2I _probePos = new Vector2I(-1, -1);
    private bool _isDrawing = false;

    private int _dialogueIdx = 0;
    private List<string> _history = new List<string>();
    private int _hintCount = 0;

    private Line2D _pathRenderer;
    private Oscilloscope _inScope, _tgtScope, _outScope, _prbScope;
    private Label _statusLabel, _titleLabel;
    private Control _diagBox;

    private CanvasLayer _historyLayer;
    private RichTextLabel _historyText, _diagLabel;

    private Button _prevBtn, _nextBtn;
    private ColorRect _passIndicator; 

    private HSlider _volumeSlider;
    private int _masterBusIndex;
    private AudioStreamPlayer _sfxSwitch, _sfxFail, _sfxPass, _sfxScan, _sfxType, _bgmPlayer;

    public override void _Ready()
    {
        _levels = LevelRegistry.GetLevels();
        _pathRenderer = GetNode<Line2D>("PathLine");

        _levelCompletionStatus = new bool[_levels.Count];

        _pathRenderer = GetNode<Line2D>("PathLine");
        _statusLabel = GetNode<Label>("%StatusLabel");
        _diagLabel = GetNode<RichTextLabel>("DialogueBox/Label");
        _titleLabel = GetNode<Label>("DeviceTitle");
        _diagBox = GetNode<Control>("DialogueBox");

        _inScope = GetNode<Oscilloscope>("InputScreen");
        _tgtScope = GetNode<Oscilloscope>("TargetScreen");
        _outScope = GetNode<Oscilloscope>("OutputScreen");
        _prbScope = GetNode<Oscilloscope>("ProbeScreen");

        _historyLayer = GetNode<CanvasLayer>("HistoryLayer");
        _historyText = GetNode<RichTextLabel>("%HistoryText");
        _historyLayer.Visible = false;

        _volumeSlider = GetNode<HSlider>("%VolumeSlider");
        _volumeSlider.MinValue = 0;
        _volumeSlider.MaxValue = 100;
        _masterBusIndex = AudioServer.GetBusIndex("Master");

        _sfxSwitch = GetNode<AudioStreamPlayer>("SfxSwitch");
        _sfxFail = GetNode<AudioStreamPlayer>("SfxFail");
        _sfxPass = GetNode<AudioStreamPlayer>("SfxPass");
        _sfxScan = GetNode<AudioStreamPlayer>("SfxScan");
        _sfxType = GetNode<AudioStreamPlayer>("SfxType");
        _bgmPlayer = GetNode<AudioStreamPlayer>("BgmPlayer");

        _volumeSlider.ValueChanged += UpdateMasterVolume;
        UpdateMasterVolume(80);


        GetNode<Button>("SwitchButton").Pressed += ExecuteCircuit;
        GetNode<Button>("HintButton").Pressed += ShowNextHint;
        GetNode<Button>("HistoryButton").Pressed += OpenHistory;
        GetNode<Button>("%CloseButton").Pressed += CloseHistory;


        _prevBtn = GetNode<Button>("%PrevButton");
        _nextBtn = GetNode<Button>("%NextButton");
        _passIndicator = GetNode<ColorRect>("%PassIndicator");

        _prevBtn.Pressed += () => LoadLevel(_currentLevelIdx - 1);
        _nextBtn.Pressed += () => LoadLevel(_currentLevelIdx + 1);

        _diagLabel.GuiInput += (ev) =>
        {
            if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left) AdvanceDialogue();
        };

        var titlePrefab = GD.Load<PackedScene>("res://title_screen.tscn");
        var titleInstance = titlePrefab.Instantiate<TitleScreen>();

        AddChild(titleInstance);

        titleInstance.StartGame += () =>
        {
            LoadLevel(0);
        };

        _inScope.Clear();
        _tgtScope.Clear();
        _outScope.Clear();
    }

    private void UpdateMasterVolume(double sliderValue)
    {
        if (sliderValue <= 0)
        {
            AudioServer.SetBusMute(_masterBusIndex, true);
        }
        else
        {
            AudioServer.SetBusMute(_masterBusIndex, false);

            float db = -48.0f + ((float)sliderValue / 100.0f) * 48.0f;
            AudioServer.SetBusVolumeDb(_masterBusIndex, db);
        }
    }

    private void LoadLevel(int idx)
    {
        if (idx < 0 || idx >= _levels.Count) return;

        _currentLevelIdx = idx;
        var cfg = _levels[idx];

        _nodes.Clear();
        _path.Clear();
        _history.Clear();
        _dialogueIdx = 0;
        _hintCount = 0;
        _probePos = new Vector2I(-1, -1);
        _pathRenderer.ClearPoints();

        // 网格生成 (MxN)
        float spacing = 70f;
        Vector2 startPos = new Vector2(50, 50);
        for (int x = 0; x < cfg.GridSize.X; x++)
        {
            for (int y = 0; y < cfg.GridSize.Y; y++)
            {
                var c = new Vector2I(x, y);
                _nodes[c] = new NodeData("NUL", "NUL", startPos + new Vector2(x * spacing, y * spacing));
            }
        }

        foreach (var c in _nodes.Keys)
        {
            Vector2I[] dirs = { new Vector2I(1, 0), new Vector2I(0, 1) };
            foreach (var d in dirs)
            {
                if (_nodes.ContainsKey(c + d))
                {
                    _nodes[c].Neighbors.Add(c + d);
                    _nodes[c + d].Neighbors.Add(c);
                }
            }
        }

        foreach (var ov in cfg.NodeOverrides)
        {
            _nodes[ov.Key].TrueType = ov.Value.True;
            _nodes[ov.Key].VisualType = ov.Value.Visual;
        }

        if (cfg.BrokenLinks != null)
            foreach (var bl in cfg.BrokenLinks)
            {
                _nodes[bl.Item1].Neighbors.Remove(bl.Item2);
                _nodes[bl.Item2].Neighbors.Remove(bl.Item1);
            }

        _titleLabel.Text = "DEVICE: " + cfg.DeviceName;
        _inScope.Display(cfg.Input);
        _tgtScope.Display(cfg.Target);
        _outScope.Clear();
        _prbScope.Clear();
        _prbScope.Visible = cfg.ProbeEnabled;

        ShowDialogue(cfg.Dialogues[0]);
        SetStatus("SYS_READY", Colors.White);

        // --- 核心更新：刷新导航 UI 状态 ---
        UpdateNavigationUI();

        QueueRedraw();
    }

    private void UpdateNavigationUI()
    {
        bool isCurrentPassed = _levelCompletionStatus[_currentLevelIdx];

        // 1. 设置指示灯颜色
        _passIndicator.Color = isCurrentPassed ? Colors.Green : Colors.Red;

        // 2. 只有当前关过了且不是最后一关，才允许按 Next
        _nextBtn.Disabled = !isCurrentPassed || (_currentLevelIdx >= _levels.Count - 1);

        // 3. 只有不是第一关，才允许按 Prev
        _prevBtn.Disabled = (_currentLevelIdx <= 0);
    }

    private void ShowDialogue(string txt, bool isHint = false)
    {
        if (!isHint) _history.Add(txt);
        _diagLabel.Text = txt;
        _diagLabel.VisibleCharacters = 0;

        _sfxType.Play();

        Tween t = CreateTween();
        t.TweenProperty(_diagLabel, "visible_characters", txt.Length, 0.8f).SetTrans(Tween.TransitionType.Linear);

        t.Finished += () => _sfxType.Stop();
    }

    private void AdvanceDialogue()
    {
        var cfg = _levels[_currentLevelIdx];
        if (_dialogueIdx < cfg.Dialogues.Length - 1)
        {
            _dialogueIdx++;
            ShowDialogue(cfg.Dialogues[_dialogueIdx]);
        }
    }

    private void ShowNextHint()
    {
        var cfg = _levels[_currentLevelIdx];
        if (cfg.Hints == null || cfg.Hints.Length == 0) return;
        ShowDialogue("HINT: " + cfg.Hints[_hintCount], true);
        _hintCount = (_hintCount + 1) % cfg.Hints.Length;
    }

    private void OpenHistory()
    {
        _historyLayer.Visible = true;
        _historyText.Text = "[color=gray]--- CONVERSATION LOG ---[/color]\n\n";
        foreach (string line in _history)
        {
            _historyText.Text += "[color=springgreen]>[/color] " + line + "\n\n";
        }
    }

    private void CloseHistory() => _historyLayer.Visible = false;

    private void SetStatus(string m, Color c)
    {
        _statusLabel.Text = m;
        _statusLabel.AddThemeColorOverride("font_color", c);
    }

    private void UpdateStatusByPath()
    {
        if (_isDrawing) SetStatus("RTE_ACTIVE", Colors.Cyan);
        else if (_path.Count > 0 && _nodes[_path.Last()].TrueType == "OUT") SetStatus("RTE_READY", Colors.GreenYellow);
        else if (_path.Count > 0) SetStatus("RTE_PENDING", Colors.Yellow);
        else SetStatus("SYS_READY", Colors.White);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_historyLayer.Visible) return;

        if (@event is InputEventMouseButton mb)
        {
            Vector2I clicked = GetNodeAt(mb.Position);
            if (mb.ButtonIndex == MouseButton.Left)
            {
                if (mb.Pressed && clicked != new Vector2I(-1, -1))
                {
                    if (_nodes[clicked].TrueType == "INP")
                    {
                        _path.Clear();
                        _path.Add(clicked);
                        _isDrawing = true;
                    }
                    else if (_path.Count > 0 && clicked == _path.Last()) _isDrawing = true;
                }
                else _isDrawing = false;

                UpdateStatusByPath();
            }
            else if (mb.ButtonIndex == MouseButton.Right && mb.Pressed && _levels[_currentLevelIdx].ProbeEnabled)
            {
                if (clicked != new Vector2I(-1, -1))
                {
                    _probePos = (_probePos == clicked) ? new Vector2I(-1, -1) : clicked;
                    QueueRedraw();
                }
            }
        }
        else if (@event is InputEventMouseMotion mm && _isDrawing)
        {
            Vector2I curr = GetNodeAt(GetGlobalMousePosition());
            if (curr != new Vector2I(-1, -1) && curr != _path.Last())
            {
                if (_nodes[_path.Last()].Neighbors.Contains(curr) && !_path.Contains(curr))
                {
                    _path.Add(curr);
                    UpdatePathUI();
                }
                else if (_path.Count > 1 && curr == _path[_path.Count - 2])
                {
                    _path.RemoveAt(_path.Count - 1);
                    UpdatePathUI();
                }
            }
        }
    }

    private async void ExecuteCircuit()
    {
        if (_path.Count == 0 || _nodes[_path.Last()].TrueType != "OUT")
        {
            SetStatus("ERR: NO_CONN", Colors.Red);
            return;
        }

        _sfxSwitch.Play();

        var cfg = _levels[_currentLevelIdx];
        int[] sig = (int[])cfg.Input.Clone();
        int[] prb = null;
        bool err = false;

        for (int i = 0; i < _path.Count; i++)
        {
            var n = _nodes[_path[i]];
            if (n.TrueType == "ERR") err = true;
            else sig = SignalProcessor.Process(sig, n.TrueType);
            if (err)
                for (int j = 0; j < sig.Length; j++)
                    sig[j] = (GD.Randi() % 2 == 0) ? 5 : 0;
            if (i % cfg.Resistance == 0 && i > 0) sig = SignalProcessor.ApplyDecay(sig);
            if (_path[i] == _probePos) prb = (int[])sig.Clone();
        }

        SetStatus("SCANNING...", Colors.Cyan);
        float originalBgmVolume = _bgmPlayer.VolumeDb;
        _bgmPlayer.VolumeDb -= 12.0f;
        float tickInterval = 1.0f / sig.Length;
        for (int i = 0; i < sig.Length; i++)
        {
            GetTree().CreateTimer(i * tickInterval).Timeout += () => _sfxScan.Play();
        }

        Tween t = CreateTween();
        t.SetParallel(true);
        t.TweenMethod(Callable.From<float>((p) => _outScope.Display(sig, p)), 0f, 1f, 1f);
        if (prb != null) t.TweenMethod(Callable.From<float>((p) => _prbScope.Display(prb, p)), 0f, 1f, 1f);

        await ToSignal(t, "finished");
        _bgmPlayer.VolumeDb = originalBgmVolume;

        if (!err && sig.SequenceEqual(cfg.Target))
        {
            SetStatus("MATCH_OK", Colors.Green);

            _levelCompletionStatus[_currentLevelIdx] = true;
            _sfxPass.Play();
            UpdateNavigationUI();
        }
        else
        {
            SetStatus(err ? "HW_FAULT" : "SIG_MISMATCH", Colors.Red);
            _sfxFail.Play();
        }
    }

    private Vector2I GetNodeAt(Vector2 p)
    {
        foreach (var kvp in _nodes)
            if (p.DistanceTo(kvp.Value.Position) < 30)
                return kvp.Key;
        return new Vector2I(-1, -1);
    }

    private void UpdatePathUI()
    {
        _pathRenderer.ClearPoints();
        foreach (var p in _path) _pathRenderer.AddPoint(_nodes[p].Position);
    }

    public override void _Draw()
    {
        foreach (var kvp in _nodes)
        {
            foreach (var n in kvp.Value.Neighbors)
                if (n.X > kvp.Key.X || n.Y > kvp.Key.Y)
                    DrawLine(kvp.Value.Position, _nodes[n].Position, new Color(1, 1, 1, 0.05f), 2);
            Color col = GetColorByType(kvp.Value.VisualType);
            DrawCircle(kvp.Value.Position, 20, col);
            if (kvp.Key == _probePos) DrawArc(kvp.Value.Position, 28, 0, Mathf.Tau, 32, Colors.Cyan, 3);
        }
    }

    private Color GetColorByType(string type)
    {
        switch (type)
        {
            case "INP": return Colors.Green;
            case "OUT": return Colors.Red;
            case "NUL": return Colors.DimGray;
            case "INV": return Colors.DodgerBlue;
            case "ADD": return Colors.Orange;
            case "MUL": return Colors.Yellow;
            case "DLY": return Colors.Purple;
            case "OSC": return Colors.Cyan;
            case "ERR": return Colors.Black;
            case "UNK": return Colors.Black;
            default: return Colors.DimGray;
        }
    }
}