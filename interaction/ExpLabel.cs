using Godot;
using System;

public partial class ExpLabel : Label
{
    [Export]
    public Color TextColor = new Color(0f, 1f, 0f); // 默认绿色经验文字

    [Export] public float fadeTime = 1f; // 消失时间
    [Export] public float maxScale = 1f; // 最大缩放比例
    [Export] public float minScale = 0.5f; // 最终缩放比例
    [Export] public float scalePeakTime = 0.1f; // 达到最大缩放的时间点(占总时间的比例)
    [Export] public float riseSpeed = 100f; // 上升速度（像素/秒）

    private float _timeElapsed = 0f;

    public override void _Ready()
    {
        Random random = new Random();
        // 随机微调参数，让效果更自然
        fadeTime += (float)random.NextDouble() * 0.6f - 0.3f;
        maxScale += (float)random.NextDouble() * 0.2f - 0.1f; // 缩小随机范围，避免缩放异常
        scalePeakTime += (float)random.NextDouble() * 0.02f - 0.01f;

        // 初始化设置
        CustomMinimumSize = new Vector2(100, 50);
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;

        // 设置缩放原点为中心
        PivotOffset = new Vector2(CustomMinimumSize.X / 2, CustomMinimumSize.Y / 2);
        Scale = Vector2.One; // 初始缩放为1
    }

    public override void _Process(double delta)
    {
        // 计算时间流逝
        _timeElapsed += (float)delta;
        float t = _timeElapsed / fadeTime; // 时间进度(0-1)

        // 上升效果（Y轴减小=向上移动）
        Position = new Vector2(Position.X, Position.Y - riseSpeed * (float)delta);

        // 缩放效果 - 先变大后变小
        float scaleFactor;
        if (t <= scalePeakTime)
        {
            // 前段时间: 从初始大小放大到最大
            scaleFactor = Mathf.Lerp(1f, maxScale, t / scalePeakTime);
        }
        else
        {
            // 后段时间: 从最大缩小到最小
            float remainingT = (t - scalePeakTime) / (1 - scalePeakTime);
            scaleFactor = Mathf.Lerp(maxScale, minScale, remainingT);
        }
        Scale = new Vector2(scaleFactor, scaleFactor);

        // 修复淡出效果（保留原颜色的RGB，只修改alpha值）
        float alpha = 1 - t;
        Modulate = new Color(TextColor.R, TextColor.G, TextColor.B, alpha);

        // 时间到了就销毁自己
        if (_timeElapsed >= fadeTime)
        {
            QueueFree();
        }
    }

    // 设置经验值文本
    public void SetExpText(int exp)
    {
        Text = "Exp+" + exp;
    }
}