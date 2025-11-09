using Godot;
using System;

public partial class CharacterBase : CharacterBody2D
{
	[Export]
	public int maxHealth;//最大生命


    public int curHealth;//当前生命

    public Vector2 curOrientation = Vector2.Right;//默认向右, 持久化记录人物朝向


    // 引用经验弹出场景
    [Export] private PackedScene expPopupScene;

    [Export] public PackedScene expBallScene;//死了之后掉落的经验球





    public override void _Ready()
	{
        RefreshAttribute();
    }

	public override void _Process(double delta)
	{
	}

    #region 伤害系统
    [Export]
    public int damage;//伤害

    // 引用伤害弹出场景
    [Export] private PackedScene damagePopupScene;

    public virtual void TakeDamage(int damage)
    {
        ShowDamagePopup(damage);
        curHealth = Mathf.Max(0, curHealth - damage);
        if (curHealth == 0)
        {
            Die();
        }
    }

    // 显示伤害弹出
    private void ShowDamagePopup(int damage)
    {
        if (damagePopupScene == null)
        {
            GD.PrintErr("伤害弹出场景未设置！");
            return;
        }

        // 实例化伤害弹出节点
        DamagePopupLabel popup = (DamagePopupLabel)damagePopupScene.Instantiate();

        // 设置随机偏移，避免多次伤害重叠
        Random random = new Random();
        float offsetX = random.Next(-100, 100); // -20 到 20 之间的随机值

        // 设置位置（敌人位置上方一点，并添加随机偏移）
        popup.GlobalPosition = new Vector2(GlobalPosition.X - 60 + offsetX, GlobalPosition.Y - 30);

        // 设置伤害数值
        popup.SetDamageValue(damage);

        // 添加到当前场景
        GetTree().Root.AddChild(popup);
    }
    #endregion
  

    protected virtual void Die()
    {
        Random random = new Random();
        int exp = 10 * level * level; ;
        while (exp != 0)
        {
            ExpBall expBall = (ExpBall)expBallScene.Instantiate();
            expBall.GlobalPosition = GlobalPosition;
            int curExp = random.Next(200, 500);
            if (exp - curExp > 0)//还有剩下的
            {
                expBall.exp = curExp;
                exp -= curExp;
            }
            else//没了, 
            {
                expBall.exp = exp;
                exp = 0;
            }

            exp = exp - curExp > 0 ? exp - curExp : 0;

            float angleRadian = (float)(random.NextDouble() * 2 * Math.PI); // 0~2π弧度（对应0~360度）
            Vector2 direction = new Vector2((float)Math.Cos(angleRadian), (float)Math.Sin(angleRadian));

            float forceMagnitude = random.Next(500, 1000);

            Vector2 impulse = direction * forceMagnitude;
            expBall.ApplyImpulse(impulse);

            GetTree().CurrentScene.CallDeferred("add_child", expBall);
        }
        QueueFree();
    }



    #region 经验系统
    [Export]
    protected int curExp = 0;//当前经验
    [Export]
    protected int maxExp = 0;//当前最大经验
    [Export]
    protected int level = 1;//等级

    public virtual void TakeExp(int addExp)
	{
        ShowExpPopup(addExp);//弹出获取经验字体
        curExp += addExp;

        // 循环判断是否满足升级条件（支持连续升级）
        while (curExp >= maxExp)
        {
            curExp -= maxExp;  // 减去升级所需的经验
            level++;
            RefreshAttribute();  //根据等级刷新属性
        }
    }
    protected virtual void RefreshAttribute()//升级/设置等级后, 根据等级刷新属性
    {
        //第i级的最大经验(升i+1所需经验)
        //  0   1   2   3   4   5   6   7   8   9   10      11
        //  0   10  40  90  160 250 360 490 640 810 1000    1210
        maxExp = 10 * level * level;

        maxHealth = level * 10 + 100;

        curHealth = maxHealth;

        damage = level * 15 + 10;
    }


