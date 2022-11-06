
using RaylibEngine.Components;
using RaylibEngine.SceneManagement;
using RaySnake.Logic;
using System.Numerics;

namespace RaySnake.Scenes;
internal class GameScene : Scene
{
    public static readonly string SceneName = nameof(GameScene);

    const int TileSize = 32;
    const int TileSourceSize = 64;
    const int Margin = 56;

    private Texture atlas;
    private Color blockingTileColor = PURPLE;

    private double totalSeconds = 0;
    private Direction requestedDirection = Direction.None;

    private readonly Font defaultFont = GetFontDefault();
    private readonly GameModel gameModel = new();
    private readonly Rectangle[] spriteFrames = new Rectangle[4];
    private readonly Dictionary<Vector2, Sprite> spriteObjects = new();

    public GameScene(string title) : base(SceneName)
    {
        WindowTitle = $"{title}";
        BackgroundColor = DARKPURPLE;

        atlas = LoadTexture("./Assets/atlas.png");
        spriteFrames[(int)FrameType.Head] = new(128, 0, TileSourceSize, TileSourceSize);
        spriteFrames[(int)FrameType.Tail] = new(192, 0, TileSourceSize, TileSourceSize);
        spriteFrames[(int)FrameType.Body] = new(0, 0, TileSourceSize, TileSourceSize);
        spriteFrames[(int)FrameType.BodyCorner] = new(64, 0, TileSourceSize, TileSourceSize);
    }

    public override void OnActivate()
    {
        gameModel.NextLevel();
        CreateObjects();
    }

    public override void OnBeginDraw()
    {
        DrawFPS(5, 2);
        DrawText($"{gameModel.Velocity:N2} T/Ses", 100, 2, 20, LIME);
        DrawText($"{gameModel.MoveDurationSeconds:N2} Sec/T", 250, 2, 20, LIME);
        DrawText($"{totalSeconds:N2} Sec", 450, 2, 20, LIME);
        DrawMenuText(new(10, 20), $"Level: {gameModel.Level}  Food: {gameModel.FoodEaten}/{gameModel.FoodCount}", 30, ORANGE);
    }

    public override void OnEndDraw()
    {
        switch (gameModel.State)
        {
            case GameState.Paused:
                DrawMenuTextCentered(new(0, 20), "press SPACE to start.", 30, GOLD);
                break;
            case GameState.Died:
                DrawMenuTextCentered(new(0, 20), "Congratz, you just killed your snake! Press SPACE to restart.", 30, GOLD);
                break;
            case GameState.LevelCompleted:
                DrawMenuTextCentered(new(0, GetScreenHeight() / 2 - Margin), $"LEVEL {gameModel.Level} COMPLETED!\nPress SPACE to continue.", 50, GOLD);
                break;
        }
        DrawSnake();
    }
    public override void OnBeginUpdate(float ellapsedSeconds)
    {
        //  animate collide-able objects
        var dtSeconds = ellapsedSeconds / 1000d;
        totalSeconds += dtSeconds;
        var sin = (float)Math.Sin(totalSeconds) / 3f + 0.5f;
        blockingTileColor = Fade(PURPLE, sin);

        if (gameModel.State == GameState.Started)
        {
            HandleStartedInput();
        }
        else
        {
            HandleNotStartedInput();
        }
        SetMousePosition(GetScreenWidth() + 100, GetScreenHeight() + 100);
        RemoveDeadSprites();
    }

    #region sync visuals with model
    private void CreateObjects()
    {
        RemoveAllChildren();
        spriteObjects.Clear();
        AddChild(new Ground(0, Margin, GetScreenWidth(), GetScreenHeight(), DARKGREEN));

        for (var j = 0; j < GameModel.GridTilesY; j++)
        {
            for (var i = 0; i < GameModel.GridTilesX; i++)
            {
                var x = i * TileSize;
                var y = j * TileSize + Margin;

                switch (gameModel.Grid[i, j])
                {
                    case TileType.Block:
                        Sprite sprBlock = new(atlas)
                        {
                            Position = new(x, y),
                            Frame = new Rectangle(0, 64, TileSourceSize, TileSourceSize),
                            Width = TileSize,
                            Height = TileSize,
                        };
                        AddChild(sprBlock);
                        spriteObjects[new(i, j)] = sprBlock;
                        break;

                    case TileType.Bomb:
                        AnimatedSprite aspr = new(atlas)
                        {
                            Position = new(x, y),
                            Width = TileSize,
                            Height = TileSize,
                            Name = "Bomb"
                        };
                        aspr.AddAnimation("bomb", new Rectangle[] {
                                    new(64, 128, TileSourceSize, TileSourceSize),
                                    new(128, 128, TileSourceSize, TileSourceSize),
                                    new(192, 128, TileSourceSize, TileSourceSize),
                                    new(64, 192, TileSourceSize, TileSourceSize),
                                    new(128, 192, TileSourceSize, TileSourceSize),
                                    new(192, 192, TileSourceSize, TileSourceSize)
                        });
                        aspr.Play("bomb", 4);
                        AddChild(aspr);
                        spriteObjects[new(i, j)] = aspr;
                        break;

                    case TileType.FoodFrog:
                        Sprite sprFrog = new(atlas)
                        {
                            Position = new(x, y),
                            Frame = new Rectangle(0, 128, TileSourceSize, TileSourceSize),
                            Width = TileSize,
                            Height = TileSize,
                        };
                        AddChild(sprFrog);
                        spriteObjects[new(i, j)] = sprFrog;
                        break;

                    case TileType.FoodApple:
                        Sprite sprApple = new(atlas)
                        {
                            Position = new(x, y),
                            Frame = new Rectangle(0, 192, TileSourceSize, TileSourceSize),
                            Width = TileSize,
                            Height = TileSize,
                        };
                        AddChild(sprApple);
                        spriteObjects[new(i, j)] = sprApple;
                        break;
                };
            }
        }
    }

