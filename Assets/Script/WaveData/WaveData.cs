using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 波的数据 —— 数据起始的链表节点 和 数据终止的链表节点 </summary>
internal class WaveData {
    /// <summary> 波形数据的蒙版的列表 </summary>
    private List<WaveDataMask> waveDataMasks;

    /// <summary>
    /// 由 多个WaveAttribute 生成新 WaveData
    /// </summary>
    /// <param name="waveAttributeList"> WaveAttribute 的数组 </param>
    internal WaveData(WaveAttribute[] waveAttributeList) {
        // 初始化蒙版
        waveDataMasks = new List<WaveDataMask> { new WaveDataMask() };

        // 添加 WaveAttribute
        foreach (WaveAttribute wa in waveAttributeList)
            waveDataMasks[0].AddLast(wa);
    }

    /// <summary>
    /// 由 多个WaveData 生成一个 表示和视图的WaveData
    /// </summary>
    /// <param name="waveDataList"> 待累加 WaveData 的数组 </param>
    /// <remarks> 会创建自己的 waveDataMasks，记录别人的 WaveDataMask们 </remarks>
    internal WaveData(WaveData[] waveDataList) {
        // 新建 waveDataMasks
        waveDataMasks = new List<WaveDataMask>();
        // 注：waveDataMasks 其下的 WaveDataMask 与 源Mask 同体
        foreach (WaveData wd in waveDataList)
            waveDataMasks.AddRange(wd.waveDataMasks);
    }

    /// <summary>
    /// 拷贝 每个WaveDataMask和WaveModification 的构造
    /// </summary>
    /// <param name="waveData"> 被拷贝的 WaveData </param>
    internal WaveData(WaveData waveData) {
        // 新建 waveDataMasks
        waveDataMasks = new List<WaveDataMask>();
        // 逐个拷贝 WaveDataMask，拷贝级别达到 拷贝每个WaveModification
        foreach (WaveDataMask wdm in waveData.waveDataMasks)
            waveDataMasks.Add(new WaveDataMask(wdm));
    }
    public void DebugList()
    {
        Debug.Log(waveDataMasks[0]);
        Debug.Log(waveDataMasks[1]);
    }

    /// <summary>
    /// 修改 WaveData 的 第index个 WaveDataMask
    /// </summary>
    /// <param name="index"> 被改 WaveDataMask 的索引 </param>
    /// <param name="modification"> 修改量 </param>
    internal void ModifyByMask(int index, WaveModification modification) {
        waveDataMasks[index].Modification.StageWith(modification);
    }

    /// <summary>
    /// 试验性原型：获取 WaveData 的 第一个WaveDataMask 的 WaveModification

    /// 获取可操作纸片的modification，默认一个
    /// </summary>
    /// <returns> WaveData 的 第一个WaveDataMask 的 WaveModification </returns>
    internal WaveModification GetWaveModificationPrototype() { // TODO
        return waveDataMasks[0].Modification;
    }

    /// <summary>
    /// 试验性原型：获取 总和纸片wavedata的两个mask总和为modification
    /// </summary>
    /// <returns> 总和modification </returns>
    internal WaveModification GetSumWaveModification(){
        WaveModification sumModification = new WaveModification();
        sumModification.StageWith(waveDataMasks[0].Modification);
        sumModification.StageWith(waveDataMasks[1].Modification);
        return sumModification;
    }

    /// <summary>
    /// 获取波在横坐标为 x 时的函数值
    /// </summary>
    /// <param name="x"> 求值用到的横坐标 x </param>
    /// <returns> 返回横坐标为 x 时波的函数值 </returns>
    internal float ReturnValueAt(float x) {
        float y = 0;
        // 累加获得 y，每次累加的值为此次循环得到的 WaveAttribute 被蒙版修改后的值
        foreach (WaveDataMask mask in waveDataMasks) {
            WaveModification mod = mask.Modification;
            foreach (WaveAttribute wa in mask)
                y += (wa.A * mod.A) *
                Mathf.Sin((wa.Omega * mod.Omega) *
                    (x + wa.Phi + mod.Phi));
        }
        return y;
    }

    /// <summary> 覆盖并影响连续一串波参数三元组的蒙版 </summary>
    private class WaveDataMask : IEnumerable<WaveAttribute> {
        /// <summary> 蒙版所属波参数节点之首 </summary>
        internal WaveDataNode First;
        /// <summary> 蒙版所属波参数节点之尾 </summary>
        internal WaveDataNode Last;
        /// <summary> 蒙版所记录的对其下所有波参数的修改 </summary>
        internal WaveModification Modification;

        /// <summary> 默认构造函数 </summary>
        internal WaveDataMask() { Modification = new WaveModification(); }

        /// <summary>
        /// 拷贝 WaveModification 的构造
        /// </summary>
        /// <param name="wdm"> 被拷贝的 WaveDataMask </param>
        internal WaveDataMask(WaveDataMask wdm) {
            First = wdm.First;
            Last = wdm.Last;
            Modification = new WaveModification(wdm.Modification);
        }

        /// <summary> 在蒙版所属链表最后插入节点 </summary>
        internal void AddLast(WaveAttribute waveAttribute) {
            Last = new WaveDataNode {
                Prevous = Last,
                Next = null,
                Value = waveAttribute
            };
            if (Last.Prevous != null)
                Last.Prevous.Next = Last;
            else
                First = Last;
        }

        #region 以下为 IEnumerable接口 的实现，遍历将会返回蒙版其下!!原始!!的 WaveAttribute
        public IEnumerator<WaveAttribute> GetEnumerator() {
            return new WaveDataMaskEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private class WaveDataMaskEnumerator : IEnumerator<WaveAttribute> {
            private WaveDataMask wdm;
            private WaveDataNode node = null;

            internal WaveDataMaskEnumerator(WaveDataMask wdm) {
                this.wdm = wdm;
            }

            public WaveAttribute Current {
                get { return node.Value; }
            }

            object IEnumerator.Current {
                get { return Current; }
            }

            public bool MoveNext() {
                if (node == null)
                    node = wdm.First;
                else
                    node = node.Next;
                return node != null;
            }

            public void Reset() {
                node = null;
            }

            public void Dispose() { }
        }
        #endregion
    }

    /// <summary> 波的数据的链表节点 </summary>
    private class WaveDataNode {
        internal WaveDataNode Prevous, Next;
        internal WaveAttribute Value;
    }
}