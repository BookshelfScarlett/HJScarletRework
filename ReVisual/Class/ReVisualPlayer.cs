using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.ReVisual.Class
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SaveableBoolAttribute : Attribute { }
    public class ReVisualPlayer : ModPlayer
    {
        [SaveableBool]
        public bool reVisualChlorophyteKnife = false;
        [SaveableBool]
        public bool reVisualHellfire = false;
        [SaveableBool]
        public bool reVisualCobaltKnife = false;
        [SaveableBool]
        public bool reVisualOrihalcumKnife= false;
        [SaveableBool]
        public bool reVisualAdamantiteKnife= false;
        [SaveableBool]
        public bool reVisualMythrilKnife= false;
        [SaveableBool]
        public bool reVisualPalladiumKnife= false;
        [SaveableBool]
        public bool reVisualTitaniumKnife= false;
        [SaveableBool]
        public bool reVisualBloodthirst = false;
        [SaveableBool]
        public bool reVisualBackStabber = false;
        [SaveableBool]
        public bool reVisualOreknife = false;
        [SaveableBool]
        public bool reVisualLonginus = false;
        [SaveableBool]
        public bool reVisualFryingPan = false;
        [SaveableBool]
        public bool reVisualDesertScourge = false;




        // 3. 静态缓存：仅程序启动时初始化一次，所有玩家实例共用
        // 存储：字段名 → (读取委托, 写入委托)
        private static readonly Dictionary<string, (Func<ReVisualPlayer, bool> Getter, Action<ReVisualPlayer, bool> Setter)> _fieldAccessors;
        // 4. 静态构造函数：仅执行一次，扫描字段并缓存委托
        static ReVisualPlayer()
        {
            _fieldAccessors = new Dictionary<string, (Func<ReVisualPlayer, bool>, Action<ReVisualPlayer, bool>)>();

            // 反射扫描所有带SaveableBool特性的字段（仅执行一次）
            var saveableFields = typeof(ReVisualPlayer)
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<SaveableBoolAttribute>() != null && f.FieldType == typeof(bool));

            // 为每个字段创建读/写委托并缓存
            foreach (var field in saveableFields)
            {
                string fieldName = field.Name;

                // 创建读取委托（替代field.GetValue）
                Func<ReVisualPlayer, bool> getter = (player) => (bool)field.GetValue(player);

                // 创建写入委托（替代field.SetValue）
                Action<ReVisualPlayer, bool> setter = (player, value) => field.SetValue(player, value);

                _fieldAccessors.Add(fieldName, (getter, setter));
            }
        }
        // 5. 保存逻辑：调用缓存的委托，无运行时反射
        public override void SaveData(TagCompound tag)
        {
            foreach (var kvp in _fieldAccessors)
            {
                string fieldName = kvp.Key;
                bool value = kvp.Value.Getter(this); // 调用委托读取值
                tag.Add(fieldName, value);
            }
        }

        // 6. 加载逻辑：调用缓存的委托，无运行时反射
        public override void LoadData(TagCompound tag)
        {
            foreach (var kvp in _fieldAccessors)
            {
                string fieldName = kvp.Key;
                // 读值（无字段时用当前值，兼容旧存档）
                bool value = tag.GetBool(fieldName);
                kvp.Value.Setter(this, value); // 调用委托写入值
            }
        }
        //public override void SaveData(TagCompound tag)
        //{
        //    tag.Add(nameof(reVisualChlorophyteKnife), reVisualChlorophyteKnife);
        //    tag.Add(nameof(reVisualHellfire), reVisualHellfire);

        //}
        //public override void LoadData(TagCompound tag)
        //{
        //    reVisualChlorophyteKnife = tag.GetBool(nameof(reVisualChlorophyteKnife));
        //    reVisualHellfire = tag.GetBool(nameof(reVisualHellfire));
        //}
    }
}