    private void RemoveDeadSprites()
    {
        for (var j = 0; j < GameModel.GridTilesY; j++)
        {
            for (var i = 0; i < GameModel.GridTilesX; i++)
            {
                if (gameModel.Grid[i, j] == TileType.Empty && spriteObjects.ContainsKey(new(i, j)))
                {
                    var child = spriteObjects[new(i, j)];
                    RemoveChild(child);
                }
            }
        }
    }
    #endregion


    private void HandleSnakeDestroyed()
    {
        requestedDirection = Direction.None;
        DrawSnake();
    }

    private DateTime nextMove = DateTime.Now;
    private void HandleStartedInput()
    {
        var direction = ReadGameInput();
        if (direction != Direction.None) requestedDirection = direction;

        if (nextMove <= DateTime.Now)
        {
            nextMove = DateTime.Now.AddSeconds(gameModel.MoveDurationSeconds);
            if (!gameModel.MoveSnake(requestedDirection == Direction.None ? gameModel.SnakeDirection : requestedDirection))
            {
                HandleSnakeDestroyed();
            }
        }
    }

    private void HandleNotStartedInput()
    {
        if (IsKeyDown(KeyboardKey.KEY_SPACE))
        {
            if (gameModel.State == GameState.Died)
            {
                requestedDirection = Direction.None;
                gameModel.RestartLevel();
            }
            else if (gameModel.State == GameState.LevelCompleted)
            {
                requestedDirection = Direction.None;
                gameModel.NextLevel();
            }
            gameModel.TogglePause();
            CreateObjects();
        }
    }

    private Direction ReadGameInput()
    {
        var direction = IsKeyDown(KeyboardKey.KEY_RIGHT) ? Direction.East :
                              IsKeyDown(KeyboardKey.KEY_LEFT) ? Direction.West :
                              IsKeyDown(KeyboardKey.KEY_UP) ? Direction.North :
                              IsKeyDown(KeyboardKey.KEY_DOWN) ? Direction.South :
                              Direction.None;

        //  prevent 180 degree turns
        if (direction != Direction.None)
        {
            direction = (direction == Direction.North && gameModel.SnakeDirection == Direction.South) ? Direction.None :
                        (direction == Direction.South && gameModel.SnakeDirection == Direction.North) ? Direction.None :
                        (direction == Direction.West && gameModel.SnakeDirection == Direction.East) ? Direction.None :
                        (direction == Direction.East && gameModel.SnakeDirection == Direction.West) ? Direction.None : direction;
        }

        return direction;
    }

    private void DrawSnake()
    {
        var halfSize = TileSize / 2;
        Rectangle dst = new(0, 0, TileSize, TileSize);
        foreach (var tile in gameModel.SnakeTiles)
        {
            var src = spriteFrames[(int)tile.FrameType];
            dst.x = tile.X * TileSize + halfSize;
            dst.y = tile.Y * TileSize + halfSize + Margin;
            int rotation;
            if (tile.FrameType == FrameType.BodyCorner)
            {
                rotation = tile.CornerDirection switch
                {
                    Direction.South => 90,
                    Direction.West => 180,
                    Direction.North => 270,
                    _ => 0
                };
            }
            else
            {
                rotation = tile.Direction switch
                {
                    Direction.South => 90,
                    Direction.West => 180,
                    Direction.North => 270,
                    _ => 0
                };
            }
            DrawTexturePro(atlas, src, dst, new(halfSize, halfSize), rotation, WHITE);
        }
    }

    private void DrawMenuTextCentered(Vector2 offset, string text, int fontSize, Color color)
    {
        var w = GetScreenWidth();
        var size = MeasureTextEx(defaultFont, text, fontSize, 1);
        DrawText(text, offset.X + (w - size.X) / 2, offset.Y + (TileSize - size.Y) / 2, fontSize, color);
    }

    private void DrawMenuText(Vector2 offset, string text, int fontSize, Color color)
    {
        var size = MeasureTextEx(defaultFont, text, fontSize, 1);
        DrawText(text, offset.X, offset.Y + (TileSize - size.Y) / 2, fontSize, color);
    }
}