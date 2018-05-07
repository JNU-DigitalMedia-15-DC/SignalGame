/// <summary> 代表一条正弦波的参数组，由 三个float：a， omega， phi 组成 </summary>
[System.Serializable]
public class WaveAttribute {
    public float A, Omega, Phi;

    /// <summary> 初始化构造函数 </summary>
    internal WaveAttribute(float a = 1, float omega = 1, float phi = 0) {
        this.A = a;
        this.Omega = omega;
        this.Phi = phi;
    }
}

/// <summary> 代表波的一次修改的参数组，由 三个float：a， omega， phi 组成 </summary>
[System.Serializable]
public class WaveModification : WaveAttribute {
    /// <summary> 初始化构造函数 </summary>
    internal WaveModification(float a = 1, float omega = 1, float phi = 0):
        base(a, omega, phi) { }

    /// <summary> 拷贝构造 </summary>
    internal WaveModification(WaveModification other):
        this(other.A, other.Omega, other.Phi) { }

    /// <summary> 求差运算符 </summary>
    public static WaveModification operator -(
        WaveModification wm1,
        WaveModification wm2
    ) {
        return new WaveModification(
            wm1.A - wm2.A,
            wm1.Omega - wm2.Omega,
            wm1.Phi - wm2.Phi);
    }

    /// <summary> 求占比运算符 </summary> // TODO
    public static float operator /(
        WaveModification wm1,
        WaveModification wm2
    ) {
        return (wm1.A / wm2.A + wm1.Omega / wm2.Omega + wm1.Phi / wm2.Phi) / 3;
    }

    /// <summary>
    /// 叠加另一个 WaveModification 效果
    /// </summary>
    /// <param name="other"> 要叠加的 WaveModification </param>
    internal void StageWith(WaveModification other) {
        A += other.A;
        Omega *= other.Omega;
        Phi += other.Phi;
    }

    /// <summary>
    /// 拷贝另一个 WaveModification 的内容
    /// </summary>
    /// <param name="other"> 要拷贝的 WaveModification </param>
    internal void CopyFrom(WaveModification other) {
        A = other.A;
        Omega = other.Omega;
        Phi = other.Phi;
    }
}