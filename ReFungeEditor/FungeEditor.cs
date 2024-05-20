using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReFunge;
using ReFunge.Data;
using ReFunge.Data.Values;

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

    private Point _topLeftPoint = Point.Zero;
    
    private FungeVector _cursor = new();
    private FungeVector _topLeft = new();
    private int _rightDim = 1;
    private int _downDim = 2;

    private bool _dragging = false;
    private Point _dragPos;

    public FungeEditor()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Window.AllowUserResizing = true;
        _interpreter = new Interpreter();
        _interpreter.PrimarySpace.LoadString(new FungeVector(), ">\"Hello, World!\">:#,_v\n^t                   <");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _font = Content.Load<SpriteFont>("JetBrainsMonoNL");
        _charSize = _font.MeasureString("M").ToPoint();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        if (Keyboard.GetState().IsKeyDown(Keys.F))
        {
            _interpreter.DoStep();
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Texture2D ipTexture = new Texture2D(GraphicsDevice, 1, 1);
        ipTexture.SetData(new[] {Color.DarkBlue});

        _spriteBatch.Begin();

        DrawIPs(ipTexture);
        
        DrawSpace();

        _spriteBatch.DrawString(_font, $"FPS: {TimeSpan.FromSeconds(1)/gameTime.ElapsedGameTime}\nAmount of IPs: {IPList.Count}", new Vector2(600, 600), Color.White);
        _spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    private void DrawSpace()
    {
        var end = Window.ClientBounds.Size / _charSize;
        var rightDirection = FungeVector.Cardinal(int.Abs(_rightDim)-1, int.Sign(_rightDim));
        var downDirection = FungeVector.Cardinal(int.Abs(_downDim)-1, int.Sign(_downDim));
        
        for (var y = 0; y < end.Y; y++)
        {
            for (var x = 0; x < end.X; x++)
            {
                var pos = _topLeft + rightDirection * x + downDirection * y;
                var c = (char)Space[pos];
                if (c == ' ') continue;
                _spriteBatch.DrawString(_font, c.ToString(), (new Point(x, y) * _charSize - _topLeftPoint).ToVector2(), Color.White);
            }
        }
    }

    private void DrawIPs(Texture2D ipTexture)
    {
        var end = Window.ClientBounds.Size / _charSize;
        foreach (var ip in IPList)
        {
            var right = ip.Position[int.Abs(_rightDim)-1] - _topLeft[int.Abs(_rightDim)-1];
            var down = ip.Position[int.Abs(_downDim)-1] - _topLeft[int.Abs(_downDim)-1];
            if (right >= 0 && right < end.X && down >= 0 && down < end.Y)
            {
                _spriteBatch.Draw(ipTexture, new Rectangle(new Point(right, down) * _charSize - _topLeftPoint, _charSize), Color.White);
            }
        }
    }
}