using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    // 导出变量：在编辑器中指定敌人预制体
    [Export] public PackedScene EnemyScene { get; set; }

    // 导出变量：生成间隔（秒），可在编辑器调整
    [Export] public float SpawnInterval = 2.0f;

    // 随机数生成器
    private RandomNumberGenerator _rng;

    // 记录上次生成敌人的时间
    private float _timeSinceLastSpawn = 0.0f;

    public override void _Ready()
    {
        // 初始化随机数生成器并设置随机种子
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    public override void _Process(double delta)
    {
        // 累计时间（转换为float处理）
        _timeSinceLastSpawn += (float)delta;

        // 当累计时间达到生成间隔，且已指定敌人预制体时生成敌人
        if (_timeSinceLastSpawn >= SpawnInterval && EnemyScene != null)
        {
            // 生成随机位置（基于当前视口范围）
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // 实例化敌人
            Node2D enemy = EnemyScene.Instantiate<Node2D>();
            // 设置敌人位置
            enemy.Position = spawnPosition;
            // 将敌人添加到场景树（作为Spawner的子节点）
            AddChild(enemy);

            // 重置计时器
            _timeSinceLastSpawn = 0.0f;
        }
    }

    // 计算随机生成位置（在当前视口范围内）
    private Vector2 GetRandomSpawnPosition()
    {
        // 获取视口矩形（包含视口位置和大小）
        Rect2 viewportRect = GetViewportRect();

        // 在视口范围内生成随机X和Y坐标
        float randomX = _rng.RandfRange(viewportRect.Position.X, viewportRect.End.X);
        float randomY = _rng.RandfRange(viewportRect.Position.Y, viewportRect.End.Y);

        return new Vector2(randomX, randomY);
    }
}