    private void ShowExpPopup(int exp)
    {
        if (expPopupScene == null)
        {
            GD.PrintErr("经验弹出场景未设置！");
            return;
        }

        // 实例化伤害弹出节点
        ExpLabel popup = (ExpLabel)expPopupScene.Instantiate();

        // 设置随机偏移，避免多次伤害重叠
        Random random = new Random();
        float offsetX = (float)(random.NextDouble() * 60 - 30); // -20 到 20 之间的随机值

        // 设置位置（人物位置上方一点，并添加随机偏移）
        popup.GlobalPosition = new Vector2(GlobalPosition.X - 60 + offsetX, GlobalPosition.Y - 70);

        popup.SetExpText(exp);

        GetTree().Root.AddChild(popup);
    }
    #endregion


    #region 攻击/技能系统
    public float attackDuration = 0.5f;
    public float attackTimer = 0f;
    public enum AbilityType
    {
        LongSwordWave,//长条射线
        ScatterSwordWave,//圆弧
        AllDirectionSwordWave,//四周散开



    }

    [Export]
    public PackedScene swordWaveScene;//剑气

    [Export]
    public AbilityType curAbility;//当前的技能, 选啥就会释放啥


    protected void LongSwordWave()
    {
        SwordWave wave = swordWaveScene.Instantiate<SwordWave>();
        wave.parent = this;
        wave.damage = damage;
        wave.direction = curOrientation; // 扇形方向
        wave.moveSpeed = 300;
        wave.surviveTime = 5f;
        wave.GlobalPosition = GlobalPosition;

        GetTree().CurrentScene.AddChild(wave);
    }
    protected void ScatterSwordWave()
    {
        int waveCount = 20; // 刀波数量（越多越密集）
        float totalAngle = 60f; // 扇形总角度（度），角度越大范围越宽

        // 计算每个刀波的角度偏移（从左到右均匀分布）
        float startAngle = -totalAngle / 2f; // 起始角度（左半部分）
        float angleStep = waveCount > 1 ? totalAngle / (waveCount - 1) : 0f; // 相邻刀波的角度差

        for (int i = 0; i < waveCount; i++)
        {
            // 计算当前刀波的角度（转换为弧度，Godot旋转用弧度）
            float currentAngle = startAngle + i * angleStep;
            float angleRad = currentAngle * Mathf.Pi / 180f;

            // 基于当前朝向旋转，得到该刀波的方向
            Vector2 waveDir = curOrientation.Rotated(angleRad);

            // 实例化刀波并设置属性
            SwordWave wave = swordWaveScene.Instantiate<SwordWave>();
            wave.parent = this;
            wave.damage = damage;
            wave.direction = waveDir; // 扇形方向
            wave.moveSpeed = 2000;
            wave.surviveTime = 0.3f;
            wave.GlobalPosition = GlobalPosition;

            GetTree().CurrentScene.AddChild(wave);
        }
    }
    protected async void AllDirectionSwordWave()
    {
        int waveCount = 30;
        float totalAngle = 720f;
        float startAngle = -totalAngle / 2f;
        float angleStep = waveCount > 1 ? totalAngle / (waveCount - 1) : 0f;

        float interval = 0.01f; // 每个刀波间隔 0.03 秒（可调）

        for (int i = 0; i < waveCount; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            float angleRad = currentAngle * Mathf.Pi / 180f;
            Vector2 waveDir = curOrientation.Rotated(angleRad);

            SwordWave wave = swordWaveScene.Instantiate<SwordWave>();
            wave.parent = this;
            wave.damage = damage;
            wave.direction = waveDir;
            wave.moveSpeed = 500;
            wave.surviveTime = 2f;
            wave.GlobalPosition = GlobalPosition;

            GetTree().CurrentScene.AddChild(wave);

            // 延时一小段时间再发下一个
            await ToSignal(GetTree().CreateTimer(interval), "timeout");
        }
    }

    #endregion
}
