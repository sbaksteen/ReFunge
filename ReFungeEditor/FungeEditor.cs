using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReFunge;
using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;
using Vector2 = System.Numerics.Vector2;

namespace ReFungeEditor;

public class FungeEditor : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SpriteFont _font;
    
    private Interpreter _interpreter;
    
    private FungeSpace Space => _interpreter.PrimarySpace;
    private List<FungeIP> IPList => _interpreter.IPList;

    private Point _charSize;

    private double _scale = 0.2;

    private Point _topLeftPoint = Point.Zero;
    
    private Color _backgroundColor = Color.Black;
    private Color _textColor = Color.White;
    private Color _cursorColor = Color.Gray;
    private Color _ipColor = Color.Blue;
    
    private int _scrollValue;
    
    private FungeVector _cursor = new();
    private FungeVector _topLeft = new();
    private int _rightDim = 1;
    private int _downDim = 2;

    private bool ImGuiHasKeyboard => ImGui.GetIO().WantCaptureKeyboard;
    private bool ImGuiHasMouse => ImGui.GetIO().WantCaptureMouse;
    
    public FungeVector RightDirection => FungeVector.Cardinal(int.Abs(_rightDim)-1, int.Sign(_rightDim));
    public FungeVector DownDirection => FungeVector.Cardinal(int.Abs(_downDim)-1, int.Sign(_downDim));
    
    private bool _running = false;
    private int _stepsPerFrame = 1;
    private int _framesPerStep = 1;
    private int _stepCounter = 0;
    private bool _slowMode = false;

    private bool _dragging = false;
    private Point _dragPos;

    private bool _followingIP = false;
    private FungeIP _followIP;

    private float _cursorBlink = 0f;
    
    private StringWriter _output = new();
    
    private ImGuiRenderer _imGuiRenderer;
    private int _currIPNum;
    private int _currStackNum;

    private float _arrowTimer = 0f;
    private readonly float _arrowTimerMax = 0.5f;

    private enum ArrowTimerState
    {
        Inactive,
        Active,
        Done
    }
    
    private ArrowTimerState _arrowTimerState = ArrowTimerState.Inactive;
    private int _dimValue;
    private string _loadPath;
    private bool _loading;
    private int _customDim;
    private bool _newSpace;

    public FungeEditor()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _imGuiRenderer = new ImGuiRenderer(this);
        _imGuiRenderer.RebuildFontAtlas();

        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        Window.TextInput += OnTextInput;
        Window.KeyDown += OnKeyDown;
        
        Window.AllowUserResizing = true;
        _scrollValue = Mouse.GetState().ScrollWheelValue;
        _interpreter = new Interpreter(2, output: _output);
        _interpreter.PrimarySpace.LoadString(new FungeVector(), ">\"!dlroW ,olleH\":v\n" +
                                                                           "              v:,_@\n" +
                                                                           "              >  ^");
        
        
        
        base.Initialize();
    }

    private void OnKeyDown(object sender, InputKeyEventArgs e)
    {
        if (ImGuiHasKeyboard || !IsActive) return;
        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl))
        {
            var d = int.Abs(_downDim);
            var ds = int.Sign(_downDim);
            var r = int.Abs(_rightDim);
            var rs = int.Sign(_rightDim);
            if (Space.Dim >= 3 && e.Key == Keys.L)
            {
                _rightDim = d switch
                {
                    1 => r switch
                    {
                        2 => 3 * rs * -ds,
                        3 => 2 * rs * ds,
                        _ => 3
                    },
                    2 => r switch
                    {
                        1 => 3 * rs * ds,
                        3 => 1 * rs * -ds,
                        _ => 1
                    },
                    3 => r switch
                    {
                        1 => 2 * rs * -ds,
                        2 => 1 * rs * ds,
                        _ => 2
                    },
                    _ => _rightDim
                };
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }

            if (Space.Dim >= 3 && e.Key == Keys.J)
            {
                _rightDim = d switch
                {
                    1 => r switch
                    {
                        2 => 3 * rs * ds,
                        3 => 2 * rs * -ds,
                        _ => 3
                    },
                    2 => r switch
                    {
                        1 => 3 * rs * -ds,
                        3 => 1 * rs * ds,
                        _ => 1
                    },
                    3 => r switch
                    {
                        1 => 2 * rs * ds,
                        2 => 1 * rs * -ds,
                        _ => 2
                    },
                    _ => _rightDim
                };
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }

            if (Space.Dim >= 3 && e.Key == Keys.I)
            {
                _downDim = r switch
                {
                    1 => d switch
                    {
                        2 => 3 * rs * ds,
                        3 => 2 * rs * -ds,
                        _ => 3
                    },
                    2 => d switch
                    {
                        1 => 3 * rs * -ds,
                        3 => 1 * rs * ds,
                        _ => 1
                    },
                    3 => d switch
                    {
                        1 => 2 * rs * ds,
                        2 => 1 * rs * -ds,
                        _ => 2
                    }
                };
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }
            if (Space.Dim >= 3 && e.Key == Keys.K)
            {
                _downDim = r switch
                {
                    1 => d switch
                    {
                        2 => 3 * rs * -ds,
                        3 => 2 * rs * ds,
                        _ => 3
                    },
                    2 => d switch
                    {
                        1 => 3 * rs * ds,
                        3 => 1 * rs * -ds,
                        _ => 1
                    },
                    3 => d switch
                    {
                        1 => 2 * rs * -ds,
                        2 => 1 * rs * ds,
                        _ => 2
                    }
                };
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }

            if (Space.Dim >= 2 && e.Key == Keys.U)
            {
                var tmp = _rightDim;
                _rightDim = _downDim;
                _downDim = -tmp;
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }
            if (Space.Dim >= 2 && e.Key == Keys.O)
            {
                var tmp = _rightDim;
                _rightDim = -_downDim;
                _downDim = tmp;
                _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                           DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
            }
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _font = Content.Load<SpriteFont>("JetBrainsMonoNL");
        _charSize = _font.MeasureString("M").ToPoint();
        _topLeft = _cursor - RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                   DownDirection * (Window.ClientBounds.Size.Y/_charSize.Y/2);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        HandleMouseInput();
        HandleKeyboardInput(gameTime);

        

        if (_running)
        {
            if (_slowMode)
            {
                _stepCounter++;
                if (_stepCounter >= _framesPerStep)
                {
                    _stepCounter = 0;
                    _interpreter.DoStep();
                }
            }
            else
            {
                for (var i = 0; i < _stepsPerFrame; i++)
                {
                    _interpreter.DoStep();
                }
                if (_currIPNum >= IPList.Count)
                {
                    _currIPNum = IPList.Count - 1;
                }
            }
        }

        if (_followingIP)
        {
            _topLeft = _followIP.Position - 
                       RightDirection * (Window.ClientBounds.Size.X/_charSize.X/2) -
                       DownDirection *  (Window.ClientBounds.Size.Y/_charSize.Y/2);
        }
        

        base.Update(gameTime);
    }

    private void HandleKeyboardInput(GameTime gameTime)
    {
        if (ImGuiHasKeyboard || !IsActive) return;
        if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right) ||
            Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            switch (_arrowTimerState)
            {
                case ArrowTimerState.Inactive:
                    DoArrowKeys();
                    _arrowTimerState = ArrowTimerState.Active;
                    _arrowTimer = 0f;
                    break;
                case ArrowTimerState.Active:
                    if (_arrowTimer >= _arrowTimerMax)
                    {
                        _arrowTimerState = ArrowTimerState.Done;
                    }
                    _arrowTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case ArrowTimerState.Done:
                    DoArrowKeys();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid arrowtimerstate");
            }
        }
        else
        {
            _arrowTimerState = ArrowTimerState.Inactive;
        }
    }

    private void DoArrowKeys()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            _cursor -= RightDirection;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            _cursor += RightDirection;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            _cursor -= DownDirection;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            _cursor += DownDirection;
        }
    }

    private void HandleMouseInput()
    {
        if (ImGuiHasMouse || !IsActive) return;
        if (Mouse.GetState().Position.X > Window.ClientBounds.Size.X || Mouse.GetState().Position.Y > Window.ClientBounds.Size.Y
            || Mouse.GetState().Position.X < 0 || Mouse.GetState().Position.Y < 0)
        {
            return;
        }

        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            var pos = (Mouse.GetState().Position + _topLeftPoint) / _charSize;
            _cursor = _topLeft + RightDirection * pos.X + DownDirection * pos.Y;
        }
        
        if (Mouse.GetState().RightButton == ButtonState.Pressed)
        {
            if (!_dragging)
            {
                _dragging = true;
                _dragPos = Mouse.GetState().Position;
            }
            else
            {
                var newPos = Mouse.GetState().Position;
                var delta = newPos - _dragPos;
                _topLeftPoint -= delta;
                if (_topLeftPoint.X < 0 || _topLeftPoint.Y < 0 || 
                    _topLeftPoint.X >= _charSize.X || _topLeftPoint.Y >= _charSize.Y)
                {
                    var adjustPoint = _topLeftPoint / _charSize;
                    if (_topLeftPoint.X < 0) adjustPoint.X--;
                    if (_topLeftPoint.Y < 0) adjustPoint.Y--;
                    var rightDirection = FungeVector.Cardinal(int.Abs(_rightDim)-1, int.Sign(_rightDim));
                    var downDirection = FungeVector.Cardinal(int.Abs(_downDim)-1, int.Sign(_downDim));
                    _topLeft += rightDirection * adjustPoint.X + downDirection * adjustPoint.Y;
                    _topLeftPoint -= adjustPoint * _charSize;
                }
                _dragPos = newPos;
            }
        }
        
        if (Mouse.GetState().RightButton == ButtonState.Released)
        {
            _dragging = false;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_backgroundColor);

        _cursorBlink = 0.3f + 0.7f * float.Pow(float.Abs((float)double.Sin(gameTime.TotalGameTime.TotalSeconds)), 2f);

        Texture2D ipTexture = new Texture2D(GraphicsDevice, 1, 1);
        ipTexture.SetData(new[] {Color.White});

        _spriteBatch.Begin();

        DrawIPs(ipTexture);
        
        DrawSpace();
        
        _spriteBatch.End();
        
        // Call BeforeLayout first to set things up
        _imGuiRenderer.BeforeLayout(gameTime);

        // Draw our UI
        ImGuiLayout(gameTime);

        // Call AfterLayout now to finish up and draw all the things
        _imGuiRenderer.AfterLayout();

        base.Draw(gameTime);
    }

    private void ImGuiLayout(GameTime gameTime)
    {
        if (_loading)
        {
            ImGui.OpenPopup("Load File");
            _loading = false;
        }

        if (_newSpace)
        {
            ImGui.OpenPopup("New Space");
            _newSpace = false;
        }
        
        LoadFileModal();

        NewSpaceModal();
        
        MainMenuBar();
        
        DebugControls();

        InterpreterDataView();

        IPDataView();
        
        ViewInfo();

        OutputView();
    }

    private void ViewInfo()
    {
        if (!ImGui.Begin("Space View Info")) return;
        ImGui.Text("Right direction: " + RightDirection);
        ImGui.Text("Down direction: " + DownDirection);
            
        ImGui.Text("Cursor: " + _cursor);
        ImGui.Text("Cell under cursor: " + Space[_cursor] + " (" + (char)Space[_cursor] + ")");
        ImGui.End();
    }

    private void OutputView()
    {
        if (!ImGui.Begin("Output")) return;
        ImGui.Text("Output:");
        if (ImGui.Button("Clear"))
        {
            _output.GetStringBuilder().Clear();
        }
        ImGui.BeginChild("OutputScrolling", new Vector2(), ImGuiChildFlags.Border, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.TextUnformatted(_output.ToString());
        ImGui.EndChild();
        ImGui.End();
    }

    private void IPDataView()
    {
        if (!ImGui.Begin("IP View", ImGuiWindowFlags.AlwaysAutoResize)) return;
        if (IPList.Count == 0) goto EndIPView;
        ImGui.PushItemWidth(80);
        ImGui.InputInt("IP #", ref _currIPNum, 1);
        if (_currIPNum < 0) _currIPNum = 0;
        if (_currIPNum >= IPList.Count) _currIPNum = IPList.Count - 1;

        var currip = IPList[_currIPNum];
            
        ImGui.Text("ID: " + currip.ID);

        ImGui.Checkbox("Follow", ref _followingIP);
        if (_followingIP)
        {
            _followIP = currip;
        }

        ImGui.Text("Position: " + currip.Position);
        ImGui.Text("Delta: " + currip.Delta);
        if (currip.StringMode)
        {
            ImGui.Text("String Mode");
        }

        if (ImGui.CollapsingHeader("Fingerprint Semantics"))
        {
            ImGui.BeginTable("FingerprintTable", 2);
            ImGui.TableSetupColumn("Char");
            ImGui.TableSetupColumn("Instruction");
            ImGui.TableHeadersRow();
            foreach (var stack in currip.FingerprintStacks)
            {
                if (stack.Count == 0) continue;
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(((char)('A' + currip.FingerprintStacks.ToList().IndexOf(stack))).ToString());
                ImGui.TableSetColumnIndex(1);
                // ImGui.Text(InstructionRegistry.NameOf(stack.Peek())); // Needs refactoring
            }
            ImGui.EndTable();
        }

        if (ImGui.CollapsingHeader("Stack"))
        {
            ImGui.Text("Stack ");
            ImGui.SameLine();
            ImGui.InputInt(" of " + currip.StackStack.Size, ref _currStackNum);
            if (_currStackNum < 0) _currStackNum = 0;
            if (_currStackNum >= currip.StackStack.Size) _currStackNum = currip.StackStack.Size - 1;
            ImGui.BeginTable("StackTable", 2);
            ImGui.TableSetupColumn("Int");
            ImGui.TableSetupColumn("Char");
            ImGui.TableHeadersRow();
            var stack = IPList[_currIPNum].StackStack[_currStackNum];
            for (var i = 0; i < stack.Size; i++)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(stack[i].ToString());
                ImGui.TableSetColumnIndex(1);
                ImGui.Text(((char)stack[i]).ToString());
            }

            ImGui.EndTable();
        }

        EndIPView:
        ImGui.End();
    }

    private void InterpreterDataView()
    {
        if (!ImGui.Begin("Interpreter View")) return;
        ImGui.Text("Tick: " + _interpreter.Tick);
        ImGui.Text($"Space Bounds: {Space.MinCoords}, {Space.MaxCoords}");
        ImGui.Text("IPs: " + IPList.Count);
        ImGui.Text("IP list:");
        ImGui.BeginChild("Scrolling");
        foreach (var ip in IPList)
        {
            if (ImGui.Button("Find##" + IPList.IndexOf(ip)))
            {
                _topLeft = ip.Position -
                           RightDirection * (Window.ClientBounds.Size.X / _charSize.X / 2) -
                           DownDirection * (Window.ClientBounds.Size.Y / _charSize.Y / 2);
                _currIPNum = IPList.IndexOf(ip);
            }

            ImGui.SameLine();
            ImGui.Text(ip.ToString());
        }

        ImGui.EndChild();
        ImGui.End();
    }

    private void DebugControls()
    {
        if (!ImGui.Begin("Main Controls", ImGuiWindowFlags.AlwaysAutoResize)) return;
        if (ImGui.Button("Step"))
        {
            _interpreter.DoStep();
        }
        ImGui.SameLine();
        if (ImGui.Button("Run"))
        {
            _running = true;
        }
        ImGui.SameLine();
        if (ImGui.Button("Stop"))
        {
            _running = false;
        }
        ImGui.PushItemWidth(100);
        ImGui.SliderInt("Steps per Frame", ref _stepsPerFrame, 1, 100);
        ImGui.SliderInt("Frames per Step", ref _framesPerStep, 1, 100);
        ImGui.PopItemWidth();
        ImGui.Checkbox("Slow Mode", ref _slowMode);
        ImGui.End();
    }

    private void MainMenuBar()
    {
        if (!ImGui.BeginMainMenuBar()) return;
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("Open..."))
            {
                var result = NativeFileDialogSharp.Dialog.FileOpen("bf,b98,f98,u98,t98");
                if (result.IsOk)
                {
                    _loadPath = result.Path;
                    _loading = true;
                }
            }

            if (ImGui.MenuItem("New..."))
            {
                _newSpace = true;
            }

            ImGui.EndMenu();
        }
        ImGui.EndMainMenuBar();
    }

    private void LoadFileModal()
    {
        if (!ImGui.BeginPopupModal("Load File")) return;
        int dim = 2;
        ImGui.Text("What kind of space to use?");
        bool pressedAnyButton = false;
        if (ImGui.Button("Unefunge (1 dimension)"))
        {
            dim = 1;
            pressedAnyButton = true;
        }
        ImGui.SameLine();
        if (ImGui.Button("Befunge (2 dimensions)"))
        {
            dim = 2;
            pressedAnyButton = true;
        }
        ImGui.SameLine();
        if (ImGui.Button("Trefunge (3 dimensions)"))
        {
            dim = 3;
            pressedAnyButton = true;
        }

        ImGui.InputInt("Dimensions", ref _customDim);
        if (_customDim < 1) _customDim = 1;
        if (ImGui.Button("Custom"))
        {
            dim = _customDim;
            pressedAnyButton = true;
        }

        if (pressedAnyButton)
        {
            _output = new StringWriter();
            _interpreter = new Interpreter(dim, output: _output);
            _interpreter.Load(_loadPath);
            ImGui.CloseCurrentPopup();
        }
        ImGui.EndPopup();
    }

    private void NewSpaceModal()
    {
        if (ImGui.BeginPopupModal("New Space"))
        {
            ImGui.InputInt("Dimensions", ref _customDim);
            if (_customDim < 1) _customDim = 1;
            if (ImGui.Button("Create"))
            {
                _output = new StringWriter();
                _interpreter = new Interpreter(_customDim, output: _output);
                _interpreter.PrimarySpace[0] = '.';
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }

    private void DrawSpace()
    {
        var end = Window.ClientBounds.Size / _charSize + new Point(2, 2);
        var rightDirection = FungeVector.Cardinal(int.Abs(_rightDim)-1, int.Sign(_rightDim));
        var downDirection = FungeVector.Cardinal(int.Abs(_downDim)-1, int.Sign(_downDim));
        
        for (var y = 0; y < end.Y; y++)
        {
            if (_interpreter.PrimarySpace.Dim < 2 && _topLeft[1] + y != 0) continue;
            for (var x = 0; x < end.X; x++)
            {
                var pos = _topLeft + rightDirection * x + downDirection * y;
                var c = (char)Space[pos];
                if (c == ' ') continue;
                try
                {
                    _spriteBatch.DrawString(_font, c.ToString(),
                        (new Point(x, y) * _charSize - _topLeftPoint).ToVector2(), _textColor);
                }
                catch (ArgumentException e)
                {
                    _spriteBatch.DrawString(_font, "?", (new Point(x, y) * _charSize - _topLeftPoint).ToVector2(), Color.Red);
                }
            }
        }
    }

    private void DrawIPs(Texture2D ipTexture)
    {
        bool cursorDrawn = false;
        var end = Window.ClientBounds.Size / _charSize;
        foreach (var ip in IPList)
        {
            var right = int.Sign(_rightDim) * (ip.Position[int.Abs(_rightDim)-1] - _topLeft[int.Abs(_rightDim)-1]);
            var down = int.Sign(_downDim) * (ip.Position[int.Abs(_downDim)-1] - _topLeft[int.Abs(_downDim)-1]);
            if (right < 0 || right > end.X || down < 0 || down > end.Y) continue;
            if (ip.Position == _cursor)
            {
                cursorDrawn = true;
                _spriteBatch.Draw(ipTexture, new Rectangle(new Point(right, down) * _charSize - _topLeftPoint, _charSize), (1-_cursorBlink) * _ipColor);
                _spriteBatch.Draw(ipTexture, new Rectangle(new Point(right, down) * _charSize - _topLeftPoint, _charSize), _cursorBlink * _cursorColor);
                continue;
            }

            _spriteBatch.Draw(ipTexture, new Rectangle(new Point(right, down) * _charSize - _topLeftPoint, _charSize), _ipColor);
        }

        if (!cursorDrawn)
        {
            var right = int.Sign(_rightDim) * (_cursor[int.Abs(_rightDim)-1] - _topLeft[int.Abs(_rightDim)-1]);
            var down = int.Sign(_downDim) * (_cursor[int.Abs(_downDim)-1] - _topLeft[int.Abs(_downDim)-1]);
            if (right >= 0 && right < end.X && down >= 0 && down < end.Y)
            {
                _spriteBatch.Draw(ipTexture, new Rectangle(new Point(right, down) * _charSize - _topLeftPoint, _charSize), _cursorBlink * _cursorColor);
            }
        }
    }

    protected virtual void OnTextInput(object sender, TextInputEventArgs textInputEventArgs)
    {
        if (ImGuiHasKeyboard || _running) return;
        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl))
        {
            return;
        }
        if (textInputEventArgs.Character == '\b')
        {
            _cursor -= RightDirection;
            _interpreter.PrimarySpace[_cursor] = ' ';
            return;
        }

        if (textInputEventArgs.Character == 127)
        {
            _interpreter.PrimarySpace[_cursor] = ' ';
            return;
        }
        _interpreter.PrimarySpace[_cursor] = textInputEventArgs.Character;
        _cursor += RightDirection;
    }
}