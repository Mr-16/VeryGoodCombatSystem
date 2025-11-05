using Godot;
using System;

public partial class DamagePopupLabel : Label
{
    [Export] public float fadeTime = 1f; // 消失时间
    [Export] public float maxScale = 2f; // 最大缩放比例
    [Export] public float minScale = 0.1f; // 最终缩放比例
    [Export] public float scalePeakTime = 0.03f; // 达到最大缩放的时间点(占总时间的比例)

    private float _timeElapsed = 0f;
    private Color _originalColor;

    public override void _Ready()
    {
        Random random = new Random();
        fadeTime += (float)random.NextDouble() * 0.6f - 0.3f;
        maxScale += (float)random.NextDouble() * 0.2f - 0.4f;
        scalePeakTime += (float)random.NextDouble() * 0.02f - 0.01f;


        // 初始化设置
        _originalColor = Modulate;
        CustomMinimumSize = new Vector2(100, 50);
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;

        // 设置缩放原点为中心，确保缩放效果围绕中心展开
        PivotOffset = new Vector2(CustomMinimumSize.X / 2, CustomMinimumSize.Y / 2);
        Scale = Vector2.One; // 初始缩放为1
    }

    public override void _Process(double delta)
    {
        // 计算时间流逝
        _timeElapsed += (float)delta;
        float t = _timeElapsed / fadeTime; // 时间进度(0-1)

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

        // 淡出效果
        float alpha = 1 - t;
        Modulate = new Color(
            _originalColor.R,
            _originalColor.G,
            _originalColor.B,
            alpha
        );

        // 时间到了就销毁自己
        if (_timeElapsed >= fadeTime)
        {
            QueueFree();
        }
    }

    // 设置伤害数值和颜色
    public void SetDamageValue(int damage, Color color = default(Color))
    {
        Text = damage.ToString();

        color = new Color(1f, 0f, 0f); // 红色
        _originalColor = color;
        Modulate = color;
    }
}
